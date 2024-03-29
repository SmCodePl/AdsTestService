

using AdsTestService.Helper;
using AdsTestService.Interfaces;
using AdsTestService.Model;
using AdsTestService.PlcStructure;
using Microsoft.Extensions.Options;
using Microsoft.VisualBasic;
using System.Diagnostics;
using System.Reactive.Concurrency;
using System.Reflection.PortableExecutable;
using System.Runtime.InteropServices;
using TwinCAT.Ads;

namespace AdsTestService.Services;

public class PlcConnectionService : IPlcConnectionService<AdsClient>
{
    private readonly ILogger<PlcConnectionService> _logger;
    private readonly IConfiguration _configuration;
    private readonly IWorkQueue _workQueue;

    private AdsClient _client = new();
    
    public PlcConnectionService(ILogger<PlcConnectionService> logger, IConfiguration configuration, IWorkQueue workQueue)
    {
        _logger = logger;
        _configuration = configuration;
        _workQueue = workQueue;
    }
    private AmsAddress GetAmsAddress(IConfiguration configuration)
    {
        int portNr = 0;
        string ip = configuration.GetSection("AmsSettings:NetId").Value ?? "";
        int.TryParse(configuration.GetSection("AmsSettings:AmsPort").Value, out portNr);

        return new AmsAddress(ip, portNr);
    }
    public Task<AdsClient> Connect()
    {
        try
        {
            _client.Connect(GetAmsAddress(_configuration));

            if(_client.IsConnected)
            {
                RegisterNotifications();
            }

            return Task.FromResult(_client);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error connecting to PLC");
            return default!;
        }

    }
    public Task Disconnect()
    {
        try
        {
            _client.AdsNotificationEx -= OnNotificationReceivedEx;
            _client.Disconnect();
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error disconnecting from PLC");
            return Task.FromException(ex);
        }
    }
    protected void RegisterNotifications()
    {
        //// Subscribe to multiple fields in a structure

        _client.AdsNotificationEx += OnNotificationReceivedEx;

        var notificationSettings = new NotificationSettings(AdsTransMode.OnChange, 20, 0);
        var variableHandle1 = _client.AddDeviceNotificationEx("Global.stAdsTxData.nItemId", notificationSettings, null, typeof(short));


        _client.AdsNotificationError += (sender, e) =>
        {
            Debug.WriteLine($"Notification error: {e.Exception}");
        };
    }

    protected void OnNotificationReceivedEx(object sender, AdsNotificationExEventArgs e)
    {
        if (e != null)
        {
            HangleRequest();
        }

    }
    protected void HangleRequest()
    {
        if(_client.IsConnected)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            short result = ReadPlcData("Global.stAdsTxData", out string readTime, out string writeTime);
            
            stopwatch.Stop();

            if (result != 0)
            {
                TimeSpan timeTaken = stopwatch.Elapsed;

                WorkItem workItem = new WorkItem
                {

                    ItemId = result,
                    pcTimeStamp = readTime,
                    plcTimeStamp = writeTime,
                    timeSpan = timeTaken
                };

                _workQueue.Enqueue(workItem);
            }
        }
        else
        {
           _logger.LogInformation("PLC is not connected");
        }
    }
    private short ReadPlcData(string structureName, out string readTieme, out string writeTime)
    {
        uint handleData = _client.CreateVariableHandle(structureName + ".stDataPayload");
        uint handleHeader = _client.CreateVariableHandle(structureName);
        short ret = 0; 
        readTieme = ""; writeTime = "";

        try
        {

            var headerData = (HeaderStruct)_client.ReadAny(handleHeader, typeof(HeaderStruct));

            readTieme = headerData.TimeStampLog[0].ReadTime = DateTime.UtcNow.ToString("o");

            var data = (DataType)_client.ReadAny(handleData, typeof(DataType));

            if (WritePlcData(ref data, ref headerData, out writeTime))
            {

            }
            ret = headerData.ItemId;
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
        }
        finally
        {
            if (handleData != 0) _client.DeleteVariableHandle(handleData);
            if (handleHeader != 0) _client.DeleteVariableHandle(handleHeader);

        }

        return ret;
    }

    private bool WritePlcData( ref DataType body, ref HeaderStruct header, out string writeTime)
    {
        uint writeHeader = _client.CreateVariableHandle("Global.stAdsRxData");
        uint writeBody = _client.CreateVariableHandle("Global.stAdsRxData.stDataPayload");
        writeTime = "";
        try
        {
            if(writeHeader == 0 || writeBody == 0) return false;

            _client.WriteAny(writeBody, body);
            writeTime = header.TimeStampLog[0].WriteTiem = DateTime.UtcNow.ToString("o");
            _client.WriteAny(writeHeader, header);
            
            return true;

        }catch(Exception ex)
        {
            Debug.WriteLine(ex.Message);
            return false;
        }
        finally
        {
            if(writeHeader != 0) _client.DeleteVariableHandle(writeHeader);
            if(writeBody != 0) _client.DeleteVariableHandle(writeBody);
        }

    }
   
  
}


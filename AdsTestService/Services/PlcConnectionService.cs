using PlcDataModel;
using PlcDataModel.Abstract;
using PlcDataModel.Interfaces;
using PlcDataModel.PlcStructore;
using PlcDataModel.PlcStructure;
using System.Diagnostics;
using TwinCAT.Ads;

namespace AdsTestService.Services;

public class PlcConnectionService : PlcRequestHelper, IPlcConnectionService<AdsClient>
{
    private readonly ILogger<PlcConnectionService> _logger;
    private readonly IConfiguration _configuration;

    private AdsClient _client = new();
    
    public PlcConnectionService(ILogger<PlcConnectionService> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
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
            Console.WriteLine($"Notification error: {e.Exception}");
        };
    }

    protected void OnNotificationReceivedEx(object sender, AdsNotificationExEventArgs e)
    {
        if (e != null)
        {
            HandleRequest();
        }
    }
    protected override void HandleRequest()
    {
        if(_client.IsConnected)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            short result = DoMeasurment("Global.stAdsTxData", "Global.stAdsTxData.stDataPayload", "Global.stAdsRxData", "Global.stAdsRxData.stDataPayload", out string readTime, out string writeTime);
            
            stopwatch.Stop();
            Console.WriteLine("Result: " + result);
            if (result != 0)
            {
                TimeSpan timeTaken = stopwatch.Elapsed;
                Console.WriteLine($"Time Taken: {timeTaken}");
                
                _logger.LogInformation("{ReadTime}, {WriteTime}, {OperationTimeTaken}", readTime,writeTime,timeTaken);
            }
        }
        else
        {
           _logger.LogInformation("PLC is not connected");
        }
    }
    protected override short DoMeasurment(string readHeader, string readBody, string writeHeader, string writeBody, out string strReadExedution, out string strWriteExecution)
    {
        return ReadPlcData(readHeader, readBody, writeHeader, writeBody, out strReadExedution, out strWriteExecution);
    }
    private short ReadPlcData(string structureHeader, string structBody, string writeHeader,string writeBody ,out string readTieme, out string writeTime)
    {
        uint handleHeader = _client.CreateVariableHandle(structureHeader);
        uint handleBody = _client.CreateVariableHandle(structBody);
        short ret = 0; 
        readTieme = ""; writeTime = "";

        try
        {
            var headerData = (HeaderStruct)_client.ReadAny(handleHeader, typeof(HeaderStruct));
            
            readTieme = headerData.TimeStampLog[0].WriteTiem = TestDataHelper.GetTimeStamp();
            
            var data = (AdsDataType)_client.ReadAny(handleBody, typeof(AdsDataType));
            if (WritePlcData(writeHeader, writeBody,ref data, ref headerData, out writeTime))
            {

            }
            ret = headerData.ItemId;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        finally
        {
            if (handleBody != 0) _client.DeleteVariableHandle(handleBody);
            if (handleHeader != 0) _client.DeleteVariableHandle(handleHeader);

        }

        return ret;
    }

    private bool WritePlcData(string writeHeader,string writeBody, ref AdsDataType body, ref HeaderStruct header, out string writeTime)
    {
        uint writeHeaderHandle = _client.CreateVariableHandle(writeHeader);
        uint writeBodyHandle = _client.CreateVariableHandle(writeBody);
        writeTime = "";
        try
        {
            if(writeHeaderHandle == 0 || writeBodyHandle == 0) return false;

            TestDataHelper.SetBitInShort(ref header.ItemStatus, 1, true);// = 3;
            _client.WriteAny(writeBodyHandle, body);
           
            writeTime = header.TimeStampLog[0].ReadTime = TestDataHelper.GetTimeStamp(); 
            _client.WriteAny(writeHeaderHandle, header);
            
            return true;

        }catch(Exception ex)
        {
            Console.WriteLine(ex.Message);
            return false;
        }
        finally
        {
            if(writeHeaderHandle != 0) _client.DeleteVariableHandle(writeHeaderHandle);
            if(writeBodyHandle != 0) _client.DeleteVariableHandle(writeBodyHandle);
        }

    }
}


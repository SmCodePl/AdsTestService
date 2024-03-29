

using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using System.Security.Permissions;
using TwinCAT.Ads;

namespace AdsTestService.Helper
{
    public abstract class PlcRequestHelper
    {
        
        protected abstract int ReadPlcData(string structureName, AdsClient client);
        protected abstract bool WritePlcData(string structureName, AdsClient client);

        //protected abstract T? ReadPlcData(string structureName, AdsClient client);
        //protected abstract bool WritePlcData(string structureName, T structure, AdsClient client);
        //protected virtual byte[] GetBytes(ref T structure)
        //{
        //    var size = Marshal.SizeOf(typeof(T));
        //    var result = new byte[size];
        //    var handle = GCHandle.Alloc(result, GCHandleType.Pinned);
        //    try
        //    {
        //        Marshal.StructureToPtr(structure, handle.AddrOfPinnedObject(), false);
        //    }
        //    finally
        //    {
        //        handle.Free();
        //    }
        //    return result;
        //}
        //protected virtual T? GetStructure(ref byte[] data)
        //{
        //    var pData = GCHandle.Alloc(data, GCHandleType.Pinned);
        //    try
        //    {
        //        var result = (T?)Marshal.PtrToStructure(pData.AddrOfPinnedObject(), typeof(T));
        //        return result;
        //    }
        //    finally
        //    {
        //        pData.Free();
        //    }
        //}
    }
}

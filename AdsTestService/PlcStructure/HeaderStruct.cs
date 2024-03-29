

using System.Runtime.InteropServices;

namespace AdsTestService.PlcStructure
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct HeaderStruct
    {
        public short ItemId;
        public byte ItemStatus;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]
        public LogTimeStamp[] TimeStampLog;

        public HeaderStruct()
        {
            TimeStampLog = new LogTimeStamp[1];
        }
    }
}

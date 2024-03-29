
using System.Runtime.InteropServices;

namespace AdsTestService.PlcStructure
{
    [StructLayout(LayoutKind.Sequential,Pack =1, CharSet = CharSet.Ansi)]
    public struct TestStructure
    {
        public byte    ItemStatus;
        public short   ItemId;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]
        public LogTimeStamp[] TimeStampLog;

       
        public DataType DataTypes;
        public TestStructure()
        {
            TimeStampLog = new LogTimeStamp[1];
            DataTypes = new DataType();
        }
    }
}

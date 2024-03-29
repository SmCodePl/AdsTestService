using System.Runtime.InteropServices;

namespace AdsTestService.PlcStructure
{
    [StructLayout(LayoutKind.Sequential,Pack = 1, CharSet = CharSet.Ansi)]
    public struct LogTimeStamp
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 31)]
        public string ReadTime;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 31)]
        public string WriteTiem;

    }
}

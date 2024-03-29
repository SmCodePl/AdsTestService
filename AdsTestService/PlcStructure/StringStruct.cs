
using System.Runtime.InteropServices;

namespace AdsTestService.PlcStructure
{
    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
    public struct StringStruct
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 89)]
        public string PlcCodeString;
    }
}


using System.Runtime.InteropServices;

namespace AdsTestService.PlcStructure
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BoolStruct
    {
        [MarshalAs(UnmanagedType.I1)]
        public bool BoolValue; 
    }
}

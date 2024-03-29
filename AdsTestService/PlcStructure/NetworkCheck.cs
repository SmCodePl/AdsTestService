using System.Runtime.InteropServices;
namespace AdsTestService.PlcStructure;

[StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
public struct NetworkCheck
{
    public short CheckNetwork;
    public short EchoCheckNetwork;
}

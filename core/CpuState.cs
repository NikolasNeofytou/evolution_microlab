namespace Microlab.Core
{
    public class CpuState
    {
        public byte A { get; set; }
        public byte B { get; set; }
        public byte C { get; set; }
        public byte D { get; set; }
        public byte E { get; set; }
        public byte H { get; set; }
        public byte L { get; set; }
        public ushort PC { get; set; }
        public ushort SP { get; set; }
        // Flags: S, Z, AC, P, CY
        public bool S { get; set; }
        public bool Z { get; set; }
        public bool AC { get; set; }
        public bool P { get; set; }
        public bool CY { get; set; }
    }
}

namespace Microlab.Core
{
    public interface IBus
    {
        byte ReadByte(ushort addr);
        void WriteByte(ushort addr, byte value);
        byte InPort(byte port);
        void OutPort(byte port, byte value);
    }
}

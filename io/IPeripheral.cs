namespace Microlab.IO;

public interface IPeripheral
{
    IEnumerable<byte> Ports { get; }
    byte ReadPort(byte port);
    void WritePort(byte port, byte value);
}

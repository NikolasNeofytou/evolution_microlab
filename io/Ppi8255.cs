namespace Microlab.IO;

public class Ppi8255 : IPeripheral
{
    private readonly byte _base;
    private readonly byte[] _ports;
    private byte _control;

    public byte PortA { get; private set; }
    public byte PortB { get; private set; }
    public byte PortC { get; private set; }

    public byte PortBInput { get; set; }

    public Ppi8255(byte basePort = 0x10)
    {
        _base = basePort;
        _ports = new[] { basePort, (byte)(basePort + 1), (byte)(basePort + 2), (byte)(basePort + 3) };
    }

    public IEnumerable<byte> Ports => _ports;

    public byte ReadPort(byte port)
    {
        if (port == _base) return PortA;
        if (port == _base + 1) return PortBInput;
        if (port == _base + 2) return PortC;
        if (port == _base + 3) return _control;
        return 0;
    }

    public void WritePort(byte port, byte value)
    {
        if (port == _base) PortA = value;
        else if (port == _base + 1) PortB = value;
        else if (port == _base + 2) PortC = value;
        else if (port == _base + 3) _control = value;
    }
}

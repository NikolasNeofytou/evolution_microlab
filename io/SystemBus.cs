using Microlab.Core;

namespace Microlab.IO;

public class SystemBus : IBus
{
    private readonly byte[] _memory = new byte[ushort.MaxValue + 1];
    private readonly Dictionary<byte, IPeripheral> _portMap = new();

    public byte[] Memory => _memory;
    public List<(byte Port, byte Value)> PortWrites { get; } = new();

    public SystemBus(byte[] program, ushort origin, IEnumerable<IPeripheral>? peripherals = null)
    {
        Array.Copy(program, 0, _memory, origin, program.Length);
        if (peripherals != null)
        {
            foreach (var device in peripherals)
                foreach (var port in device.Ports)
                    _portMap[port] = device;
        }
    }

    public byte ReadByte(ushort addr) => _memory[addr];
    public void WriteByte(ushort addr, byte value) => _memory[addr] = value;

    public byte InPort(byte port)
    {
        if (_portMap.TryGetValue(port, out var device))
            return device.ReadPort(port);
        return 0;
    }

    public void OutPort(byte port, byte value)
    {
        PortWrites.Add((port, value));
        if (_portMap.TryGetValue(port, out var device))
            device.WritePort(port, value);
    }
}

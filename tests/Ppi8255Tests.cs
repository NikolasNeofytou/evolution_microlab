using Microlab.IO;
using Xunit;

namespace Microlab.Tests;

public class Ppi8255Tests
{
    [Fact]
    public void PortAWriteIsLatched()
    {
        var ppi = new Ppi8255();
        var bus = new SystemBus(Array.Empty<byte>(), 0, new[] { ppi });
        bus.OutPort(0x10, 0xAA);
        Assert.Equal(0xAA, ppi.PortA);
        Assert.Equal(0xAA, bus.InPort(0x10));
    }

    [Fact]
    public void PortBReadReturnsInput()
    {
        var ppi = new Ppi8255();
        ppi.PortBInput = 0x55;
        var bus = new SystemBus(Array.Empty<byte>(), 0, new[] { ppi });
        Assert.Equal(0x55, bus.InPort(0x11));
    }
}

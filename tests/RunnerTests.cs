using Microlab.Grader;
using Microlab.IO;
using System.Linq;
using Xunit;

namespace Microlab.Tests;

public class RunnerTests
{
    [Fact]
    public void RunProgramProducesExpectedState()
    {
        var source = """
ORG 0
MVI A,1
ADI 1
HLT
END
""";
        var result = Runner.Run(source);
        Assert.Equal(2, result.Cpu.State.A);
    }

    [Fact]
    public void TraceProducesEntries()
    {
        var source = """
ORG 0
MVI A,1
ADI 1
HLT
END
""";
        var result = Runner.Trace(source);
        Assert.Equal(3, result.Trace.Count);
        Assert.Equal(0, result.Trace[0].PC);
        Assert.Equal(0x3E, result.Trace[0].Op);
        Assert.Equal(2, result.Cpu.State.A);
        Assert.Empty(result.MemoryDiff);
    }

    [Fact]
    public void RunWithPeripheralCapturesPortWrite()
    {
        var source = """
ORG 0
MVI A,0AAH
OUT 10H
HLT
END
""";
        var ppi = new Ppi8255();
        var result = Runner.Run(source, peripherals: new[] { ppi });
        Assert.Contains((byte)0x10, result.PortWrites.Select(p => p.Port));
        Assert.Equal(0xAA, ppi.PortA);
    }
}

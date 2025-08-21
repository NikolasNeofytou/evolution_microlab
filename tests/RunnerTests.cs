using Microlab.Grader;
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
}

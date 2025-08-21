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
        var cpu = Runner.Run(source);
        Assert.Equal(2, cpu.State.A);
    }
}

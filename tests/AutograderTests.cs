using Microlab.Grader;
using Xunit;

namespace Microlab.Tests;

public class AutograderTests
{
    [Fact]
    public void SpecPasses()
    {
        var dir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        Directory.CreateDirectory(dir);
        var progPath = Path.Combine(dir, "prog.8085");
        File.WriteAllText(progPath, """
ORG 0
MVI A,1
ADI 1
HLT
END
""");
        var specPath = Path.Combine(dir, "spec.json");
        File.WriteAllText(specPath, """
{
  "program": "prog.8085",
  "entry": 0,
  "maxCycles": 1000,
  "expect": {
    "registers": {"A": "0x02"}
  }
}
""");
        Assert.True(Microlab.Grader.Grader.Grade(specPath, TextWriter.Null));
    }
}

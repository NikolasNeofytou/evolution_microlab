
using Microlab.Grader;
using System.Text.Json;

if (args.Length == 0)
{
    Console.WriteLine("Usage: ml85 run <source-file> [--entry N] [--max-cycles N]\n       ml85 grade <spec.json>\n       ml85 trace <source-file> [--entry N] [--max-cycles N] [--out file]");
    return;
}

if (args[0] == "grade")
{
    if (args.Length < 2)
    {
        Console.WriteLine("Usage: ml85 grade <spec.json>");
        return;
    }
    var ok = Grader.Grade(args[1]);
    Environment.Exit(ok ? 0 : 1);
    return;
}

if (args[0] == "trace")
{
    if (args.Length < 2)
    {
        Console.WriteLine("Usage: ml85 trace <source-file> [--entry N] [--max-cycles N] [--out file]");
        return;
    }
    var path = args[1];
    ushort entry = 0;
    int maxCycles = 100000;
    string outPath = "trace.json";
    for (int i = 2; i < args.Length; i++)
    {
        switch (args[i])
        {
            case "--entry":
                entry = Convert.ToUInt16(args[++i], 16);
                break;
            case "--max-cycles":
                maxCycles = int.Parse(args[++i]);
                break;
            case "--out":
                outPath = args[++i];
                break;
        }
    }
    var source = File.ReadAllText(path);
    var result = Runner.Trace(source, entry, maxCycles);
    var cpu = result.Cpu;
    var obj = new
    {
        trace = result.Trace.Select(t => new { pc = $"0x{t.PC:X4}", op = $"0x{t.Op:X2}" }),
        registers = new
        {
            A = $"0x{cpu.State.A:X2}",
            B = $"0x{cpu.State.B:X2}",
            C = $"0x{cpu.State.C:X2}",
            D = $"0x{cpu.State.D:X2}",
            E = $"0x{cpu.State.E:X2}",
            H = $"0x{cpu.State.H:X2}",
            L = $"0x{cpu.State.L:X2}",
            S = cpu.State.S,
            Z = cpu.State.Z,
            AC = cpu.State.AC,
            P = cpu.State.P,
            CY = cpu.State.CY
        },
        memory = result.MemoryDiff.Select(m => new[] { $"0x{m.Address:X4}", $"0x{m.Value:X2}" }),
        ports = result.PortWrites.Select(p => new[] { $"0x{p.Port:X2}", $"0x{p.Value:X2}" })
    };
    var json = JsonSerializer.Serialize(obj, new JsonSerializerOptions { WriteIndented = true });
    File.WriteAllText(outPath, json);
    return;
}

int index = args[0] == "run" ? 1 : 0;
if (args.Length <= index)
{
    Console.WriteLine("Usage: ml85 run <source-file> [--entry N] [--max-cycles N]");
    return;
}

var pathRun = args[index];
ushort entryRun = 0;
int maxCyclesRun = 100000;
for (int i = index + 1; i < args.Length; i++)
{
    switch (args[i])
    {
        case "--entry":
            entryRun = Convert.ToUInt16(args[++i], 16);
            break;
        case "--max-cycles":
            maxCyclesRun = int.Parse(args[++i]);
            break;
    }
}

var sourceRun = File.ReadAllText(pathRun);
var runResult = Runner.Run(sourceRun, entryRun, maxCyclesRun);
var runCpu = runResult.Cpu;
Console.WriteLine($"A={runCpu.State.A:X2} B={runCpu.State.B:X2} C={runCpu.State.C:X2} D={runCpu.State.D:X2} E={runCpu.State.E:X2} H={runCpu.State.H:X2} L={runCpu.State.L:X2} PC={runCpu.State.PC:X4} SP={runCpu.State.SP:X4}");
Console.WriteLine($"Flags: S={(runCpu.State.S?1:0)} Z={(runCpu.State.Z?1:0)} AC={(runCpu.State.AC?1:0)} P={(runCpu.State.P?1:0)} CY={(runCpu.State.CY?1:0)}");



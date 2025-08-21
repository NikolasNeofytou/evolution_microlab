using Microlab.Grader;

if (args.Length == 0)
{
    Console.WriteLine("Usage: ml85 <source-file> [--entry N] [--max-cycles N]");
    return;
}

var path = args[0];
ushort entry = 0;
int maxCycles = 100000;
for (int i = 1; i < args.Length; i++)
{
    switch (args[i])
    {
        case "--entry":
            entry = Convert.ToUInt16(args[++i], 16);
            break;
        case "--max-cycles":
            maxCycles = int.Parse(args[++i]);
            break;
    }
}

var source = File.ReadAllText(path);
var cpu = Runner.Run(source, entry, maxCycles);
Console.WriteLine($"A={cpu.State.A:X2} B={cpu.State.B:X2} C={cpu.State.C:X2} D={cpu.State.D:X2} E={cpu.State.E:X2} H={cpu.State.H:X2} L={cpu.State.L:X2} PC={cpu.State.PC:X4} SP={cpu.State.SP:X4}");
Console.WriteLine($"Flags: S={(cpu.State.S?1:0)} Z={(cpu.State.Z?1:0)} AC={(cpu.State.AC?1:0)} P={(cpu.State.P?1:0)} CY={(cpu.State.CY?1:0)}");

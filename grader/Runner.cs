using Microlab.Asm8085;
using Microlab.Core;
using Microlab.IO;

namespace Microlab.Grader;

public record RunResult(Cpu8085 Cpu, byte[] Memory, List<(byte Port, byte Value)> PortWrites);
public record TraceEntry(ushort PC, byte Op);
public record TraceResult(List<TraceEntry> Trace, Cpu8085 Cpu, List<(ushort Address, byte Value)> MemoryDiff, List<(byte Port, byte Value)> PortWrites);

public static class Runner
{
    public static RunResult Run(string source, ushort entry = 0, int maxCycles = 100000, IEnumerable<IPeripheral>? peripherals = null)
    {
        var assembler = new Assembler();
        var program = assembler.Assemble(source);
        var bus = new SystemBus(program, entry, peripherals);
        var cpu = new Cpu8085();
        cpu.Reset();
        cpu.State.PC = entry;
        var cycles = 0;
        while (!cpu.Halted && cycles < maxCycles)
            cycles += cpu.Step(bus);
        return new RunResult(cpu, bus.Memory, bus.PortWrites);
    }

    public static TraceResult Trace(string source, ushort entry = 0, int maxCycles = 100000, IEnumerable<IPeripheral>? peripherals = null)
    {
        var assembler = new Assembler();
        var program = assembler.Assemble(source);
        var bus = new SystemBus(program, entry, peripherals);
        var initialMemory = (byte[])bus.Memory.Clone();
        var cpu = new Cpu8085();
        cpu.Reset();
        cpu.State.PC = entry;
        var trace = new List<TraceEntry>();
        var cycles = 0;
        while (!cpu.Halted && cycles < maxCycles)
        {
            ushort pc = cpu.State.PC;
            byte op = bus.ReadByte(pc);
            trace.Add(new TraceEntry(pc, op));
            cycles += cpu.Step(bus);
        }

        var diff = new List<(ushort Address, byte Value)>();
        for (int i = 0; i < initialMemory.Length; i++)
        {
            if (bus.Memory[i] != initialMemory[i])
                diff.Add(((ushort)i, bus.Memory[i]));
        }

        return new TraceResult(trace, cpu, diff, bus.PortWrites);
    }
}

using Microlab.Asm8085;
using Microlab.Core;

namespace Microlab.Grader;

public record RunResult(Cpu8085 Cpu, byte[] Memory, List<(byte Port, byte Value)> PortWrites);
public record TraceEntry(ushort PC, byte Op);
public record TraceResult(List<TraceEntry> Trace, Cpu8085 Cpu, List<(ushort Address, byte Value)> MemoryDiff, List<(byte Port, byte Value)> PortWrites);

public static class Runner
{
    public static RunResult Run(string source, ushort entry = 0, int maxCycles = 100000)
    {
        var assembler = new Assembler();
        var program = assembler.Assemble(source);
        var bus = new MemoryBus(program, entry);
        var cpu = new Cpu8085();
        cpu.Reset();
        cpu.State.PC = entry;
        var cycles = 0;
        while (!cpu.Halted && cycles < maxCycles)
            cycles += cpu.Step(bus);
        return new RunResult(cpu, bus.Memory, bus.PortWrites);
    }

    public static TraceResult Trace(string source, ushort entry = 0, int maxCycles = 100000)
    {
        var assembler = new Assembler();
        var program = assembler.Assemble(source);
        var bus = new MemoryBus(program, entry);
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

    private class MemoryBus : IBus
    {
        private readonly byte[] _memory = new byte[ushort.MaxValue + 1];
        public byte[] Memory => _memory;
        public List<(byte Port, byte Value)> PortWrites { get; } = new();

        public MemoryBus(byte[] program, ushort origin)
        {
            Array.Copy(program, 0, _memory, origin, program.Length);
        }
        public byte ReadByte(ushort addr) => _memory[addr];
        public void WriteByte(ushort addr, byte value) => _memory[addr] = value;
        public byte InPort(byte port) => 0;
        public void OutPort(byte port, byte value) => PortWrites.Add((port, value));
    }
}

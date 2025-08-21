using Microlab.Asm8085;
using Microlab.Core;
using Xunit;

namespace Microlab.Tests;

public class Cpu8085Tests
{
    private class SimpleBus : IBus
    {
        private readonly byte[] _memory = new byte[ushort.MaxValue + 1];

        public SimpleBus(byte[] program, ushort origin = 0)
        {
            Array.Copy(program, 0, _memory, origin, program.Length);
        }

        public byte ReadByte(ushort addr) => _memory[addr];
        public void WriteByte(ushort addr, byte value) => _memory[addr] = value;
        public byte InPort(byte port) => 0;
        public void OutPort(byte port, byte value) { }
    }

    [Fact]
    public void AddsImmediate()
    {
        var source = """
ORG 0
MVI A,1
ADI 1
HLT
END
""";
        var asm = new Assembler();
        var program = asm.Assemble(source);
        var bus = new SimpleBus(program);
        var cpu = new Cpu8085();
        cpu.Reset();

        while (!cpu.Halted)
            cpu.Step(bus);

        Assert.Equal(2, cpu.State.A);
        Assert.False(cpu.State.Z);
    }
}

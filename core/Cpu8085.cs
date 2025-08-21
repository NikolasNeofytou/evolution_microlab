using System;

namespace Microlab.Core;

public class Cpu8085 : ICpu8085
{
    public CpuState State { get; } = new();
    public bool Halted { get; private set; }

    public void Reset()
    {
        State.A = State.B = State.C = State.D = State.E = State.H = State.L = 0;
        State.PC = 0;
        State.SP = 0;
        State.S = State.Z = State.AC = State.P = State.CY = false;
        Halted = false;
    }

    public int Step(IBus bus)
    {
        if (Halted)
            return 0;

        ushort pc = State.PC;
        byte op = bus.ReadByte(pc);
        State.PC++;

        // MVI r,# pattern 00ddd110
        if ((op & 0xC7) == 0x06)
        {
            int reg = (op >> 3) & 0x7;
            if (reg == 6)
                throw new CpuException("MVI M not supported");
            SetReg(reg, bus.ReadByte(State.PC++));
            return 7;
        }

        // MOV r1,r2 pattern 01dddsss
        if ((op & 0xC0) == 0x40)
        {
            if (op == 0x76)
            {
                Halted = true;
                return 7;
            }
            int dstCode = (op >> 3) & 0x7;
            int srcCode = op & 0x7;
            if (dstCode == 6 || srcCode == 6)
                throw new CpuException("MOV involving M not supported");
            byte value = GetReg(srcCode);
            SetReg(dstCode, value);
            return 5;
        }

        // ADD r pattern 10000sss
        if ((op & 0xF8) == 0x80)
        {
            int srcCode = op & 0x7;
            if (srcCode == 6)
                throw new CpuException("ADD M not supported");
            byte value = GetReg(srcCode);
            Add(value);
            return 4;
        }

        switch (op)
        {
            case 0x00: // NOP
                return 4;
            case 0xC6: // ADI #
            {
                byte value = bus.ReadByte(State.PC++);
                Add(value);
                return 7;
            }
            case 0xC3: // JMP addr
            {
                byte lo = bus.ReadByte(State.PC++);
                byte hi = bus.ReadByte(State.PC++);
                State.PC = (ushort)(lo | (hi << 8));
                return 10;
            }
            case 0xD3: // OUT port
            {
                byte port = bus.ReadByte(State.PC++);
                bus.OutPort(port, State.A);
                return 10;
            }
            case 0xDB: // IN port
            {
                byte port = bus.ReadByte(State.PC++);
                State.A = bus.InPort(port);
                return 10;
            }
            case 0x76: // HLT (should be caught above but keep)
                Halted = true;
                return 7;
            default:
                throw new CpuException($"Unknown opcode 0x{op:X2} at 0x{pc:X4}");
        }
    }

    private byte GetReg(int code) => code switch
    {
        0 => State.B,
        1 => State.C,
        2 => State.D,
        3 => State.E,
        4 => State.H,
        5 => State.L,
        7 => State.A,
        _ => throw new CpuException("Register M not supported")
    };

    private void SetReg(int code, byte value)
    {
        switch (code)
        {
            case 0: State.B = value; break;
            case 1: State.C = value; break;
            case 2: State.D = value; break;
            case 3: State.E = value; break;
            case 4: State.H = value; break;
            case 5: State.L = value; break;
            case 7: State.A = value; break;
            default: throw new CpuException("Register M not supported");
        }
    }

    private void Add(byte value)
    {
        int result = State.A + value;
        State.CY = result > 0xFF;
        State.AC = ((State.A & 0x0F) + (value & 0x0F)) > 0x0F;
        State.A = (byte)result;
        State.Z = State.A == 0;
        State.S = (State.A & 0x80) != 0;
        State.P = Parity(State.A);
    }

    private static bool Parity(byte value)
    {
        int bits = 0;
        for (int i = 0; i < 8; i++)
            bits += (value >> i) & 1;
        return (bits & 1) == 0;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using Microlab.Core;

namespace Microlab.Asm8085;

public class Assembler
{
    public byte[] Assemble(string source)
    {
        var symbols = new Dictionary<string, ushort>(StringComparer.OrdinalIgnoreCase);
        var lines = new List<(string Op, string[] Operands)>();
        ushort pc = 0;

        foreach (var raw in source.Split('\n'))
        {
            var line = raw;
            int comment = line.IndexOf(';');
            if (comment >= 0)
                line = line[..comment];
            line = line.Trim();
            if (line.Length == 0)
                continue;

            string label = null;
            int colon = line.IndexOf(':');
            if (colon >= 0)
            {
                label = line[..colon].Trim();
                line = line[(colon + 1)..].Trim();
                symbols[label] = pc;
            }
            if (line.Length == 0)
                continue;

            var tokens = line.Split(new[] { ' ', '\t', ',' }, StringSplitOptions.RemoveEmptyEntries);
            var op = tokens[0].ToUpperInvariant();
            var operands = tokens.Skip(1).ToArray();

            lines.Add((op, operands));

            if (op == "ORG")
            {
                pc = ParseNumber(operands[0]);
                continue;
            }

            pc += op switch
            {
                "DB" => (ushort)1,
                "MVI" or "ADI" => (ushort)2,
                "JMP" => (ushort)3,
                "END" => (ushort)0,
                _ => (ushort)1,
            };

            if (op == "END")
                break;
        }

        pc = 0;
        var output = new List<byte>();
        foreach (var (op, operands) in lines)
        {
            switch (op)
            {
                case "ORG":
                    pc = ParseNumber(operands[0]);
                    break;
                case "DB":
                    output.Add((byte)ParseNumberOrLabel(operands[0], symbols));
                    pc++;
                    break;
                case "MVI":
                {
                    int reg = RegisterCode(operands[0]);
                    if (reg == 6)
                        throw new CpuException("MVI M not supported");
                    output.Add((byte)(0x06 | (reg << 3)));
                    output.Add((byte)ParseNumberOrLabel(operands[1], symbols));
                    pc += 2;
                    break;
                }
                case "MOV":
                {
                    int dst = RegisterCode(operands[0]);
                    int src = RegisterCode(operands[1]);
                    if (dst == 6 || src == 6)
                        throw new CpuException("MOV involving M not supported");
                    output.Add((byte)(0x40 | (dst << 3) | src));
                    pc++;
                    break;
                }
                case "ADD":
                {
                    int src = RegisterCode(operands[0]);
                    if (src == 6)
                        throw new CpuException("ADD M not supported");
                    output.Add((byte)(0x80 | src));
                    pc++;
                    break;
                }
                case "ADI":
                {
                    output.Add(0xC6);
                    output.Add((byte)ParseNumberOrLabel(operands[0], symbols));
                    pc += 2;
                    break;
                }
                case "JMP":
                {
                    ushort addr = ParseNumberOrLabel(operands[0], symbols);
                    output.Add(0xC3);
                    output.Add((byte)(addr & 0xFF));
                    output.Add((byte)(addr >> 8));
                    pc += 3;
                    break;
                }
                case "HLT":
                    output.Add(0x76);
                    pc++;
                    break;
                case "END":
                    goto End;
                default:
                    throw new CpuException($"Unknown opcode '{op}'");
            }
        }
End:
        return output.ToArray();
    }

    private static ushort ParseNumberOrLabel(string token, IDictionary<string, ushort> symbols)
    {
        if (symbols.TryGetValue(token, out var value))
            return value;
        return ParseNumber(token);
    }

    private static ushort ParseNumber(string token)
    {
        token = token.Trim();
        if (token.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
            return Convert.ToUInt16(token[2..], 16);
        if (token.EndsWith("H", StringComparison.OrdinalIgnoreCase))
            return Convert.ToUInt16(token[..^1], 16);
        return Convert.ToUInt16(token, 10);
    }

    private static int RegisterCode(string name)
    {
        return name.ToUpperInvariant() switch
        {
            "B" => 0,
            "C" => 1,
            "D" => 2,
            "E" => 3,
            "H" => 4,
            "L" => 5,
            "M" => 6,
            "A" => 7,
            _ => throw new CpuException($"Unknown register '{name}'")
        };
    }
}

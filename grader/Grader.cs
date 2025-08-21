using System.Text.Json;
using Microlab.Core;

namespace Microlab.Grader;

public static class Grader
{
    public static bool Grade(string specPath, TextWriter? log = null)
    {
        log ??= Console.Out;
        var json = File.ReadAllText(specPath);
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        var spec = JsonSerializer.Deserialize<TestSpec>(json, options) ?? throw new InvalidOperationException("Invalid spec");
        var baseDir = Path.GetDirectoryName(specPath) ?? "";
        var programPath = Path.IsPathRooted(spec.Program) ? spec.Program : Path.Combine(baseDir, spec.Program);
        var source = File.ReadAllText(programPath);
        var result = Runner.Run(source, spec.Entry, spec.MaxCycles);
        var cpu = result.Cpu;
        bool success = true;

        foreach (var kvp in spec.Expect.Registers)
        {
            var name = kvp.Key.ToUpperInvariant();
            var elem = kvp.Value;
            switch (name)
            {
                case "A": success &= CheckByte(cpu.State.A, elem, name, log); break;
                case "B": success &= CheckByte(cpu.State.B, elem, name, log); break;
                case "C": success &= CheckByte(cpu.State.C, elem, name, log); break;
                case "D": success &= CheckByte(cpu.State.D, elem, name, log); break;
                case "E": success &= CheckByte(cpu.State.E, elem, name, log); break;
                case "H": success &= CheckByte(cpu.State.H, elem, name, log); break;
                case "L": success &= CheckByte(cpu.State.L, elem, name, log); break;
                case "S": success &= CheckBool(cpu.State.S, elem, name, log); break;
                case "Z": success &= CheckBool(cpu.State.Z, elem, name, log); break;
                case "AC": success &= CheckBool(cpu.State.AC, elem, name, log); break;
                case "P": success &= CheckBool(cpu.State.P, elem, name, log); break;
                case "CY": success &= CheckBool(cpu.State.CY, elem, name, log); break;
            }
        }

        foreach (var pair in spec.Expect.Memory)
        {
            var addr = ParseUShort(pair[0]);
            var expected = ParseByte(pair[1]);
            var actual = result.Memory[addr];
            if (actual != expected)
            {
                log.WriteLine($"Memory[0x{addr:X4}] expected 0x{expected:X2} but was 0x{actual:X2}");
                success = false;
            }
        }

        foreach (var pair in spec.Expect.Ports)
        {
            var port = ParseByte(pair[0]);
            var value = ParseByte(pair[1]);
            if (!result.PortWrites.Any(p => p.Port == port && p.Value == value))
            {
                log.WriteLine($"Port 0x{port:X2} expected output 0x{value:X2}");
                success = false;
            }
        }

        if (success)
            log.WriteLine("PASS");
        else
            log.WriteLine("FAIL");
        return success;
    }

    private static bool CheckByte(byte actual, JsonElement elem, string name, TextWriter log)
    {
        var expected = ParseByte(elem.GetString()!);
        if (actual != expected)
        {
            log.WriteLine($"Register {name} expected 0x{expected:X2} but was 0x{actual:X2}");
            return false;
        }
        return true;
    }

    private static bool CheckBool(bool actual, JsonElement elem, string name, TextWriter log)
    {
        var expected = elem.GetBoolean();
        if (actual != expected)
        {
            log.WriteLine($"Flag {name} expected {expected} but was {actual}");
            return false;
        }
        return true;
    }

    private static byte ParseByte(string token)
    {
        if (token.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
            return Convert.ToByte(token[2..], 16);
        if (token.EndsWith("H", StringComparison.OrdinalIgnoreCase))
            return Convert.ToByte(token[..^1], 16);
        return Convert.ToByte(token, 10);
    }

    private static ushort ParseUShort(string token)
    {
        if (token.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
            return Convert.ToUInt16(token[2..], 16);
        if (token.EndsWith("H", StringComparison.OrdinalIgnoreCase))
            return Convert.ToUInt16(token[..^1], 16);
        return Convert.ToUInt16(token, 10);
    }
}

public class TestSpec
{
    public string Program { get; set; } = string.Empty;
    public ushort Entry { get; set; } = 0;
    public int MaxCycles { get; set; } = 100000;
    public ExpectSection Expect { get; set; } = new();
}

public class ExpectSection
{
    public Dictionary<string, JsonElement> Registers { get; set; } = new();
    public List<string[]> Memory { get; set; } = new();
    public List<string[]> Ports { get; set; } = new();
}

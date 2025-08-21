using Avalonia;
using Avalonia.Controls;
using Microlab.Asm8085;
using Microlab.Core;
using Microlab.IO;
using System;
using System.Linq;

namespace Microlab.UiDesktop;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
#if DEBUG
        this.AttachDevTools();
#endif
        CodeBox.Text = "ORG 0\nMVI A,0AAH\nOUT 10H\nMVI A,3FH\nOUT 12H\nHLT\nEND";
    }

    private void OnRunClicked(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var assembler = new Assembler();
        try
        {
            var program = assembler.Assemble(CodeBox.Text ?? string.Empty);
            var ppi = new Ppi8255();
            ppi.PortBInput = DipInput.Value;
            var bus = new SystemBus(program, 0, new[] { ppi });
            var cpu = new Cpu8085();
            cpu.Reset();
            var cycles = 0;
            while (!cpu.Halted && cycles < 100000)
                cycles += cpu.Step(bus);
            LedOutput.Value = ppi.PortA;
            SegOutput.Value = ppi.PortC;
            var regs = $"A={cpu.State.A:X2} B={cpu.State.B:X2} C={cpu.State.C:X2} D={cpu.State.D:X2} E={cpu.State.E:X2} H={cpu.State.H:X2} L={cpu.State.L:X2} PC={cpu.State.PC:X4} SP={cpu.State.SP:X4}";
            var flags = $"S={(cpu.State.S?1:0)} Z={(cpu.State.Z?1:0)} AC={(cpu.State.AC?1:0)} P={(cpu.State.P?1:0)} CY={(cpu.State.CY?1:0)}";
            var mem = string.Join(' ', bus.Memory.Take(16).Select(b => b.ToString("X2")));
            OutputBlock.Text = $"Registers: {regs}\nFlags: {flags}\nMem[0..15]: {mem}";
        }
        catch (Exception ex)
        {
            OutputBlock.Text = ex.Message;
        }
    }
}

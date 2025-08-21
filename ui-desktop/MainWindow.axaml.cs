using Avalonia;
using Avalonia.Controls;
using Microlab.Asm8085;
using Microlab.Core;
using System;

namespace Microlab.UiDesktop;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
#if DEBUG
        this.AttachDevTools();
#endif
        CodeBox.Text = "ORG 0\nMVI A,1\nADI 1\nHLT\nEND";
    }

    private void OnRunClicked(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var assembler = new Assembler();
        try
        {
            var program = assembler.Assemble(CodeBox.Text ?? string.Empty);
            var bus = new SimpleBus(program);
            var cpu = new Cpu8085();
            cpu.Reset();
            while (!cpu.Halted)
                cpu.Step(bus);
            OutputBlock.Text = $"A={cpu.State.A}";
        }
        catch (Exception ex)
        {
            OutputBlock.Text = ex.Message;
        }
    }

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
}

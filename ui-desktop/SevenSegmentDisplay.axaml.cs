using Avalonia.Controls;
using Avalonia.Media;

namespace Microlab.UiDesktop;

public partial class SevenSegmentDisplay : UserControl
{
    private byte _value;

    public SevenSegmentDisplay()
    {
        InitializeComponent();
    }

    public byte Value
    {
        get => _value;
        set
        {
            _value = value;
            UpdateSegments();
        }
    }

    private void UpdateSegments()
    {
        var segments = new[] { SegA, SegB, SegC, SegD, SegE, SegF, SegG };
        for (int i = 0; i < 7; i++)
        {
            segments[i].Background = ((_value >> i) & 1) == 1 ? Brushes.Red : Brushes.Black;
        }
    }
}

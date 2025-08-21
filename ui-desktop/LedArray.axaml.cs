using Avalonia.Controls;
using Avalonia.Media;

namespace Microlab.UiDesktop;

public partial class LedArray : UserControl
{
    private byte _value;

    public LedArray()
    {
        InitializeComponent();
    }

    public byte Value
    {
        get => _value;
        set
        {
            _value = value;
            UpdateLeds();
        }
    }

    private void UpdateLeds()
    {
        var leds = new[] { Led0, Led1, Led2, Led3, Led4, Led5, Led6, Led7 };
        for (int i = 0; i < 8; i++)
        {
            leds[i].Background = ((_value >> i) & 1) == 1 ? Brushes.Red : Brushes.Black;
        }
    }
}

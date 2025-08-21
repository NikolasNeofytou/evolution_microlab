using Avalonia.Controls;

namespace Microlab.UiDesktop;

public partial class DipSwitch : UserControl
{
    public DipSwitch()
    {
        InitializeComponent();
    }

    public byte Value
    {
        get
        {
            byte v = 0;
            if (Sw0.IsChecked == true) v |= 1 << 0;
            if (Sw1.IsChecked == true) v |= 1 << 1;
            if (Sw2.IsChecked == true) v |= 1 << 2;
            if (Sw3.IsChecked == true) v |= 1 << 3;
            if (Sw4.IsChecked == true) v |= 1 << 4;
            if (Sw5.IsChecked == true) v |= 1 << 5;
            if (Sw6.IsChecked == true) v |= 1 << 6;
            if (Sw7.IsChecked == true) v |= 1 << 7;
            return v;
        }
        set
        {
            Sw0.IsChecked = (value & (1 << 0)) != 0;
            Sw1.IsChecked = (value & (1 << 1)) != 0;
            Sw2.IsChecked = (value & (1 << 2)) != 0;
            Sw3.IsChecked = (value & (1 << 3)) != 0;
            Sw4.IsChecked = (value & (1 << 4)) != 0;
            Sw5.IsChecked = (value & (1 << 5)) != 0;
            Sw6.IsChecked = (value & (1 << 6)) != 0;
            Sw7.IsChecked = (value & (1 << 7)) != 0;
        }
    }
}

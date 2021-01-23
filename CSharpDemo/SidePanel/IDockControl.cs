using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace VenueRtcWpf
{
    public interface IDockControl
    {
        bool IsDocked { get; set; }
        string ClassName { get; }
    }
    public class DockControl
    {
        public static readonly DependencyProperty IsDockedProperty =
            DependencyProperty.RegisterAttached("IsDocked", typeof(bool), typeof(IDockControl), new PropertyMetadata(true));
    }
}

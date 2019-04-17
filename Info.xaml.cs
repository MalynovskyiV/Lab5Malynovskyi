using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MalynovskyiLab5
{
    /// <summary>
    /// Логика взаимодействия для Info.xaml
    /// </summary>
   public partial class Info : Window
    {
        private InfoWindow _infoWindow;

        internal Info(System.Diagnostics.Process process)
        {
            InitializeComponent();
            Title = $"{process.ProcessName} Info";
            ShowInfoWindow(process);
        }

        private void ShowInfoWindow(System.Diagnostics.Process process)
        {
            MainGrid.Children.Clear();
            if (_infoWindow == null)
                _infoWindow = new InfoWindow(process);
            MainGrid.Children.Add(_infoWindow);
        }
    }
}

using System;
using System.Windows.Controls;

namespace MalynovskyiLab5
{
    /// <summary>
    /// Логика взаимодействия для InfoWindow.xaml
    /// </summary>
    public partial class InfoWindow : UserControl
    {
        internal InfoWindow(System.Diagnostics.Process process)
        {
            InitializeComponent();
            DataContext = new InfoVM(process.Modules, process.Threads);
        }
    }
}

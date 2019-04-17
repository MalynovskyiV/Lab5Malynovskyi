using System;
using System.Windows.Controls;

namespace MalynovskyiLab5
{
    /// <summary>
    /// Логика взаимодействия для ProcessesWindow.xaml
    /// </summary>
    public partial class ProcessesWindow : UserControl
    {
        internal ProcessesWindow(Action<bool> showLoaderAction)
        {
            InitializeComponent();
            DataContext = new ProcessesVM(showLoaderAction);
        }

        internal void Close()
        {
            ((ProcessesVM)DataContext).Close();
        }
    }
}

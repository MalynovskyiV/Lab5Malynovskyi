using System.ComponentModel;
using System.Windows;
using FontAwesome.WPF;

namespace MalynovskyiLab5
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ImageAwesome _loader;
        private ProcessesWindow _processesWindow;

        public MainWindow()
        {
            InitializeComponent();
            ShowProcessesListView();
        }

        private void ShowProcessesListView()
        {
            MainGrid.Children.Clear();
            if (_processesWindow == null)
                _processesWindow = new ProcessesWindow(ShowLoader);
            MainGrid.Children.Add(_processesWindow);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            _processesWindow?.Close();
            ProcessMod2.Close();
            base.OnClosing(e);
        }

        private void ShowLoader(bool isShow)
        {
            LoaderHelp.OnRequestLoader(MainGrid, ref _loader, isShow);
        }
    }
}

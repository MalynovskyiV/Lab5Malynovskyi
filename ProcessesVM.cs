using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace MalynovskyiLab5
{
    internal class ProcessesVM : INotifyPropertyChanged
    {
        private readonly Action<bool> _showLoaderAction;
        private ObservableCollection<ProcessMod1> _processes;
        private readonly Thread _updateThread;
        private ProcessMod1 _selectedProcess;
        private RelayCommand<object> _endTaskCommand;
        private RelayCommand<object> _getInfoCommand;
        private RelayCommand<object> _openFileLocationCommand;
        private Info _info;

        public bool IsItemSelected
        {
            get
            {
                if (SelectedProcess != null) return true;
                return false;
            }
        }

        public ProcessMod1 SelectedProcess
        {
            get
            {
                return _selectedProcess;
            }
            set
            {
                _selectedProcess = value;
                OnPropertyChanged();

            }
        }

        public ObservableCollection<ProcessMod1> Processes
        {
            get => _processes;
            private set
            {
                _processes = value;
                OnPropertyChanged();
            }
        }

        internal ProcessesVM(Action<bool> showLoaderAction)
        {
            _showLoaderAction = showLoaderAction;
            _updateThread = new Thread(UpdateUsers);
            Thread initializationThread = new Thread(InitializeProcesses);
            initializationThread.Start();
        }

        public RelayCommand<object> EndTaskCommand => _endTaskCommand ?? (_endTaskCommand = new RelayCommand<object>(EndTaskImpl));
        public RelayCommand<object> GetInfoCommand => _getInfoCommand ?? (_getInfoCommand = new RelayCommand<object>(GetInfoImpl));
        public RelayCommand<object> OpenFileLocationCommand => _openFileLocationCommand ?? (_openFileLocationCommand = new RelayCommand<object>(OpenFileLocationImpl));

        private void EndTaskImpl(object o)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(delegate
            {
                System.Diagnostics.Process process = System.Diagnostics.Process.GetProcessById(SelectedProcess.Id);
                try
                {
                    process.Kill();
                }
                catch (Win32Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            });
        }

        private async void GetInfoImpl(object o)
        {
            try
            {
                await Task.Run(() =>
                {
                    System.Windows.Application.Current.Dispatcher.Invoke(delegate
                    {
                        System.Diagnostics.Process process = System.Diagnostics.Process.GetProcessById(SelectedProcess.Id);
                        _info?.Close();
                        try
                        {
                            _info = new Info(process);
                            _info.Show();
                        }
                        catch (Win32Exception e)
                        {
                            MessageBox.Show(e.Message);
                        }
                    });
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async void OpenFileLocationImpl(object o)
        {
            await Task.Run(() =>
            {
                System.Diagnostics.Process process = System.Diagnostics.Process.GetProcessById(SelectedProcess.Id);
                try
                {
                    string fullPath = process.MainModule.FileName;
                    System.Diagnostics.Process.Start("", fullPath.Remove(fullPath.LastIndexOf('\\')));
                }
                catch (Win32Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            });
        }

        private async void UpdateUsers()
        {
            while (true)
            {
                await Task.Run(() =>
                {
                    System.Windows.Application.Current.Dispatcher.Invoke(delegate
                    {
                        try
                        {
                            lock (Processes)
                            {
                                List<ProcessMod1> toRemove =
                                    new List<ProcessMod1>(
                                        Processes.Where(proc => !ProcessMod2.Processes.ContainsKey(proc.Id)));
                                foreach (ProcessMod1 proc in toRemove)
                                {
                                    Processes.Remove(proc);
                                }

                                List<ProcessMod1> toAdd =
                                    new List<ProcessMod1>(
                                        ProcessMod2.Processes.Values.Where(proc => !Processes.Contains(proc)));
                                foreach (ProcessMod1 proc in toAdd)
                                {
                                    Processes.Add(proc);
                                }
                            }
                        }
                        catch (NullReferenceException e)
                        {
                            MessageBox.Show(e.Message);
                        }
                        catch (ArgumentNullException e)
                        {
                            MessageBox.Show(e.Message);
                        }
                        catch (InvalidOperationException e)
                        {
                            MessageBox.Show(e.Message);
                        }
                    });
                });
                Thread.Sleep(4000);
            }
        }

        private async void InitializeProcesses()
        {
            System.Windows.Application.Current.Dispatcher.Invoke(delegate { _showLoaderAction.Invoke(true); });
            await Task.Run(() =>
            {
                Processes = new ObservableCollection<ProcessMod1>(ProcessMod2.Processes.Values);
            });
            _updateThread.Start();
            while (ProcessMod2.Processes.Count == 0)
                Thread.Sleep(3000);
            System.Windows.Application.Current.Dispatcher.Invoke(delegate { _showLoaderAction.Invoke(false); });
        }

        internal void Close()
        {
            _updateThread.Join(3000);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
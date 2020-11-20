using System;
using System.Windows;

namespace Aria2Launcher.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool _isNeedShutdown = true;
        
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            bool isMinState = this.WindowState == WindowState.Minimized;
            _isNeedShutdown = !isMinState;
            
            if (isMinState)
                this.Close();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (_isNeedShutdown)
            {
                Application.Current.Shutdown();
            }
        }
    }
}

using Aria2Launcher.Services;
using Aria2Launcher.ViewModel;

using CommunityToolkit.Mvvm.DependencyInjection;

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

namespace Aria2Launcher.Views
{
    /// <summary>
    /// ChooseFolderWindow.xaml 的交互逻辑
    /// </summary>
    public partial class FolderChooserWindow : Window
    {
        private FolderChooserViewModel _vm;

        public FolderChooserWindow()
        {
            InitializeComponent();
            _vm = Ioc.Default.GetRequiredService<FolderChooserViewModel>();
        }

        protected override void OnClosed(EventArgs e)
        {
            if (_vm.IsNoExe)
                Application.Current.Shutdown();
            else
                new MainWindow().Show();

            base.OnClosed(e);
        }

        private void Close_Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}

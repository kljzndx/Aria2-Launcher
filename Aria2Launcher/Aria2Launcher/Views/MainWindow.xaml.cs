using Aria2Launcher.ViewModels;

using CommunityToolkit.Mvvm.DependencyInjection;

using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;

using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;

using WinRT.Interop;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Aria2Launcher.Views
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        private AppWindow _;
        private MainViewModel _vm;

        public MainWindow()
        {
            this.InitializeComponent();
            _ = AppWindow.GetFromWindowId(Win32Interop.GetWindowIdFromWindow(WindowNative.GetWindowHandle(this)));
            _vm = Ioc.Default.GetRequiredService<MainViewModel>();

            _.Destroying += __Destroying;
            _vm.Aria2.Outputed += Aria2_Outputed;
        }

        private void Aria2_Outputed(object? sender, string e)
        {
            this.DispatcherQueue?.TryEnqueue(() => _vm.AddLog(e));
        }

        private void __Destroying(AppWindow sender, object args)
        {
            _vm.Aria2.Outputed -= Aria2_Outputed;
        }
    }
}

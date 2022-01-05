using CommunityToolkit.Mvvm.DependencyInjection;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aria2Launcher.ViewModels
{
    public class ViewModelLocator
    {
        public MainViewModel Main => Ioc.Default.GetRequiredService<MainViewModel>();
    }
}

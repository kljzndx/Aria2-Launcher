using CommunityToolkit.Mvvm.DependencyInjection;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aria2Launcher.ViewModel
{
    public class ViewModelLocator
    {
        public MainViewModel Main => Ioc.Default.GetRequiredService<MainViewModel>();
        public AppConfigViewModel AppConf => Ioc.Default.GetRequiredService<AppConfigViewModel>();
        public Aria2ConfigureViewModel Aria2Conf => Ioc.Default.GetRequiredService<Aria2ConfigureViewModel>();
        public TaskBarViewModel TaskBar => Ioc.Default.GetRequiredService<TaskBarViewModel>();
        public FolderChooserViewModel FolderChooser => Ioc.Default.GetRequiredService<FolderChooserViewModel>();

    }
}

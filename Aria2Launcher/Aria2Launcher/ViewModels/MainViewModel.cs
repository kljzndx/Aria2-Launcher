using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aria2Launcher.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        public ObservableCollection<string> Logs { get; } = new ObservableCollection<string>();
    }
}

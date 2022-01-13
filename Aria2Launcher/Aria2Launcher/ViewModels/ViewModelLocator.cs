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
        private Dictionary<Type, IViewModel> Cache = new ();

        public MainViewModel Main => GetViewModel<MainViewModel>();

        private T GetViewModel<T>() where T : class, IViewModel, new()
        {
            if (Cache.ContainsKey(typeof(T)))
                return (T) Cache[typeof(T)];

            T vm = new T();
            Cache.Add(typeof(T), vm);
            return vm;
        }
    }
}

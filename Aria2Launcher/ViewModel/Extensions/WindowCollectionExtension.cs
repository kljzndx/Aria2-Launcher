using Aria2Launcher.Views;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Aria2Launcher.ViewModel.Extensions
{
    public static class WindowCollectionExtension
    {
        public static bool ContainsWindow<TWindow>(this WindowCollection collection)
        {
            foreach (Window item in collection)
                if (item is TWindow)
                    return true;

            return false;
        }
    }
}

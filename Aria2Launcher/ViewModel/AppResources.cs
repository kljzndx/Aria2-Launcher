using Aria2Launcher.Resources;

using System;
using System.Collections.Generic;
using System.Text;

namespace Aria2Launcher.ViewModel
{
    public class AppResources
    {
        public AppResources()
        {
            Strings = new StringResource();
        }

        public StringResource Strings { get; }
    }
}

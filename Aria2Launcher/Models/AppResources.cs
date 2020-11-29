using Aria2Launcher.Resources;

using System;
using System.Collections.Generic;
using System.Text;

namespace Aria2Launcher.Models
{
    public class AppResources : ObservableObject
    {
        public static AppResources Current;

        public AppResources()
        {
            Current = this;
            Strings = new StringResource();
        }

        public StringResource Strings { get; set; }
    }
}

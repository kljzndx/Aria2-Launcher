using System.Collections.Generic;

namespace Aria2Launcher.Models.SettingModels
{
    public class SettingGroup
    {
        public SettingGroup(string name, List<SettingItem> items)
        {
            Name = name;
            Items = items;
        }

        public string Name { get; }
        public List<SettingItem> Items { get; }
    }
}
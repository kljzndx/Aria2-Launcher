using System.Collections.Generic;

using CommunityToolkit.Mvvm.ComponentModel;

using Newtonsoft.Json;

namespace Aria2Launcher.Models.SettingModels
{
    public class SettingItem : ObservableObject
    {
        private string _value;

        public SettingItem(string key, string value)
        {
            Key = key;
            _value = value;
        }

        [JsonConstructor]
        public SettingItem(string name, string description, string key, string defaultValue, List<string> optionList)
        {
            Name = name;
            Description = description;
            Key = key;
            _value = defaultValue;
            DefaultValue = defaultValue;
            OptionList = optionList;
        }

        public string Name { get; }
        public string Description { get; }
        public string Key { get; }

        public string Value
        {
            get => _value;
            set => SetProperty(ref _value, value.Trim());
        }

        public string DefaultValue { get; }
        public List<string> OptionList { get; }
    }
}
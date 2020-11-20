using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Aria2Launcher.Models.SettingModels;
using Newtonsoft.Json;

namespace Aria2Launcher.Services
{
    public class Aria2ConfService
    {
        private string _filePath;

        public Aria2ConfService(string docJson)
        {
            SettingGroupList = JsonConvert.DeserializeObject<List<SettingGroup>>(docJson);
        }

        public List<SettingGroup> SettingGroupList { get; }

        public void Load(string filePath)
        {
            var options = new Dictionary<string, string>();
            string[] lines = File.ReadAllLines(filePath);

            _filePath = filePath;

            foreach (var line in lines)
            {
                string[] option = line.Split('=');
                options.Add(option[0].Trim(), option[1].Trim());
            }

            // TODO 各种分类配置加载

            foreach (var settingGroup in SettingGroupList.Select(g => g.Items))
            {
                foreach (var setting in settingGroup.Where(s => options.ContainsKey(s.Key)))
                {
                    setting.Value = options[setting.Key];
                    options.Remove(setting.Key);
                }
            }

            // 创建“其他”分组
            if (options.Any())
            {
                var otherItems = new List<SettingItem>();

                foreach (var option in options)
                    otherItems.Add(new SettingItem(option.Key, option.Value));

                SettingGroupList.Add(new SettingGroup("other", otherItems));
            }
        }

        public void Save()
        {
            var options = new Dictionary<string, string>();

            foreach (var settingGroup in SettingGroupList.Select(g => g.Items))
            {
                foreach (var setting in settingGroup.Where(s => s.Value != s.DefaultValue))
                    options.Add(setting.Key, setting.Value);
            }

            StringBuilder builder = new StringBuilder();
            foreach (var option in options)
                builder.AppendLine(string.Concat(option.Key, "=", option.Value));

            File.WriteAllText(_filePath, builder.ToString());
        }
    }
}
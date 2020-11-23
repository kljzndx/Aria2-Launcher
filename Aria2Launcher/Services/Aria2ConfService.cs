using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Aria2Launcher.Models.SettingModels;
using Newtonsoft.Json;

namespace Aria2Launcher.Services
{
    public class Aria2ConfService
    {
        private string _filePath;
        private readonly HttpClient _http = new HttpClient();

        public Aria2ConfService(string docJson)
        {
            SettingGroupList = new ObservableCollection<SettingGroup>(JsonConvert.DeserializeObject<List<SettingGroup>>(docJson));
        }

        public ObservableCollection<SettingGroup> SettingGroupList { get; }

        public async Task UpdateTracker(string trackerSource)
        {
            var mes = await _http.GetAsync(trackerSource);
            if (!mes.IsSuccessStatusCode)
            {
                MessageBox.Show("地址请求失败");
                return;
            }

            string content = await mes.Content.ReadAsStringAsync();
            var list = content.Split('\n').Where(s => !string.IsNullOrWhiteSpace(s)).Select(s => s.Trim()).ToList();
            string tracker = string.Join(',', list);
            SettingGroupList[4].Items[0].Value = tracker;
            MessageBox.Show("完成更新");
        }
        
        public void Load(string filePath)
        {
            var options = new Dictionary<string, string>();
            string[] lines = File.ReadAllLines(filePath);

            _filePath = filePath;

            foreach (var line in lines)
            {
                string str = line.Trim();
                if (str.StartsWith('#'))
                    continue;

                List<string> option = str.Split('=').Select(s => s.Trim()).ToList();

                if (option.Count < 2)
                    continue;

                string key = option.First();
                option.RemoveAt(0);
                string value = string.Join('=', option);
                
                options.Add(key, value);
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

        public void Save() => Save(_filePath);
        
        public void Save(string filePath)
        {
            StringBuilder builder = new StringBuilder();

            foreach (var settingGroup in SettingGroupList.Select(g => g.Items))
            {
                foreach (var setting in settingGroup.Where(s => s.Value != s.DefaultValue))
                    builder.AppendLine(string.Concat(setting.Key, "=", setting.Value));
            }

            File.WriteAllText(filePath, builder.ToString());
        }
    }
}
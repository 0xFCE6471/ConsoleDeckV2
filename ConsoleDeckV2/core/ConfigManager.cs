using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleDeckV2.core
{
    public class ButtonAction
    {
        public string Type { get; set; } = "None";
        public string Value { get; set; } = "";
    }

    public class SerialSettings
    {
        public string Port { get; set; } = "COM3";
        public int BaudRate { get; set; } = 9600;
    }

    public class AppConfig
    {
        public SerialSettings SerialConfig { get; set; } = new SerialSettings();
        public Dictionary<string, ButtonAction> Buttons { get; set; } = new Dictionary<string, ButtonAction>();
    }

    public class ConfigManager
    {
        private static readonly string ConfigFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json");

        public static AppConfig LoadConfig()
        {
            if (File.Exists(ConfigFile))
            {
                try
                {
                    string json = File.ReadAllText(ConfigFile);
                    return JsonConvert.DeserializeObject<AppConfig>(json);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("[ERROR] Failed to read config: " + ex.Message);
                }
            }

            var defaultConfig = CreateDefaultConfig();
            SaveConfig(defaultConfig);
            return defaultConfig;
        }

        public static void SaveConfig(AppConfig config)
        {
            try
            {
                string json = JsonConvert.SerializeObject(config, Formatting.Indented);
                File.WriteAllText(ConfigFile, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine("[ERROR] Failed to save config: " + ex.Message);
            }
        }

        private static AppConfig CreateDefaultConfig()
        {
            var appConfig = new AppConfig
            {
                SerialConfig = new SerialSettings { Port = "COM3", BaudRate = 9600 },
                Buttons = new Dictionary<string, ButtonAction>()
            };

            for (int i = 1; i <= 9; i++)
            {
                appConfig.Buttons[$"BUTTON_{i}"] = new ButtonAction
                {
                    Type = i == 1 ? "link" : "none",
                    Value = i == 1 ? "https://makerworld.com/en/models/1717141-console-deck-v2#profileId-1822667" : ""
                };
            }

            return appConfig;
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Vista.Controller.ScreenMaster3.SpyderTranslator
{
    public class ButtonTranslationMap
    {
        [JsonPropertyName("keyMappings")]
        public Dictionary<int, KeyConfig> KeyMappings { get; set; } = new Dictionary<int, KeyConfig>();

        [JsonPropertyName("customSegmentButtons")]
        public Dictionary<string, List<SegmentConfig>> CustomSegmentButtons { get; set; } = new Dictionary<string, List<SegmentConfig>>();

        [JsonPropertyName("startupSegmentConfig")]
        public List<SegmentConfig> StartupSegmentConfigs { get; set; } = new List<SegmentConfig>();

        public static async Task<ButtonTranslationMap> LoadAsync(string fileName)
        {
            if (!File.Exists(fileName))
                throw new FileNotFoundException("Specified file was not found", fileName);

            using Stream stream = File.OpenRead(fileName);
            var response = await LoadAsync(stream);
            return response;
        }

        public static async Task<ButtonTranslationMap> LoadAsync(Stream stream)
        {
            JsonSerializerOptions jsonOptions = new JsonSerializerOptions()
            {
                WriteIndented = true,
                Converters =
                {
                    new JsonStringEnumConverter(JsonNamingPolicy.CamelCase, false)
                }
            };
            var response = await JsonSerializer.DeserializeAsync<ButtonTranslationMap>(stream, jsonOptions);
            return response;
        }

        /// <summary>
        /// Loads the embedded ScreenMaster III 3216 key map
        /// </summary>
        /// <returns></returns>
        public static Task<ButtonTranslationMap> LoadScreenMaster3216Map()
        {
            var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Vista.Controller.ScreenMaster3.SpyderTranslator.SM3-3216.json");
            return LoadAsync(stream);
        }

        public KeyConfig GetConfigForButton(int keyIndex)
        {
            if (KeyMappings.ContainsKey(keyIndex))
                return KeyMappings[keyIndex];

            return null;
        }

        public List<int> GetKeysInGroup(string group, bool ignoreCase = true)
        {
            return KeyMappings
                .Where(key => string.Compare(key.Value.CustomGroup, group, ignoreCase) == 0)
                .Select(key => key.Key)
                .ToList();
        }

        public List<SegmentConfig> GetSegmentConfigsForCustomKey(string customKey)
        {
            if (CustomSegmentButtons.ContainsKey(customKey))
                return CustomSegmentButtons[customKey];

            return null;
        }

        public Dictionary<int, KeyConfig> GetKeysFromSegmentID(int segmentID)
        {
            return KeyMappings
                .Where(key => key.Value.SegmentID == segmentID)
                .ToDictionary(key => key.Key, key => key.Value);                
        }
    }

    public class KeyConfig
    {
        [JsonPropertyName("customKey")]
        public string CustomKey { get; set; }

        [JsonPropertyName("group")]
        public string CustomGroup { get; set; }

        [JsonPropertyName("segmentId")]
        public int SegmentID { get; set; }

        [JsonPropertyName("buttonIndex")]
        public int SpyderButtonIndex { get; set; }

        [JsonPropertyName("pressAtStartup")]
        public bool PressAtStartup { get; set; }

        public static bool IsQuickKey(int keyIndex)
        {
            return keyIndex >= 288 && keyIndex <= 335;
        }

        public static int GetQuickKeyIndex(int keyIndex)
        {
            if (!IsQuickKey(keyIndex))
                return -1;

            return keyIndex - 288;
        }
    }

    /// <summary>
    /// Spyder 2x8 segment type.  Integer index correlates to key index when a 2x8 is in config mode
    /// </summary>
    public enum SegmentType
    {
        Source = 0,
        Treatment = 1,
        CommandKey = 2,
        FunctionKey = 3,
        Transition = 4,
        Device = 5,
        DualLayer = 6,
        Layer = 7
    }

    public class SegmentConfig
    {
        [JsonPropertyName("segmentId")]
        public int SegmentID { get; set; }

        [JsonPropertyName("segmentType")]
        public SegmentType SegmentType { get; set; }

        [JsonPropertyName("index")]
        public int Index { get; set; }
    }
}

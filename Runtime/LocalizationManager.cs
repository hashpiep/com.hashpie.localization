using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
namespace Hashpie.Localization
{
    public class LocalizationManager : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField]
        private string fallbackLocalName = "en_us";
        [SerializeField]
        private string assetsSubpath = "Localizations";
        private static Dictionary<string, string> selectedLocal;
        private static Dictionary<string, string> fallbackLocal;
        private static LocalizationManager instance;
        public static LocalizationManager Instance { get { return instance; } }
        public string AssetsSubpath { get { return assetsSubpath; } }
        public event Action<Dictionary<string, string>> OnLocalChanged;
        private void Awake()
        {
            if (instance != null)
                Destroy(gameObject);

            instance = this;

            Initialize();
        }
        public string GetKeyValue(string keyName)
        {
            if (selectedLocal == null)
                return GetFallbackKeyValue(keyName);

            if (!selectedLocal.ContainsKey(keyName))
                return GetFallbackKeyValue(keyName);

            return selectedLocal[keyName];
        }
        private string GetFallbackKeyValue(string keyName)
        {
            if (fallbackLocal == null)
            {
                Debug.LogWarning($"Fallback Local not set!");
                return "";
            }

            if (fallbackLocal == null)
            {
                Debug.LogWarning($"Key {keyName} is missing in fallback local!");
                return "";
            }

            return fallbackLocal[keyName];
        }
        private void Initialize()
        {
            fallbackLocal = GetLocal(fallbackLocalName);
        }
        private Dictionary<string, string> GetLocal(string localName)
        {
            string streamingAssetsPath = Application.streamingAssetsPath;

            if (!Directory.Exists(streamingAssetsPath))
                Directory.CreateDirectory(streamingAssetsPath);

            string path = Path.Combine(streamingAssetsPath, AssetsSubpath);

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            path = Path.Combine(path, localName + ".json");

            if (!File.Exists(path))
            {
                Debug.LogError($"File {path} does not exist.");
                return new Dictionary<string, string>();
            }

            string json = File.ReadAllText(path);
            Dictionary<string, string> local = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            return local;
        }
        public void ChangeLocal(string localName)
        {
            selectedLocal = GetLocal(localName);
            OnLocalChanged?.Invoke(selectedLocal);
        }
    }
}
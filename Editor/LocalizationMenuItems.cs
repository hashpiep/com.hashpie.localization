using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
namespace FrogBaseball.Localization.Editor
{
    public class LocalizationMenuItems
    {
        [MenuItem("Assets/Create/Frog Baseball Localization/Create Local")]
        private static void CreateLocal()
        {
            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            string fileName = "xx_yy.json";
            string fullPath = Path.Combine(path, fileName);
            fullPath = AssetDatabase.GenerateUniqueAssetPath(fullPath);
            Dictionary<string, string> defaultLocal = new Dictionary<string, string>
        {
            { "example", "This is an example!" }
        };
            string json = JsonConvert.SerializeObject(defaultLocal, Formatting.Indented);
            File.WriteAllText(fullPath, json);
            AssetDatabase.Refresh();
        }
        [MenuItem("GameObject/Frog Baseball/Localization/LocalizationManager")]
        private static void CreateLocalizationManager(MenuCommand menuCommand)
        {
            GameObject obj = new GameObject("LocalizationManager");
            obj.AddComponent<LocalizationManager>();
            GameObjectUtility.SetParentAndAlign(obj, menuCommand.context as GameObject);
        }
    }
}
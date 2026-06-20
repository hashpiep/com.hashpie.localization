using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
namespace FrogBaseball.Localization.Editor
{
    public class LocalEditorWindow : EditorWindow
    {
        private Dictionary<string, string> currentLocal;
        private string currentFilePath;
        [MenuItem("Window/Frog Baseball/Localization Editor")]
        public static void ShowWindow()
        {
            LocalEditorWindow wnd = GetWindow<LocalEditorWindow>();
            wnd.titleContent = new GUIContent("Localization Editor");
        }
        public void CreateGUI()
        {
            VisualElement root = rootVisualElement;

            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
            "Packages/com.frogbaseball.localization/Editor/LocalEditorWindow.uxml"
            );
            VisualElement rootUXML = visualTree.Instantiate();

            rootUXML.Q<Button>("CreateBtn").clicked += () => OnCreateBtnPressed(root);
            rootUXML.Q<Button>("LoadBtn").clicked += () => OnLoadBtnPressed(root);
            rootUXML.Q<Button>("AddMissingKeysBtn").clicked += () => OnAddMissingKeysBtnPressed(root);
            rootUXML.Q<Button>("AddKeyBtn").clicked += () => OnAddKeyBtnPress(root, currentLocal);
            rootUXML.Q<Button>("SaveBtn").clicked += () => OnSaveBtnPressed(root, currentLocal, currentFilePath);

            root.Add(rootUXML);
        }
        private void LoadAllKeysAndValues(VisualElement root, Dictionary<string, string> local)
        {
            ScrollView scrView = root.Q<ScrollView>("KeysAndValuesScrView");

            scrView.Clear();

            foreach (string key in local.Keys)
            {
                VisualElement horContainer = new VisualElement();
                horContainer.style.flexDirection = FlexDirection.Row;

                TextField textField = new TextField();
                textField.label = key;
                textField.value = local[key];
                textField.style.flexGrow = 1;

                void TextFieldChanged(ChangeEvent<string> evt)
                {
                    local[key] = evt.newValue;
                }

                textField.RegisterValueChangedCallback(TextFieldChanged);

                void OnRemoveBtnPressed()
                {
                    local.Remove(key);
                    LoadAllKeysAndValues(root, local);
                }

                Button removeBtn = new Button();
                removeBtn.text = "X";
                removeBtn.clicked += OnRemoveBtnPressed;

                horContainer.Add(textField);
                horContainer.Add(removeBtn);
                scrView.Add(horContainer);
            }
        }
        private void OnAddKeyBtnPress(VisualElement root, Dictionary<string, string> local)
        {
            if (local == null)
            {
                ShowPopup(root, "WARNING: Create or load a local first!");
                return;
            }

            string keyName = root.Q<TextField>("AddKeyTextField").value;

            if (keyName == null || keyName == "")
                return;

            if (local.ContainsKey(keyName))
            {
                ShowPopup(root, $"WARNING: {keyName} already exists!");
                return;
            }

            local.Add(keyName, "");

            LoadAllKeysAndValues(root, local);
        }
        private void ShowPopup(VisualElement root, string message)
        {
            root.Q<Label>("InfoLabel").text = message;
        }
        private void OnSaveBtnPressed(VisualElement root, Dictionary<string, string> dictToSave, string filePath)
        {
            try
            {
                string json = JsonConvert.SerializeObject(dictToSave);
                File.WriteAllText(filePath, json);
                ShowPopup(root, $"INFO: Local {filePath} saved successfully!");
            }
            catch (Exception e)
            {
                ShowPopup(root, $"ERROR: There was an error saving {filePath}. {e.Message}");
            }
        }
        private void OnLoadBtnPressed(VisualElement root)
        {
            string path = EditorUtility.OpenFilePanel("Open JSON", Application.streamingAssetsPath, "json");

            if (path == null || path == "")
                return;

            Dictionary<string, string> local = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(path));

            if (local == null)
                return;

            currentFilePath = path;
            currentLocal = local;
            LoadAllKeysAndValues(root, local);
        }
        private void OnAddMissingKeysBtnPressed(VisualElement root)
        {
            if (currentLocal == null)
                return;

            string path = EditorUtility.OpenFilePanel("Open JSON", Application.streamingAssetsPath, "json");

            if (path == null || path == "")
                return;

            Dictionary<string, string> local = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(path));

            if (local == null)
                return;

            foreach (string key in local.Keys)
            {
                if (!currentLocal.ContainsKey(key))
                    currentLocal.Add(key, local[key]);
            }

            LoadAllKeysAndValues(root, currentLocal);
        }
        private void OnCreateBtnPressed(VisualElement root)
        {
            if (!Directory.Exists(Application.streamingAssetsPath))
                Directory.CreateDirectory(Application.streamingAssetsPath);

            if (LocalizationManager.Instance != null)
                if (!Directory.Exists(Path.Combine(Application.streamingAssetsPath, LocalizationManager.Instance.AssetsSubpath)))
                    Directory.CreateDirectory(Path.Combine(Application.streamingAssetsPath, LocalizationManager.Instance.AssetsSubpath));

            Dictionary<string, string> local = new Dictionary<string, string>();
            local.Add("example", "This is an example!");
            string json = JsonConvert.SerializeObject(local);
            string path = EditorUtility.SaveFilePanel("Create Localization", Application.streamingAssetsPath, "xx_yy", "json");

            if (path == "" || path == null)
                return;

            File.WriteAllText(path, json);
            currentFilePath = path;
            currentLocal = local;
            LoadAllKeysAndValues(root, local);
        }
    }
}
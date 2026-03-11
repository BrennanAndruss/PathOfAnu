using UnityEngine;
using UnityEditor;

namespace Project.Spells.Scripts.Editor
{
    [CustomEditor(typeof(WandManager))]
    public class WandManagerEditor : UnityEditor.Editor
    {
        private string _newTemplateName = "NewSpell";

        public override void OnInspectorGUI()
        {
            // Draw the standard inspector fields
            DrawDefaultInspector();

            WandManager manager = (WandManager)target;
            
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Gesture Recording Tools", EditorStyles.boldLabel);

            // Check if we have template data to act upon
            bool hasData = manager.CapturedProjectedPoints != null && manager.CapturedProjectedPoints.Length > 0;
            
            _newTemplateName = EditorGUILayout.TextField("Template Name", _newTemplateName);

            // Disable the button if there's no data buffered
            GUI.enabled = hasData;
            if (GUILayout.Button("Save Captured Gesture as Template"))
            {
                SaveTemplate(manager);
            }

            GUI.backgroundColor = hasData ? Color.red : Color.white;
            if (GUILayout.Button("Discard Captured Gesture"))
            {
                // Clear the spell after discarding
                manager.ClearCapturedGesture();
                Debug.Log("Gesture cleared");
            }

            GUI.backgroundColor = Color.white;
            GUI.enabled = true;
        }

        private void SaveTemplate(WandManager manager)
        {
            GestureTemplate asset = ScriptableObject.CreateInstance<GestureTemplate>();
            asset.gestureName = _newTemplateName;
            asset.Points = manager.CapturedProjectedPoints;

            string directory = "Assets/Project/Spells/ScriptableObjects/Templates";
            string path = $"{directory}/{_newTemplateName}.asset";
        
            // Ensure the directory exists
            System.IO.Directory.CreateDirectory(directory);

            AssetDatabase.CreateAsset(asset, path);
            AssetDatabase.SaveAssets();
        
            Debug.Log($"Successfully saved {_newTemplateName} to {path}");
        
            // Clear the spell after saving
            manager.ClearCapturedGesture();
        }
    }
}
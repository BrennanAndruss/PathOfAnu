using UnityEditor;
using UnityEngine;
using _Project.Features.Spells.ScriptableObjects;

namespace _Project.Features.Spells.Scripts.Editor
{
    [CustomEditor(typeof(WandManager))]
    public class WandManagerEditor : UnityEditor.Editor
    {
        private SpellType _selectedType;

        public override void OnInspectorGUI()
        {
            // Draw the standard inspector fields
            DrawDefaultInspector();

            WandManager manager = (WandManager)target;
            
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Gesture Recording Tools", EditorStyles.boldLabel);

            // Check if we have template data to act upon
            bool hasData = manager.capturedProjectedPoints != null && manager.capturedProjectedPoints.Length > 0;
            
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Spell Type", GUILayout.Width(100));
            _selectedType = (SpellType)EditorGUILayout.EnumPopup(_selectedType);
            
            EditorGUILayout.EndHorizontal();

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
            GestureData asset = ScriptableObject.CreateInstance<GestureData>();
            // asset.spellType = _selectedType;
            asset.points = manager.capturedProjectedPoints;
            asset.strokeCount = asset.points[^1].StrokeId;

            string directory = "Assets/Project/Spells/ScriptableObjects/Templates";
            string path = $"{directory}/{_selectedType}Gesture.asset";
        
            // Ensure the directory exists
            System.IO.Directory.CreateDirectory(directory);

            AssetDatabase.CreateAsset(asset, path);
            AssetDatabase.SaveAssets();
        
            Debug.Log($"Successfully saved {_selectedType}Gesture to {path}");
        
            // Clear the spell after saving
            manager.ClearCapturedGesture();
        }
    }
}
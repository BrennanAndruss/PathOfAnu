using System;
using UnityEngine;

namespace Project.Spells.Scripts
{
    public class SpellRecognizer : MonoBehaviour
    {
        [SerializeField] private SpellSettings spellSettings;
        [SerializeField] private GestureTemplate[] spellLibrary;
        private Gesture[] _templates;
        
        public Action<SpellType> OnSpellRecognized;

        private void Awake()
        {
            // Preprocesses templates at startup
            _templates = new Gesture[spellLibrary.Length];
            for (int i = 0; i < spellLibrary.Length; i++)
            {
                _templates[i] = new Gesture(spellLibrary[i].Points, spellLibrary[i].gestureName, spellSettings);
            }
        }

        public void RecognizeSpell(GesturePoint[] spellPoints)
        {
            // Create a gesture from the user's drawing
            var candidate = new Gesture(spellPoints, "UserDrawing", spellSettings);
            Debug.Log("[SpellRecognizer] " + spellPoints.Length + " gesture points");
            Debug.Log("[SpellRecognizer] " + candidate.Points.Length + " processed points");
            
            // Use the $Q Recognizer
            RecognitionResult result = QRecognizer.Classify(candidate, _templates, spellSettings);
            Debug.Log("[SpellRecognizer] Result: " + result.Name);
            
            SpellType spellType = SpellType.Prototype;
            OnSpellRecognized?.Invoke(spellType);
        }
    }
}
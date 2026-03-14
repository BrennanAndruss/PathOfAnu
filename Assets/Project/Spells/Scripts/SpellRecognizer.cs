using System;
using System.Collections.Generic;
using UnityEngine;

namespace Project.Spells.Scripts
{
    public class SpellRecognizer : MonoBehaviour
    {
        [SerializeField] private SpellSettings spellSettings;
        [SerializeField] private GestureTemplate[] spellLibrary;
        private Dictionary<int, List<Gesture>> _gestureBins = new();
        private Dictionary<int, Gesture[]> _gestures = new();
        
        public Action<SpellType> OnSpellRecognized;

        private void Awake()
        {
            // Preprocess gestures at startup
            foreach (var template in spellLibrary)
            {
                var gesture = new Gesture(template, spellSettings);

                // Bin gestures by numStrokes
                if (!_gestureBins.ContainsKey(gesture.StrokeCount))
                {
                    _gestureBins[gesture.StrokeCount] = new List<Gesture>();
                }
                _gestureBins[gesture.StrokeCount].Add(gesture);
            }
            
            // Convert gesture lists to arrays
            foreach (var bin in _gestureBins)
            {
                _gestures[bin.Key] = bin.Value.ToArray();
            }
        }

        public void RecognizeSpell(GesturePoint[] spellPoints)
        {
            // Create a gesture from the user's drawing
            var candidate = new Gesture(spellPoints, "UserDrawing", spellSettings);
            Debug.Log("[SpellRecognizer] " + spellPoints.Length + " gesture points");
            Debug.Log("[SpellRecognizer] " + candidate.Points.Length + " processed points");
            
            // Use the $Q Recognizer
            RecognitionResult result = QRecognizer.Classify(candidate, _gestures[candidate.StrokeCount], spellSettings);
            Debug.Log("[SpellRecognizer] Result: " + result.Name + " " + result.SpellType + " " + result.Confidence);
            
            OnSpellRecognized?.Invoke(result.SpellType);
        }
    }
}
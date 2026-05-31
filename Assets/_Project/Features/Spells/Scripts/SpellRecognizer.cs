using System;
using System.Collections.Generic;
using _Project.Features.Spells.ScriptableObjects;
using UnityEngine;

namespace _Project.Features.Spells.Scripts
{
    public class SpellRecognizer : MonoBehaviour
    {
        [SerializeField] private SpellSettings spellSettings;
        private Dictionary<int, List<Gesture>> _gestureBins = new();
        private Dictionary<int, Gesture[]> _gestures = new();
        
        public Action<SpellType> OnSpellRecognized;

        public void SetGestures(SpellData[] spellLibrary)
        {
            // Preprocess gestures at startup
            foreach (var spell in spellLibrary)
            {
                Debug.Log($"[SpellRecognizer] Template: {spell.name} {spell.gestureData.strokeCount}");
                var gesture = new Gesture(spell, spellSettings);

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

            // Check against gestures with the same stroke count
            if (!_gestures.TryGetValue(candidate.StrokeCount, out var gestures))
            {
                OnSpellRecognized?.Invoke(SpellType.Unknown);
                return;
            }
            
            // Use the $Q Recognizer
            RecognitionResult result = QRecognizer.Classify(candidate, gestures, spellSettings);
            Debug.Log("[SpellRecognizer] Result: " + result.SpellType + " " + result.Confidence);
            
            OnSpellRecognized?.Invoke(result.SpellType);
        }
    }
}
using System;
using UnityEngine;

namespace Project.Spells.Scripts
{
    public class SpellRecognizer : MonoBehaviour
    {
        [SerializeField] private SpellSettings spellSettings;
        
        public Action<SpellType> OnSpellRecognized;

        public void RecognizeSpell(GesturePoint[] spellPoints)
        {
            var candidate = new Gesture(spellPoints, "UserDrawing", spellSettings);
            
            Debug.Log("[SpellRecognizer] " + spellPoints.Length + " gesture points");
            Debug.Log("[SpellRecognizer] " + candidate.Points.Length + " processed points");
            
            SpellType spellType = SpellType.Prototype;
            OnSpellRecognized?.Invoke(spellType);
        }
    }
}
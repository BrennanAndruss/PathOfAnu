using System;
using System.Collections.Generic;
using UnityEngine;

namespace Project.Spells.Scripts
{
    public class SpellRecognizer : MonoBehaviour
    {
        public Action<SpellType> OnSpellRecognized;

        public void RecognizeSpell(List<SpellPoint> spellPoints)
        {
            Debug.Log("[SpellRecognizer] " + spellPoints.Count + " spell points");
            // Receive _activeSpellPoints
            SpellType spellType = SpellType.Prototype;
            OnSpellRecognized?.Invoke(spellType);
        }
    }
}
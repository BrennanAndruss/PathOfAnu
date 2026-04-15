using UnityEngine;
using _Project.Features.Spells.Scripts;

namespace _Project.Features.Spells.ScriptableObjects
{
    [CreateAssetMenu(fileName = "SpellData", menuName = "Spells/SpellData")]
    public class SpellData : ScriptableObject
    {
        public SpellType spellType;
        public GestureData gestureData;
        public GameObject spellPrefab;
    }
}
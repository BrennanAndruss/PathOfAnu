using UnityEngine;
using UnityEngine.Serialization;

namespace Project.Spells.Scripts
{
    [CreateAssetMenu(fileName = "GestureTemplate", menuName = "Spells/Template")]
    public class GestureTemplate : ScriptableObject
    {
        public string gestureName;
        public SpellType spellType;
        public int strokeCount;
        public GesturePoint[] points;
    }
}

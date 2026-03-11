using UnityEngine;

namespace Project.Spells.Scripts
{
    [CreateAssetMenu(fileName = "GestureTemplate", menuName = "Spells/Template")]
    public class GestureTemplate : ScriptableObject
    {
        public string gestureName;
        public GesturePoint[] Points;
    }
}

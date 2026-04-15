using UnityEngine;
using _Project.Features.Spells.Scripts;

namespace _Project.Features.Spells.ScriptableObjects
{
    [CreateAssetMenu(fileName = "GestureData", menuName = "Spells/Template")]
    public class GestureData : ScriptableObject
    {
        public int strokeCount;
        public GesturePoint[] points;
    }
}

using UnityEngine;

namespace Project.Spells.Scripts
{
    [CreateAssetMenu(fileName = "SpellSettings", menuName = "Spells/SpellSettings")]
    public class SpellSettings : ScriptableObject
    {
        [Header("$Q-Algorithm Parameters")] 
        public int sampleResolution = 64;
        public int maxIntCoordinates = 1024;
        public int lutSize = 64;
        public float InvLutScaleFactor => 1.0f / (maxIntCoordinates / lutSize);

        [Header("Recognition Parameters")] 
        [Range(0f, 1f)]
        public float matchThreshold = 0.8f;
    }
}
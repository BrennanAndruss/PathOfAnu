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

        [Space(10f)] 
        public bool useEarlyAbandoning = false; // Faster computation, less reliable drawing accuracy
        public bool useLowerBounding = true;

        [Header("Recognition Parameters")] 
        [Range(0f, 1f)]
        public float matchThreshold = 0.2f;
    }
}
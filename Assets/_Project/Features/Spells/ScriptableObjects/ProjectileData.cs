using UnityEngine;
using _Project.Features.Spells.Scripts;

namespace _Project.Features.Spells.ScriptableObjects
{
    [CreateAssetMenu(fileName = "ProjectileData", menuName = "Spells/ProjectileData")]
    public class ProjectileData : ScriptableObject
    {
        public SpellType spellType;
        public GameObject prefab;
        public float launchForce = 15f;
        public AudioClip spellSfx;
        public AudioSource audioSource;
    }
}
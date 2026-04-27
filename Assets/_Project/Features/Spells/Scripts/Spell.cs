using UnityEngine;

namespace _Project.Features.Spells.Scripts
{
    public abstract class Spell : MonoBehaviour
    {
        [SerializeField] private SpellType spellType;

        [Header("Spell SFX")]
        [SerializeField] private AudioClip activeSound;
        [SerializeField] private AudioClip destroySound;

        protected Rigidbody Rb;
        protected AudioSource AudioSource;

        protected bool IsCasting = false;
        
        protected virtual void Awake()
        {
            Debug.Log("[SpellProjectile] " + gameObject.name + " created.");
            
            Rb = GetComponent<Rigidbody>();
            AudioSource = GetComponent<AudioSource>();
            if (AudioSource)
            {
                AudioSource.playOnAwake = false;
            }
        }

        // Child scripts define casting behavior
        public abstract void Cast();

        public SpellType GetSpellType()
        {
            return spellType;
        }
    }
}
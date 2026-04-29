using _Project.Features.Spells.Scripts;
using UnityEngine;

namespace _Project.Code.Scripts.Interactables
{
    public abstract class Interactable : MonoBehaviour
    {
        [Header("Interaction Settings")]
        [SerializeField] protected SpellType requiredType = SpellType.Unknown;
        [SerializeField] protected bool triggerOnlyOnce = true;

        protected bool IsActive = false;
        protected bool HasBeenTriggered = false;

        protected virtual void OnTriggerEnter(Collider other)
        {
            if (triggerOnlyOnce && HasBeenTriggered) return;
            
            // Try to get spell component and check SpellType
            if (other.TryGetComponent<SpellProjectile>(out var projectile))
            {
                if (projectile.GetSpellType() == requiredType || requiredType == SpellType.Unknown)
                {
                    IsActive = true;
                    HasBeenTriggered = true;
                    OnInteract(projectile);
                }
            }
        }

        protected virtual void OnTriggerExit(Collider other)
        {
            if (triggerOnlyOnce && HasBeenTriggered) return;

            if (IsActive)
            {
                IsActive = false;
                OnInteractExit();
            }
        }

        // Child scripts define the interaction
        protected abstract void OnInteract(Spell spell);

        protected virtual void OnInteractExit() {}
    }
}
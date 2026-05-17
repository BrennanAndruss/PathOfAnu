using _Project.Features.Spells.Scripts;
using Deform;
using UnityEngine;

namespace _Project.Code.Scripts.Interactables
{
    public class ChimeInteractable : Interactable
    {
        [SerializeField] private SineDeformer sineDeformer;
        [SerializeField] private float activeFactor = 1f;
        [SerializeField] private float inactiveFactor = 0f;
        [SerializeField] private float waveSpeed = 1f;

        private void Update()
        {
            // Create continuous movement
            if (IsActive && !Mathf.Approximately(sineDeformer.Factor, activeFactor))
            {
                // Keep wave on after first valid spell contact
                sineDeformer.Factor = activeFactor;
            }

            if (sineDeformer.Factor > 0)
            {
                // Move the offset property to make the wave "scroll"
                sineDeformer.Offset += Time.deltaTime * waveSpeed;
            }
        }
        
        protected override void OnInteract(Spell spell)
        {
            sineDeformer.Factor = activeFactor;
            // sfx??
        }

        protected override void OnInteractExit()
        {
            if (triggerOnlyOnce) return;

            sineDeformer.Factor = inactiveFactor;
        }
    }
}
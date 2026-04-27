using System.Collections.Generic;
using _Project.Features.Spells.Scripts;
using UnityEngine;

namespace _Project.Code.Scripts.Interactables
{
    public class BowlInteractable : Interactable
    {
        [SerializeField] private List<GameObject> waterVisuals;
        
        protected override void OnInteract(Spell spell)
        {
            foreach (var waterVisual in waterVisuals)
            {
                waterVisual.SetActive(true);
            }
            // sfx??
        }
    }
}
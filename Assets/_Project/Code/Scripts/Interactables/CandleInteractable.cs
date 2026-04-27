using System.Collections.Generic;
using _Project.Features.Spells.Scripts;
using UnityEngine;

namespace _Project.Code.Scripts.Interactables
{
    public class CandleInteractable : Interactable
    {
        [SerializeField] private List<GameObject> flames;
        
        protected override void OnInteract(Spell spell)
        {
            foreach (var flame in flames)
            {
                flame.SetActive(true);
            }
            // sfx??
        }
    }
}
using System;
using _Project.Code.Scripts.Interactables;
using _Project.Features.Spells.Scripts;
using Unity.VisualScripting;
using UnityEngine;

public class GrowingAreaController : Interactable
{
    [SerializeField] private QuestManager questManager;
    [Header("Geometry To Activate")]
    [SerializeField] private GameObject[] growingAreas;
    [SerializeField] public int state = 0; 
    /*
        0 -- Grass Growing Area 
        1 -- Flowers Growing Area
        2 -- Tree Growing Area 
    */

    [Header("VFX")]
    [SerializeField] private GameObject growingSpellVFX;

    [Header("State")]
    [SerializeField] private bool healed = false;

    private void Start()
    {
        // Make sure everything starts hidden
        foreach (GameObject area in growingAreas)
        {
            if (area != null)
                area.SetActive(false);
        }
    }
    protected override void OnInteract(Spell spell)
    {
        // if other is spelltype == virgo
        ActivateGrowthArea(ref state, ref healed);
    }

    private void ActivateGrowthArea(ref int state, ref bool healed)
    {
        // Turn on all geometry growing areas in 3 stages
        if (state == 0)
        {
            growingAreas[state].SetActive(true); 
            state++;    
        } 
        else if (state == 1)
        {
            growingAreas[state].SetActive(true); 
            state++;
        }
        else if (state == 2)
        {
            growingAreas[state].SetActive(true); 
            healed = true;
            
            // growing area done
            if (questManager != null)
            {
                questManager.OnGrowingAreaHealed();
            }
        }
        else
        {
            Debug.Log("Error out of Bounds");
        }
      
        // Spawn VFX
        if (growingSpellVFX != null)
        {
            growingSpellVFX.SetActive(true);
        }
        Debug.Log($"{gameObject.name} activated.");
    }
}
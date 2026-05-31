using _Project.Features.Spells.Scripts;
using UnityEngine;
using UnityEngine.Serialization;

public class QuestManagerQ1 : MonoBehaviour
{
    [SerializeField] private WandManager wandManager;
    
    [Header("Quest State")]
    [SerializeField] private int questpoint = 1;

    [Header("Quest 1 Objects")]
    [SerializeField] private RuineController[] ruines;
    [SerializeField] private GrowingAreaController[] growingAreas;
    [SerializeField] private GameObject virgoUI;
    [SerializeField] private GameObject BeaconVFX;

    [Header("Quest 1 Progress")]
    [SerializeField] private int ruinesActivated = 0;
    [SerializeField] private int growingAreasHealed = 0;

    [Header("Quest 1 Gates")]
    [SerializeField] private bool ruinesComplete = false;
    [SerializeField] private bool growingAreasComplete = false;
    [FormerlySerializedAs("quest1completed")]
    [SerializeField] private bool quest1Completed = false;


    private void Start()
    {
        ruinesActivated = 0;
        growingAreasHealed = 0;

        if (virgoUI != null)
            virgoUI.SetActive(false);

        SetGrowingAreasEnabled(false);

        foreach (RuineController ruine in ruines)
        {
            if (ruine == null) continue;

            ruine.onActivated.AddListener(OnRuineActivated);
        }
    }

    private void OnDestroy()
    {
        foreach (RuineController ruine in ruines)
        {
            if (ruine == null) continue;

            ruine.onActivated.RemoveListener(OnRuineActivated);
        }
    }

    // Main Interface for Ruine's to ping Manager
    public void  OnRuineActivated()
    {
        if (ruinesComplete) return;
        ruinesActivated++;
        if (ruinesActivated >= ruines.Length)
        {
            CompleteRuinesStep();
        }
    }

    private void CompleteRuinesStep()
    {
        ruinesComplete = true;
        // All activation vfx's turned off

        // Play exiting vfx flask
        foreach (RuineController ruine in ruines)
        {
            if (ruine == null) continue;
            ruine.PlayDespawnVFX();
            ruine.DisableActivationVFX();
        }

        // Virgo Spell Playbast UI is now shown in middle of area
        // Waiting on Brennan's Implementation [Using Temp]
        if (virgoUI != null)
            virgoUI.SetActive(true);
        SetGrowingAreasEnabled(true);
        // Player now unlocks Virgo
        wandManager.ActivateSpell(SpellType.Virgo);
        Debug.Log("Quest 1: All ruines activated. Growing areas and virgo spell unlocked.");
    }

    public void OnGrowingAreaHealed()
    {
        if (!ruinesComplete) return;
        if (growingAreasComplete) return;

        growingAreasHealed++;

        if (growingAreasHealed >= growingAreas.Length)
        {
            CompleteGrowingAreasStep();
        }
    }

    private void CompleteGrowingAreasStep()
    {
        growingAreasComplete = true;
        questpoint = 2;
        // Activate Beacon
        BeaconVFX.SetActive(true);

        Debug.Log("Quest 1 complete. Moving to Quest 2.");
    }

    private void SetGrowingAreasEnabled(bool enabled)
    {
        foreach (GrowingAreaController area in growingAreas)
        {
            if (area == null) continue;

            Collider col = area.GetComponent<Collider>();
            if (col != null)
                col.enabled = enabled;
        }
    }
}
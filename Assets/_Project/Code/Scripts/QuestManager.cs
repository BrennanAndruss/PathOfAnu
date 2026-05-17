using _Project.Features.Spells.Scripts;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    [SerializeField] private WandManager wandManager;
    
    [Header("Quest State")]
    [SerializeField] private int questpoint = 1;

    [Header("Quest 1 Objects")]
    [SerializeField] private RuineController[] ruines;
    [SerializeField] private GrowingAreaController[] growingAreas;
    [SerializeField] private GameObject virgoUI;

    [Header("Quest 1 Progress")]
    [SerializeField] private int ruinesActivated = 0;
    [SerializeField] private int growingAreasHealed = 0;

    [Header("Quest 1 Gates")]
    [SerializeField] private bool ruinesComplete = false;
    [SerializeField] private bool growingAreasComplete = false;
    [SerializeField] private bool quest1completed = false;


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

        if (virgoUI != null)
            virgoUI.SetActive(true);

        SetGrowingAreasEnabled(true);

        Debug.Log("Quest 1: All ruines activated. Growing areas unlocked.");
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

        Debug.Log("Quest 1 complete. Moving to Quest 2.");
        wandManager.ActivateSpell(SpellType.Virgo);
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
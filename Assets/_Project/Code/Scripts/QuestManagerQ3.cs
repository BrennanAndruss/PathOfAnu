using _Project.Features.Spells.Scripts;
using UnityEngine;

public class QuestManagerQ3 : MonoBehaviour
{
    [Header("Quest 3 Objects")]
    [SerializeField] private WaterBowlController[] waterBowls;
    [SerializeField] private WindChimeController[] windChimes;
    [SerializeField] private DragonController[] dragonShrines;
    [SerializeField] private AirChamberController[] airChambers;

    [Header("Spell Unlock UI")]
    [SerializeField] private WandManager wandManager;
    [SerializeField] private GameObject aquariusUI;
    [SerializeField] private GameObject piscesUI;

    [Header("Optional Spell Spawn Objects")]
    [SerializeField] private GameObject aquariusSpawnObject;
    [SerializeField] private GameObject piscesSpawnObject;

    [Header("Quest Complete")]
    [SerializeField] private GameObject finalBeaconVFX;

    private int waterBowlsActivated;
    private int windChimesActivated;
    private int dragonShrinesCompleted;
    private int airChambersActivated;

    private bool waterStepComplete;
    private bool windStepComplete;
    private bool zodiacSpellsUnlocked;
    private bool dragonStepComplete;
    private bool airChamberStepComplete;
    private bool questComplete;

    private void Start()
    {
        InitializeQuestObjects();
        RegisterEvents();

        Debug.Log("Quest 3 started. Activate 3 water bowls and 3 wind chimes.");
    }

    private void InitializeQuestObjects()
    {
        SetObjectActive(aquariusUI, false);
        SetObjectActive(piscesUI, false);
        SetObjectActive(aquariusSpawnObject, false);
        SetObjectActive(piscesSpawnObject, false);
        SetObjectActive(finalBeaconVFX, false);

        SetDragonShrinesEnabled(false);
        SetAirChambersEnabled(false);
    }

    private void RegisterEvents()
    {
        foreach (WaterBowlController bowl in waterBowls)
        {
            if (bowl != null)
                bowl.onActivated.AddListener(OnWaterBowlActivated);
        }

        foreach (WindChimeController chime in windChimes)
        {
            if (chime != null)
                chime.onActivated.AddListener(OnWindChimeActivated);
        }

        foreach (DragonController dragon in dragonShrines)
        {
            if (dragon != null)
                dragon.onCompleted.AddListener(OnDragonShrineCompleted);
        }

        foreach (AirChamberController chamber in airChambers)
        {
            if (chamber != null)
                chamber.onActivated.AddListener(OnAirChamberActivated);
        }
    }

    private void OnWaterBowlActivated()
    {
        if (waterStepComplete) return;

        waterBowlsActivated++;

        Debug.Log($"Quest 3: Water bowls activated {waterBowlsActivated}/{waterBowls.Length}");

        if (waterBowlsActivated >= waterBowls.Length)
        {
            waterStepComplete = true;
            Debug.Log("Quest 3: Water bowl step complete.");
            TryUnlockZodiacSpells();
        }
    }

    private void OnWindChimeActivated()
    {
        if (windStepComplete) return;

        windChimesActivated++;

        Debug.Log($"Quest 3: Wind chimes activated {windChimesActivated}/{windChimes.Length}");

        if (windChimesActivated >= windChimes.Length)
        {
            windStepComplete = true;
            Debug.Log("Quest 3: Wind chime step complete.");
            TryUnlockZodiacSpells();
        }
    }

    private void TryUnlockZodiacSpells()
    {
        if (zodiacSpellsUnlocked) return;
        if (!waterStepComplete || !windStepComplete) return;
        // Unlock spells
        if (wandManager != null)
        {
            wandManager.ActivateSpell(SpellType.Aquarius);
            wandManager.ActivateSpell(SpellType.Pisces);
        }
        // Show UI and activate other regions
        zodiacSpellsUnlocked = true;

        SetObjectActive(aquariusUI, true);
        SetObjectActive(piscesUI, true);

        SetObjectActive(aquariusSpawnObject, true);
        SetObjectActive(piscesSpawnObject, true);

        SetDragonShrinesEnabled(true);
        SetAirChambersEnabled(true);

        Debug.Log("Quest 3: Aquarius and Pisces are now available.");
    }

    private void OnDragonShrineCompleted()
    {
        if (!zodiacSpellsUnlocked) return;
        if (dragonStepComplete) return;

        dragonShrinesCompleted++;

        Debug.Log($"Quest 3: Dragon shrines completed {dragonShrinesCompleted}/{dragonShrines.Length}");

        if (dragonShrinesCompleted >= dragonShrines.Length)
        {
            dragonStepComplete = true;
            Debug.Log("Quest 3: Dragon shrine step complete.");
            TryCompleteQuest();
        }
    }

    private void OnAirChamberActivated()
    {
        if (!zodiacSpellsUnlocked) return;
        if (airChamberStepComplete) return;

        airChambersActivated++;

        Debug.Log($"Quest 3: Air chambers activated {airChambersActivated}/{airChambers.Length}");

        if (airChambersActivated >= airChambers.Length)
        {
            airChamberStepComplete = true;
            Debug.Log("Quest 3: Air chamber step complete.");
            TryCompleteQuest();
        }
    }

    private void TryCompleteQuest()
    {
        if (questComplete) return;
        if (!dragonStepComplete || !airChamberStepComplete) return;

        questComplete = true;

        SetObjectActive(finalBeaconVFX, true);

        Debug.Log("Quest 3 complete. Final beacon activated.");
    }

    private void SetDragonShrinesEnabled(bool enabled)
    {
        foreach (DragonController dragon in dragonShrines)
        {
            if (dragon == null) continue;

            Collider col = dragon.GetComponent<Collider>();

            if (col != null)
                col.enabled = enabled;
        }
    }

    private void SetAirChambersEnabled(bool enabled)
    {
        foreach (AirChamberController chamber in airChambers)
        {
            if (chamber == null) continue;

            Collider col = chamber.GetComponent<Collider>();

            if (col != null)
                col.enabled = enabled;
        }
    }

    private void SetObjectActive(GameObject target, bool active)
    {
        if (target != null)
            target.SetActive(active);
    }
}
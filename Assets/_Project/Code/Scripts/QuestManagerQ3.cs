using _Project.Features.Spells.Scripts;
using UnityEngine;

public class QuestManagerQ3 : MonoBehaviour
{
    [SerializeField] private WandManager wandManager;

    [Header("Quest 3 Objects")]
    [SerializeField] private WaterBowlController[] waterBowls;
    [SerializeField] private WindChimeController[] windChimes;
    [SerializeField] private DragonController[] dragonShrines;
    [SerializeField] private AirChamberController[] airChambers;

    [Header("Spell Unlocks")]
    [SerializeField] private GameObject aquariusUI;
    [SerializeField] private GameObject piscesUI;
    [SerializeField] private GameObject aquariusSpawnObject;
    [SerializeField] private GameObject piscesSpawnObject;

    [Header("Quest Complete")]
    [SerializeField] private GameObject finalBeaconVFX;

    private int waterBowlsActivated = 0;
    private int windChimesActivated = 0;
    private int dragonShrinesCompleted = 0;
    private int airChambersActivated = 0;

    private bool waterStepComplete = false;
    private bool windStepComplete = false;
    private bool zodiacSpellsUnlocked = false;
    private bool dragonStepComplete = false;
    private bool airChamberStepComplete = false;
    private bool questComplete = false;

    private void Start()
    {
        if (aquariusUI != null) aquariusUI.SetActive(false);
        if (piscesUI != null) piscesUI.SetActive(false);
        if (aquariusSpawnObject != null) aquariusSpawnObject.SetActive(false);
        if (piscesSpawnObject != null) piscesSpawnObject.SetActive(false);
        if (finalBeaconVFX != null) finalBeaconVFX.SetActive(false);

        SetDragonShrinesEnabled(false);
        SetAirChambersEnabled(false);

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

        if (waterBowlsActivated >= waterBowls.Length)
        {
            waterStepComplete = true;
            TryUnlockZodiacSpells();
        }
    }

    private void OnWindChimeActivated()
    {
        if (windStepComplete) return;

        windChimesActivated++;

        if (windChimesActivated >= windChimes.Length)
        {
            windStepComplete = true;
            TryUnlockZodiacSpells();
        }
    }

    private void TryUnlockZodiacSpells()
    {
        if (zodiacSpellsUnlocked) return;
        if (!waterStepComplete || !windStepComplete) return;

        zodiacSpellsUnlocked = true;

        if (aquariusUI != null) aquariusUI.SetActive(true);
        if (piscesUI != null) piscesUI.SetActive(true);
        if (aquariusSpawnObject != null) aquariusSpawnObject.SetActive(true);
        if (piscesSpawnObject != null) piscesSpawnObject.SetActive(true);

        if (wandManager != null)
        {
            wandManager.ActivateSpell(SpellType.Aquarius);
            wandManager.ActivateSpell(SpellType.Pisces);
        }

        SetDragonShrinesEnabled(true);
        SetAirChambersEnabled(true);

        Debug.Log("Quest 3: Aquarius and Pisces unlocked.");
    }

    private void OnDragonShrineCompleted()
    {
        if (!zodiacSpellsUnlocked) return;
        if (dragonStepComplete) return;

        dragonShrinesCompleted++;

        if (dragonShrinesCompleted >= dragonShrines.Length)
        {
            dragonStepComplete = true;
            TryCompleteQuest();
        }
    }

    private void OnAirChamberActivated()
    {
        if (!zodiacSpellsUnlocked) return;
        if (airChamberStepComplete) return;

        airChambersActivated++;

        if (airChambersActivated >= airChambers.Length)
        {
            airChamberStepComplete = true;
            TryCompleteQuest();
        }
    }

    private void TryCompleteQuest()
    {
        if (questComplete) return;
        if (!dragonStepComplete || !airChamberStepComplete) return;

        questComplete = true;

        if (finalBeaconVFX != null)
            finalBeaconVFX.SetActive(true);

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
}
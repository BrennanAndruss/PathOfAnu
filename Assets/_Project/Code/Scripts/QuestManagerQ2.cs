using _Project.Features.Spells.Scripts;
using UnityEngine;

public class QuestManagerQ2 : MonoBehaviour
{
    [SerializeField] private WandManager wandManager;

    [Header("Quest 2 Objects")]
    [SerializeField] private LanternController[] lanterns;
    [SerializeField] private FireChamberController[] fireChambers;
    [SerializeField] private GameObject leoUI;
    [SerializeField] private GameObject beaconVFX;

    [Header("Quest 2 Progress")]
    [SerializeField] private int lanternsActivated = 0;
    [SerializeField] private int fireChambersCompleted = 0;

    [Header("Quest 2 Gates")]
    [SerializeField] private bool lanternsComplete = false;
    [SerializeField] private bool fireChambersComplete = false;
    [SerializeField] private bool quest2Completed = false;

    private void Start()
    {
        lanternsActivated = 0;
        fireChambersCompleted = 0;

        if (leoUI != null)
            leoUI.SetActive(false);

        if (beaconVFX != null)
            beaconVFX.SetActive(false);

        SetFireChambersEnabled(false);

        foreach (LanternController lantern in lanterns)
        {
            if (lantern == null) continue;
            lantern.onActivated.AddListener(OnLanternActivated);
        }

        foreach (FireChamberController chamber in fireChambers)
        {
            if (chamber == null) continue;
            chamber.onCompleted.AddListener(OnFireChamberCompleted);
        }
    }

    private void OnDestroy()
    {
        foreach (LanternController lantern in lanterns)
        {
            if (lantern == null) continue;
            lantern.onActivated.RemoveListener(OnLanternActivated);
        }

        foreach (FireChamberController chamber in fireChambers)
        {
            if (chamber == null) continue;
            chamber.onCompleted.RemoveListener(OnFireChamberCompleted);
        }
    }

    public void OnLanternActivated()
    {
        if (lanternsComplete) return;

        lanternsActivated++;

        if (lanternsActivated >= lanterns.Length)
        {
            CompleteLanternStep();
        }
    }

    private void CompleteLanternStep()
    {
        lanternsComplete = true;

        foreach (LanternController lantern in lanterns)
        {
            if (lantern == null) continue;

            lantern.PlayCompleteVFX();
        }

        if (leoUI != null)
            leoUI.SetActive(true);

        if (wandManager != null)
            wandManager.ActivateSpell(SpellType.Leo);

        SetFireChambersEnabled(true);

        Debug.Log("Quest 2: All lanterns activated. Leo spell unlocked.");
    }

    public void OnFireChamberCompleted()
    {
        if (!lanternsComplete) return;
        if (fireChambersComplete) return;

        fireChambersCompleted++;

        if (fireChambersCompleted >= fireChambers.Length)
        {
            CompleteFireChamberStep();
        }
    }

    private void CompleteFireChamberStep()
    {
        fireChambersComplete = true;
        quest2Completed = true;

        if (beaconVFX != null)
            beaconVFX.SetActive(true);

        Debug.Log("Quest 2 complete. Beacon activated.");
    }

    private void SetFireChambersEnabled(bool enabled)
    {
        foreach (FireChamberController chamber in fireChambers)
        {
            if (chamber == null) continue;

            Collider col = chamber.GetComponent<Collider>();
            if (col != null)
                col.enabled = enabled;
        }
    }
}
using _Project.Code.Scripts.Interactables;
using _Project.Features.Spells.Scripts;
using UnityEngine;
using UnityEngine.Events;

public class FireChamberController : Interactable
{
    public UnityEvent onCompleted = new();

    [Header("Fire Chamber Progress")]
    [SerializeField] private int hitsRequired = 3;
    [SerializeField] private int currentHits = 0;

    [Header("Pre-Placed Flame VFX")]
    [SerializeField] private GameObject[] flameVFXOrder;

    private bool completed = false;

    private void Awake()
    {
        requiredType = SpellType.Leo;
        triggerOnlyOnce = false;

        DisableAllFlames();

        if (hitsRequired != flameVFXOrder.Length)
        {
            Debug.LogWarning($"{gameObject.name}: Hits Required does not match Flame VFX count.");
        }
    }

    protected override void OnInteract(Spell spell)
    {
        if (completed) return;

        currentHits++;

        ActivateFlameForCurrentHit();

        if (currentHits >= hitsRequired)
        {
            CompleteFireChamber();
        }
    }

    private void ActivateFlameForCurrentHit()
    {
        int index = currentHits - 1;

        if (index < 0 || index >= flameVFXOrder.Length) return;

        GameObject flame = flameVFXOrder[index];

        if (flame == null) return;

        flame.SetActive(true);
    }

    private void DisableAllFlames()
    {
        foreach (GameObject flame in flameVFXOrder)
        {
            if (flame == null) continue;

            flame.SetActive(false);
        }
    }

    private void CompleteFireChamber()
    {
        completed = true;
        onCompleted?.Invoke();

        Debug.Log($"{gameObject.name}: Fire chamber complete.");
    }
}
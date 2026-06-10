using _Project.Code.Scripts.Interactables;
using _Project.Features.Spells.Scripts;
using UnityEngine;
using UnityEngine.Events;

public class DragonController : Interactable
{
    public UnityEvent onCompleted = new();

    [Header("Dragon Progress")]
    [SerializeField] private int hitsRequired = 3;
    [SerializeField] private int currentHits;

    [Header("Stage VFX")]
    [SerializeField] private GameObject[] dragonVFXOrder;

    private bool completed;

    private void Awake()
    {
        triggerOnlyOnce = false;

        DisableAllVFX();
    }

    protected override void OnInteract(Spell spell)
    {
        if (completed) return;

        currentHits++;

        ActivateCurrentVFX();

        if (currentHits >= hitsRequired)
        {
            CompleteDragon();
        }
    }

    private void ActivateCurrentVFX()
    {
        int index = currentHits - 1;

        if (index < 0 || index >= dragonVFXOrder.Length)
            return;

        if (dragonVFXOrder[index] != null)
            dragonVFXOrder[index].SetActive(true);
    }

    private void DisableAllVFX()
    {
        foreach (GameObject vfx in dragonVFXOrder)
        {
            if (vfx != null)
                vfx.SetActive(false);
        }
    }

    private void CompleteDragon()
    {
        completed = true;

        onCompleted?.Invoke();

        Debug.Log($"{gameObject.name}: Dragon shrine complete.");
    }
}
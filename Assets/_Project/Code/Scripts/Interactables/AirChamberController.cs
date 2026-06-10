using _Project.Code.Scripts.Interactables;
using _Project.Features.Spells.Scripts;
using UnityEngine;
using UnityEngine.Events;

public class AirChamberController : Interactable
{
    public UnityEvent onActivated = new();

    [Header("Activation VFX")]
    [SerializeField] private GameObject[] activationVFX;

    [Header("Ritual Audio")]
    [SerializeField] private AudioSource ritualAudioSource;

    private bool activated;

    private void Awake()
    {
        triggerOnlyOnce = true;

        DisableAllVFX();
    }

    protected override void OnInteract(Spell spell)
    {
        if (activated) return;

        activated = true;

        foreach (GameObject vfx in activationVFX)
        {
            if (vfx != null)
                vfx.SetActive(true);
        }

        if (ritualAudioSource != null)
            ritualAudioSource.Play();

        onActivated?.Invoke();

        Debug.Log($"{gameObject.name}: Air chamber activated.");
    }

    private void DisableAllVFX()
    {
        foreach (GameObject vfx in activationVFX)
        {
            if (vfx != null)
                vfx.SetActive(false);
        }
    }
}
using System.Collections.Generic;
using _Project.Code.Scripts.Interactables;
using _Project.Features.Spells.Scripts;
using UnityEngine;
using UnityEngine.Events;

public class WaterBowlController : Interactable
{
    public UnityEvent onActivated = new();

    [SerializeField] private GameObject activationVFX;
    [SerializeField] private List<GameObject> waterVisuals;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip activationSound;

    private bool activated = false;

    private void Awake()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    protected override void OnInteract(Spell spell)
    {
        if (activated) return;

        activated = true;

        if (activationVFX != null)
            activationVFX.SetActive(true);

        if (audioSource != null && activationSound != null)
            audioSource.PlayOneShot(activationSound);

        foreach (var waterVisual in waterVisuals)
        {
            if (waterVisual != null)
                waterVisual.SetActive(true);
        }

        onActivated?.Invoke();

        Debug.Log($"{gameObject.name}: Water bowl activated.");
    }
}
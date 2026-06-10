using System.Collections.Generic;
using _Project.Code.Scripts.Interactables;
using _Project.Features.Spells.Scripts;
using UnityEngine;
using UnityEngine.Events;

public class WaterBowlController : Interactable
{
    public UnityEvent onActivated = new();

    [Header("Water Visuals")]
    [SerializeField] private List<GameObject> waterVisuals;

    [Header("Optional Effects")]
    [SerializeField] private GameObject activationVFX;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip activationSound;

    private bool activated;

    private void Awake()
    {
        triggerOnlyOnce = true;

        DisableAllWaterVisuals();

        if (activationVFX != null)
            activationVFX.SetActive(false);

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    protected override void OnInteract(Spell spell)
    {
        if (activated) return;

        activated = true;

        foreach (GameObject waterVisual in waterVisuals)
        {
            if (waterVisual != null)
                waterVisual.SetActive(true);
        }

        if (activationVFX != null)
            activationVFX.SetActive(true);

        if (audioSource != null && activationSound != null)
            audioSource.PlayOneShot(activationSound);

        onActivated?.Invoke();

        Debug.Log($"{gameObject.name}: Water bowl activated.");
    }

    private void DisableAllWaterVisuals()
    {
        foreach (GameObject waterVisual in waterVisuals)
        {
            if (waterVisual != null)
                waterVisual.SetActive(false);
        }
    }
}
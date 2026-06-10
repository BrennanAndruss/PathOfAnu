using _Project.Code.Scripts.Interactables;
using _Project.Features.Spells.Scripts;
using UnityEngine;
using UnityEngine.Events;
using Deform;

public class WindChimeController : Interactable
{
    public UnityEvent onActivated = new();

    [Header("Wind Chime Animation")]
    [SerializeField] private SineDeformer sineDeformer;
    [SerializeField] private float activeFactor = 1f;
    [SerializeField] private float inactiveFactor = 0f;
    [SerializeField] private float waveSpeed = 2f;

    [Header("Optional Effects")]
    [SerializeField] private GameObject activationVFX;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip activationSound;

    private bool activated;

    private void Awake()
    {
        triggerOnlyOnce = true;

        if (sineDeformer != null)
            sineDeformer.Factor = inactiveFactor;

        if (activationVFX != null)
            activationVFX.SetActive(false);

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (!activated) return;
        if (sineDeformer == null) return;

        sineDeformer.Offset += Time.deltaTime * waveSpeed;
    }

    protected override void OnInteract(Spell spell)
    {
        if (activated) return;

        activated = true;

        if (sineDeformer != null)
            sineDeformer.Factor = activeFactor;

        if (activationVFX != null)
            activationVFX.SetActive(true);

        if (audioSource != null && activationSound != null)
            audioSource.PlayOneShot(activationSound);

        onActivated?.Invoke();

        Debug.Log($"{gameObject.name}: Wind chime activated.");
    }
}
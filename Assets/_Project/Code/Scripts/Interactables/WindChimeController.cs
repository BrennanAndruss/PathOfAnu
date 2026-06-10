using _Project.Code.Scripts.Interactables;
using _Project.Features.Spells.Scripts;
using UnityEngine;
using UnityEngine.Events;
using Deform;

public class WindChimeController : Interactable
{
    public UnityEvent onActivated = new();

    [Header("Visual / Audio")]
    [SerializeField] private GameObject activationVFX;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip activationSound;

    [Header("Sine Deformer Animation")]
    [SerializeField] private SineDeformer sineDeformer;
    [SerializeField] private float activeFactor = 1f;
    [SerializeField] private float inactiveFactor = 0f;
    [SerializeField] private float waveSpeed = 2f;

    [Header("Debug")]
    [SerializeField] private bool debugLogs = true;

    private bool activated = false;

    private void Awake()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        SetWaveFactor(inactiveFactor);
    }

    private void Update()
    {
        if (!activated || sineDeformer == null)
            return;

        sineDeformer.Factor = activeFactor;
        sineDeformer.Offset += Time.deltaTime * waveSpeed;
    }

    protected override void OnInteract(Spell spell)
    {
        if (activated)
            return;

        activated = true;

        if (activationVFX != null)
            activationVFX.SetActive(true);

        if (audioSource != null && activationSound != null)
            audioSource.PlayOneShot(activationSound);

        SetWaveFactor(activeFactor);

        onActivated?.Invoke();

        if (debugLogs)
            Debug.Log($"{gameObject.name}: Wind chime activated.");
    }

    private void SetWaveFactor(float factor)
    {
        if (sineDeformer == null)
        {
            if (debugLogs)
                Debug.LogWarning($"{gameObject.name}: No SineDeformer assigned.");

            return;
        }

        sineDeformer.Factor = factor;
    }
}
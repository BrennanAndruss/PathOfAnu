using _Project.Code.Scripts.Interactables;
using _Project.Features.Spells.Scripts;
using UnityEngine;
using UnityEngine.Events;

public class LanternController : Interactable
{
    public UnityEvent onActivated = new();

    [Header("Lantern State")]
    [SerializeField] private bool activated = false;

    [Header("Glow Settings")]
    [SerializeField] private float emissionBrightnessMultiplier = 3f;

    [Header("VFX")]
    [SerializeField] private GameObject activationVFX;
    [SerializeField] private GameObject completeVFX;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip activationSound;

    private void Awake()
    {
        requiredType = SpellType.Fire;
        triggerOnlyOnce = true;

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        DisableGlow();
    }

    protected override void OnInteract(Spell spell)
    {
        if (activated) return;

        activated = true;

        EnableGlow();
        PlayActivationVFX();
        PlayActivationSound();

        onActivated?.Invoke();

        Debug.Log($"{gameObject.name}: Lantern activated.");
    }

    public void EnableGlow()
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>();

        foreach (Renderer rend in renderers)
        {
            foreach (Material mat in rend.materials)
            {
                if (mat == null) continue;
                if (!mat.HasProperty("_EmissionColor")) continue;

                mat.EnableKeyword("_EMISSION");

                Color baseEmission = mat.GetColor("_EmissionColor");
                mat.SetColor("_EmissionColor", baseEmission * emissionBrightnessMultiplier);
            }
        }
    }

    public void DisableGlow()
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>();

        foreach (Renderer rend in renderers)
        {
            foreach (Material mat in rend.materials)
            {
                if (mat == null) continue;
                if (!mat.HasProperty("_EmissionColor")) continue;

                mat.DisableKeyword("_EMISSION");
            }
        }
    }

    public void PlayActivationVFX()
    {
        if (activationVFX == null) return;

        Instantiate(
            activationVFX,
            transform.position,
            transform.rotation,
            transform
        );
    }

    public void PlayCompleteVFX()
    {
        if (completeVFX == null) return;

        Instantiate(
            completeVFX,
            transform.position,
            transform.rotation,
            transform
        );
    }

    private void PlayActivationSound()
    {
        if (audioSource == null || activationSound == null) return;

        audioSource.PlayOneShot(activationSound);
    }
}
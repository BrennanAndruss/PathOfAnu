using _Project.Code.Scripts.Interactables;
using _Project.Features.Spells.Scripts;
using UnityEngine;
using UnityEngine.Events;

public class AirChamberController : Interactable
{
    public UnityEvent onActivated = new();

    [SerializeField] private GameObject activationVFX;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip ritualMusic;

    private bool activated = false;

    private void Awake()
    {
        requiredType = SpellType.Pisces;
        triggerOnlyOnce = true;

        if (activationVFX != null)
            activationVFX.SetActive(false);

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    protected override void OnInteract(Spell spell)
    {
        if (activated) return;

        activated = true;

        if (activationVFX != null)
            activationVFX.SetActive(true);

        if (audioSource != null && ritualMusic != null)
        {
            audioSource.clip = ritualMusic;
            audioSource.loop = true;
            audioSource.Play();
        }

        onActivated?.Invoke();

        Debug.Log($"{gameObject.name}: Air chamber activated.");
    }
}
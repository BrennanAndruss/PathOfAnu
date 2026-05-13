using _Project.Code.Scripts.Interactables;
using _Project.Features.Spells.Scripts;
using UnityEngine;
using UnityEngine.Events;

public class RuineController : Interactable
{
    [Header("Rune Requirement")]
    [Tooltip("Set required spell here. Mapped to Interactable.requiredType at Awake.")]
    [SerializeField] private SpellType requiredSpell = SpellType.Unknown;

    [Header("VFX / SFX")]
    [SerializeField] private GameObject correctSpellVFX;
    [SerializeField] private GameObject incorrectSpellVFX;
    [SerializeField] private AudioClip successSfx;
    [SerializeField] private AudioClip failSfx;
    [SerializeField] private bool disableAfterActivation = true;

    [Header("State (debug)")]
    [SerializeField] private bool activated;

    [Header("Events")]
    public UnityEvent onActivated;
    public UnityEvent onFailed;

    private AudioSource _audioSource;

    private void Awake()
    {
        requiredType = requiredSpell;
        _audioSource = GetComponent<AudioSource>();
    }

    protected override void OnInteract(Spell spell)
    {
        if (activated) return;
        if (spell == null) return;

        if (requiredType != SpellType.Unknown && spell.GetSpellType() != requiredType)
        {
            SpawnVfx(incorrectSpellVFX);
            PlaySound(failSfx);
            onFailed?.Invoke();
            return;
        }

        activated = true;

        SpawnVfx(correctSpellVFX);
        PlaySound(successSfx);
        onActivated?.Invoke();

        if (disableAfterActivation)
        {
            Collider col = GetComponent<Collider>();
            if (col != null)
                col.enabled = false;
        }
    }

    private void SpawnVfx(GameObject prefab)
    {
        if (prefab == null) return;

        Instantiate(prefab, transform.position, Quaternion.identity);
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip == null) return;

        if (_audioSource != null)
            _audioSource.PlayOneShot(clip);
        else
            AudioSource.PlayClipAtPoint(clip, transform.position);
    }
}
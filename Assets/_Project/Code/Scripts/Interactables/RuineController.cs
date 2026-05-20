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
    [SerializeField] private GameObject despawnVFX;

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

    private void Start()
    {
        if (correctSpellVFX != null) correctSpellVFX.SetActive(false);
        if (incorrectSpellVFX != null) incorrectSpellVFX.SetActive(false);
        if (despawnVFX != null) despawnVFX.SetActive(false);
    }

    protected override void OnInteractFail(Spell spell)
    {
        if (activated) return;

        PlayPlacedVfx(incorrectSpellVFX);
        PlaySound(failSfx);
        onFailed?.Invoke();
    }

   protected override void OnInteract(Spell spell)
    {
        if (activated) return;
        if (spell == null) return;

        activated = true;

        PlayPlacedVfx(correctSpellVFX);
        PlaySound(successSfx);
        onActivated?.Invoke();

        if (disableAfterActivation)
        {
            Collider col = GetComponent<Collider>();
            if (col != null)
                col.enabled = false;
        }
    }

    public void PlayDespawnVFX()
    {
        PlayPlacedVfx(despawnVFX);
    }

    private void PlayPlacedVfx(GameObject vfxObject)
    {
        if (vfxObject == null) return;

        vfxObject.SetActive(true);

        ParticleSystem[] particles = vfxObject.GetComponentsInChildren<ParticleSystem>();

        foreach (ParticleSystem ps in particles)
        {
            ps.Clear();
            ps.Play();
        }
    }

    public void DisableActivationVFX()
    {
        if (correctSpellVFX != null) correctSpellVFX.SetActive(false);
        if (incorrectSpellVFX != null) incorrectSpellVFX.SetActive(false);
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
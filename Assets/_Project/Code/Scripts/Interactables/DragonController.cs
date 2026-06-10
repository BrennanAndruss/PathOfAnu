using _Project.Code.Scripts.Interactables;
using _Project.Features.Spells.Scripts;
using UnityEngine;
using UnityEngine.Events;

public class DragonController : Interactable
{
    public UnityEvent onCompleted = new();

    [SerializeField] private int hitsRequired = 3;
    [SerializeField] private GameObject[] hitVFXOrder;
    [SerializeField] private GameObject completedVFX;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip hitSound;
    [SerializeField] private AudioClip completedSound;

    private int currentHits = 0;
    private bool completed = false;

    private void Awake()
    {
        requiredType = SpellType.Aquarius;
        triggerOnlyOnce = false;

        foreach (GameObject vfx in hitVFXOrder)
        {
            if (vfx != null)
                vfx.SetActive(false);
        }

        if (completedVFX != null)
            completedVFX.SetActive(false);

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    protected override void OnInteract(Spell spell)
    {
        if (completed) return;

        currentHits++;

        int index = currentHits - 1;

        if (index >= 0 && index < hitVFXOrder.Length && hitVFXOrder[index] != null)
            hitVFXOrder[index].SetActive(true);

        if (audioSource != null && hitSound != null)
            audioSource.PlayOneShot(hitSound);

        if (currentHits >= hitsRequired)
            CompleteDragonShrine();
    }

    private void CompleteDragonShrine()
    {
        completed = true;

        if (completedVFX != null)
            completedVFX.SetActive(true);

        if (audioSource != null && completedSound != null)
            audioSource.PlayOneShot(completedSound);

        onCompleted?.Invoke();

        Debug.Log($"{gameObject.name}: Dragon shrine complete.");
    }
}
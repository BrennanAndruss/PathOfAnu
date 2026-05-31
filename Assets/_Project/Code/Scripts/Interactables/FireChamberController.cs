using _Project.Code.Scripts.Interactables;
using _Project.Features.Spells.Scripts;
using UnityEngine;
using UnityEngine.Events;

public class FireChamberController : Interactable
{
    public UnityEvent onCompleted;

    [Header("Fire Chamber Progress")]
    [SerializeField] private int hitsRequired = 3;
    [SerializeField] private int currentHits = 0;

    [Header("Flame VFX Order")]
    [SerializeField] private GameObject[] flameVFXOrder;
    [SerializeField] private Transform[] flameSpawnPoints;

    private bool completed = false;

    private void Awake()
    {
        requiredType = SpellType.Leo;
        triggerOnlyOnce = false;
    }

    protected override void OnInteract(Spell spell)
    {
        if (completed) return;

        currentHits++;

        SpawnFlameForCurrentHit();

        if (currentHits >= hitsRequired)
        {
            CompleteFireChamber();
        }
    }

    private void SpawnFlameForCurrentHit()
    {
        int index = currentHits - 1;

        if (index < 0 || index >= flameVFXOrder.Length) return;

        GameObject prefab = flameVFXOrder[index];
        if (prefab == null) return;

        Transform spawnPoint = null;

        if (flameSpawnPoints != null && index < flameSpawnPoints.Length)
            spawnPoint = flameSpawnPoints[index];

        Vector3 position = spawnPoint != null ? spawnPoint.position : transform.position;
        Quaternion rotation = spawnPoint != null ? spawnPoint.rotation : transform.rotation;

        Instantiate(prefab, position, rotation, transform);
    }

    private void CompleteFireChamber()
    {
        completed = true;
        onCompleted?.Invoke();

        Debug.Log($"{gameObject.name}: Fire chamber complete.");
    }
}
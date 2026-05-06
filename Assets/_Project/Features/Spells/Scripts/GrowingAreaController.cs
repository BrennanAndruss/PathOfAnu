using UnityEngine;

public class GrowingAreaController : MonoBehaviour
{
    [Header("Geometry To Activate")]
    [SerializeField] private GameObject[] growingAreas;

    [Header("VFX")]
    [SerializeField] private GameObject growingSpellVFX;

    [Header("State")]
    [SerializeField] private bool activated = false;

    private void Start()
    {
        // Make sure everything starts hidden
        foreach (GameObject area in growingAreas)
        {
            if (area != null)
                area.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (activated)
            return;

        // TEMP: accept any collider (we'll filter later)
        ActivateGrowthArea();
    }

    private void ActivateGrowthArea()
    {
        activated = true;

        // Turn on all geometry
        foreach (GameObject area in growingAreas)
        {
            if (area != null)
                area.SetActive(true);
        }

        // Spawn VFX
        if (growingSpellVFX != null)
        {
            Instantiate(growingSpellVFX, transform.position, Quaternion.identity);
        }

        Debug.Log($"{gameObject.name} activated.");
    }
}
using System.Collections.Generic;
using UnityEngine;

public class BowlWaterFillTrigger : MonoBehaviour
{
    [Header("Tags")]
    [SerializeField] private string spellTag = "Bowl-Water";
    [SerializeField] private string bowlWaterTag = "Bowl-Water";

    [Header("Behavior")]
    [SerializeField] private bool disableWaterOnStart = true;
    [SerializeField] private bool fillOnlyOnce = true;

    private readonly List<GameObject> bowlWaterObjects = new List<GameObject>();
    private bool isFilled;

    private void Awake()
    {
        CacheBowlWaterObjects();

        if (disableWaterOnStart)
        {
            SetWaterActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (fillOnlyOnce && isFilled)
        {
            return;
        }

        if (!other.CompareTag(spellTag))
        {
            return;
        }

        SetWaterActive(true);
        isFilled = true;
    }

    private void CacheBowlWaterObjects()
    {
        bowlWaterObjects.Clear();

        Transform[] allTransforms = FindObjectsByType<Transform>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (Transform currentTransform in allTransforms)
        {
            if (currentTransform.CompareTag(bowlWaterTag))
            {
                bowlWaterObjects.Add(currentTransform.gameObject);
            }
        }
    }

    private void SetWaterActive(bool activeState)
    {
        if (bowlWaterObjects.Count == 0)
        {
            CacheBowlWaterObjects();
        }

        foreach (GameObject waterObject in bowlWaterObjects)
        {
            if (waterObject != null)
            {
                waterObject.SetActive(activeState);
            }
        }
    }
}

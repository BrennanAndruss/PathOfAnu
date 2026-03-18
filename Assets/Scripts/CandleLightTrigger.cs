using System.Collections.Generic;
using UnityEngine;

public class CandleLightTrigger : MonoBehaviour
{
    [Header("Tags")]
    [SerializeField] private string spellTag = "Spell";
    [SerializeField] private string candleFireTag = "CandleFire";

    [Header("Behavior")]
    [SerializeField] private bool disableFlamesOnStart = true;
    [SerializeField] private bool lightOnlyOnce = true;

    private readonly List<GameObject> candleFlames = new List<GameObject>();
    private bool isLit;

    private void Awake()
    {
        CacheCandleFlames();

        if (disableFlamesOnStart)
        {
            SetFlamesActive(false);
        }
    }
    // waiting for the onntriggerenter on the collider
    private void OnTriggerEnter(Collider other)
    {
        if (lightOnlyOnce && isLit)
        {
            return;
        }

        if (!other.CompareTag(spellTag))
        {
            return;
        }

        SetFlamesActive(true);
        isLit = true; 
    }
    // looking for the "candlefire" tag and adding it to the list of candle flames, then we can set them active or inactive as needed
    private void CacheCandleFlames()
    {
        candleFlames.Clear();

        Transform[] allTransforms = FindObjectsByType<Transform>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (Transform currentTransform in allTransforms)
        {
            if (currentTransform.CompareTag(candleFireTag))
            {
                candleFlames.Add(currentTransform.gameObject);
            }
        }
    }
    // method to set the active state of all candle flames in the list
    private void SetFlamesActive(bool activeState)
    {
        if (candleFlames.Count == 0)
        {
            CacheCandleFlames();
        }

        foreach (GameObject flame in candleFlames)
        {
            if (flame != null)
            {
                flame.SetActive(activeState);
            }
        }
    }
}

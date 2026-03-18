using UnityEngine;
using Deform;

public class WaveTrigger : MonoBehaviour
{
    public SineDeformer sineDeformer; 
    public string targetTag = "Spell";

    [Header("Mode")]
    public bool stayActiveAfterFirstContact = true;

    [Header("Wave Values")]
    public float activeFactor = 1f;
    public float inactiveFactor = 0f;
    public float waveSpeed = 2f; // How fast the chime wiggles

    [Header("Debug")]
    public bool debugLogs = true;

    private bool isLatchedActive;

    private void Start()
    {
        isLatchedActive = false;
        SetWaveFactor(inactiveFactor, "Start");
    }

    // This Update loop creates the continuous movement
    private void Update()
    {
        if (isLatchedActive && sineDeformer != null && !Mathf.Approximately(sineDeformer.Factor, activeFactor))
        {
            // Keep the wave on after the first valid spell contact.
            sineDeformer.Factor = activeFactor;
        }

        if (sineDeformer != null && sineDeformer.Factor > 0)
        {
            // Moving the Offset property is what makes the wave "scroll"
            sineDeformer.Offset += Time.deltaTime * waveSpeed;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(targetTag))
        {
            SetWaveFactor(activeFactor, "OnTriggerEnter");

            if (stayActiveAfterFirstContact && !isLatchedActive)
            {
                isLatchedActive = true; 

                if (debugLogs)
                {
                    Debug.Log($"[WaveTrigger] Latched active after first '{targetTag}' contact on '{name}'.", this);
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (isLatchedActive)
        {
            return;
        }

        if (other.CompareTag(targetTag))
        {
            SetWaveFactor(inactiveFactor, "OnTriggerExit");
        }
    }

    private void SetWaveFactor(float factor, string source)
    {
        if (sineDeformer == null)
        {
            if (debugLogs) Debug.LogWarning($"[WaveTrigger] No SineDeformer assigned on '{name}'.", this);
            return;
        }

        sineDeformer.Factor = factor;

        if (debugLogs)
        {
            Debug.Log($"[WaveTrigger] Set factor to {factor} via {source}.", this);
        }
    }
}

using UnityEngine;
using Deform; // Ensure you include the Deform namespace

public class WaveTrigger : MonoBehaviour
{
    public SineDeformer sineDeformer; // Drag your Sine Deformer here in the Inspector
    public string targetTag = "Spell";
    [Header("Wave Values")]
    public float activeFactor = 1f;
    public float inactiveFactor = 0f;

    [Header("Debug")]
    public bool debugLogs = true;

    private void Start()
    {
        // start with the with the wave disabled
        SetWaveFactor(inactiveFactor, "Start");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (debugLogs)
        {
            Debug.Log($"[WaveTrigger] OnTriggerEnter from '{other.name}' (tag: {other.tag}) on '{name}'. Looking for tag '{targetTag}'.", this);
        }

        if (other.CompareTag(targetTag))
        {
            // Activate the wave by setting its Factor (Amplitude)
            SetWaveFactor(activeFactor, "OnTriggerEnter");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (debugLogs)
        {
            Debug.Log($"[WaveTrigger] OnTriggerExit from '{other.name}' (tag: {other.tag}) on '{name}'.", this);
        }

        if (other.CompareTag(targetTag))
        {
            // Deactivate when they leave
            SetWaveFactor(inactiveFactor, "OnTriggerExit");
        }
    }

    private void SetWaveFactor(float factor, string source)
    {
        if (sineDeformer == null)
        {
            if (debugLogs)
            {
                Debug.LogWarning($"[WaveTrigger] No SineDeformer assigned on '{name}' while handling {source}.", this);
            }
            return;
        }

        sineDeformer.Factor = factor;

        if (debugLogs)
        {
            Debug.Log($"[WaveTrigger] Set factor to {factor} via {source} on '{name}'.", this);
        }
    }
}

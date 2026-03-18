using UnityEngine;
using Deform; // Ensure you include the Deform namespace

public class WaveTrigger : MonoBehaviour
{
    public SineDeformer sineDeformer; // Drag your Sine Deformer here in the Inspector
    public string targetTag = "Player";

    private void Start()
    {
        // start with the with the wave disabled
        if (sineDeformer != null) sineDeformer.Factor = 0f; 
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(targetTag))
        {
            // Activate the wave by setting its Factor (Amplitude)
            sineDeformer.Factor = 1f; 
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(targetTag))
        {
            // Deactivate when they leave
            sineDeformer.Factor = 0f; 
        }
    }
}

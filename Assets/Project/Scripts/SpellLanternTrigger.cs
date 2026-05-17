using UnityEngine;

/// <summary>
/// Detects when a Spell projectile collides with this object and enables emission on its material.
/// Attach this script to the object that should react to spell collisions.
/// </summary>
public class SpellInteractableTrigger : MonoBehaviour
{
    [SerializeField] private string materialName = ""; // Name of the material to enable emission on
    
    private void OnTriggerEnter(Collider collision)
    {
        // Check if the colliding object is tagged as "Spell"
        if (collision.CompareTag("Spell"))
        {
            // Enable emission on this object's material
            EnableEmission();
            
            Debug.Log($"Spell hit {gameObject.name}, emission enabled");
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Also handle non-trigger collisions
        if (collision.gameObject.CompareTag("Spell"))
        {
            // Enable emission on this object's material
            EnableEmission();
            
            Debug.Log($"Spell hit {gameObject.name}, emission enabled");
        }
    }

    private void EnableEmission()
    {
        // Get all renderers in this object and its children
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        
        if (renderers.Length == 0)
        {
            Debug.LogWarning($"{gameObject.name} does not have a Renderer component");
            return;
        }

        // Find the material by name
        foreach (Renderer renderer in renderers)
        {
            foreach (Material material in renderer.materials)
            {
                // If materialName is empty, use the first material found
                // Otherwise, search for the specific material by name
                if (string.IsNullOrEmpty(materialName) || material.name.Contains(materialName))
                {
                    if (material == null)
                    {
                        continue;
                    }

                    // Enable the emission keyword to use the material's configured emission map and color
                    material.EnableKeyword("_EMISSION");
                    
                    Debug.Log($"{material.name} emission enabled");
                    return; // Exit after finding and modifying the first matching material
                }
            }
        }

        //Debug.LogWarning($"Material '{materialName}' not found on {gameObject.name} or its children");
    }
}

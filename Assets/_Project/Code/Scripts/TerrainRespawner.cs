using UnityEngine;

namespace _Project.Code.Scripts
{
    public class TerrainRespawner : MonoBehaviour
    {
        [SerializeField] private float voidThreshold = -3.0f;
        [SerializeField] private LayerMask terrainLayer;
        [SerializeField] private float respawnNudgeUp = 1.5f;
        
        private void Update()
        {
            // Respawn above terrain if player drops below void threshold
            if (transform.position.y < voidThreshold)
            {
                RespawnAboveTerrain();
            }
        }

        private void RespawnAboveTerrain()
        {
            // Raycast from above the player's current XZ coordinate
            Vector3 rayStart = new Vector3(transform.position.x, 200f, transform.position.z);
            
            // Cast straight down onto terrain layer
            if (Physics.Raycast(rayStart, Vector3.down, out RaycastHit hit, 500f, terrainLayer))
            {
                transform.position = new Vector3(hit.point.x, hit.point.y + respawnNudgeUp, hit.point.z);
                Debug.Log("[TerrainRespawner] Snapping to terrain surface.");
            }
            else
            {
                Debug.LogError("[TerrainRespawner] Fell out of terrain bounds.");
            }
        }
    }
}

using UnityEngine;

public class CameraRigTerrainFollower : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Terrain terrain;
    [SerializeField] private LayerMask groundMask = ~0;
    [SerializeField] private float heightOffset = 0f;
    [SerializeField] private bool useRaycastFallback = true;
    [SerializeField] private float raycastStartHeight = 50f;
    [SerializeField] private float raycastDistance = 200f;
    [SerializeField] private bool disableChildGravity = true;

    private Rigidbody[] childRigidbodies;

    private void Reset()
    {
        target = transform;
    }

    private void Awake()
    {
        if (target == null)
        {
            target = transform;
        }

        if (terrain == null)
        {
            terrain = Terrain.activeTerrain;
        }

        // Cache child rigidbodies for gravity control
        childRigidbodies = GetComponentsInChildren<Rigidbody>();
        
        if (disableChildGravity)
        {
            foreach (Rigidbody rb in childRigidbodies)
            {
                rb.useGravity = false;
            }
        }
    }

    private void LateUpdate()
    {
        if (target == null)
        {
            return;
        }

        float groundY = GetGroundHeight(target.position);
        Vector3 position = target.position;
        position.y = groundY + heightOffset;
        target.position = position;

        // Zero out vertical velocity to prevent dropping
        Rigidbody targetRb = target.GetComponent<Rigidbody>();
        if (targetRb != null)
        {
            Vector3 velocity = targetRb.linearVelocity;
            velocity.y = 0f;
            targetRb.linearVelocity = velocity;
        }
    }

    private float GetGroundHeight(Vector3 worldPosition)
    {
        if (terrain != null)
        {
            Vector3 terrainPosition = terrain.GetPosition();
            Vector3 localPosition = worldPosition - terrainPosition;
            float terrainHeight = terrain.SampleHeight(worldPosition) + terrainPosition.y;
            if (terrainHeight > terrainPosition.y || terrain.terrainData != null)
            {
                return terrainHeight;
            }
        }

        if (useRaycastFallback)
        {
            Vector3 rayOrigin = worldPosition + Vector3.up * raycastStartHeight;
            if (Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hit, raycastDistance, groundMask, QueryTriggerInteraction.Ignore))
            {
                return hit.point.y;
            }
        }

        return worldPosition.y;
    }
}
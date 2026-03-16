using UnityEngine;

public class CrystalController : MonoBehaviour
{
    [Header("Base values")]
    [SerializeField] private float baseSpeed = 1f;
    [SerializeField] private float baseScale = 1f;

    [HideInInspector] public Material myMaterial;

    private Renderer _renderer;
    private Vector3 originalLocalScale;
    private float currentSpeed;

    void Awake()
    {
        _renderer= GetComponent<Renderer>();
        originalLocalScale = transform.localScale;
        currentSpeed = baseSpeed;
    }

    // Called by the spawner right after the wave prefab is instantiated
    public void ApplySettings(Material mat, float speedMultiplier, float scaleMultiplier)
    {
        myMaterial = mat;

        // Visual
        if (_renderer != null && myMaterial != null)
            _renderer.sharedMaterial = myMaterial;

        // Movement + size
        currentSpeed = baseSpeed * speedMultiplier;
        float finalScale = baseScale * scaleMultiplier;
        transform.localScale = originalLocalScale * finalScale;
        // setting it true in the same frame, it will show up all at once with the calls.
        gameObject.SetActive(true);
    }

    void Update()
    {
        transform.position += transform.forward * (currentSpeed * Time.deltaTime);
    }
}

using UnityEngine;

public class PlayerCrystalController : MonoBehaviour
{
    [SerializeField] private Material[] materialList;
    public Material playersMaterial;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if (materialList == null || materialList.Length == 0)
        {
            Debug.LogError("materialList empty on PlayerCrystalController");
            return;
        }
        Renderer renderer = GetComponent<Renderer>();
        int index = Random.Range(0, materialList.Length);
        renderer.sharedMaterial = materialList[index];
        playersMaterial = materialList[index];
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Material GetPlayerMaterial() => playersMaterial;

}

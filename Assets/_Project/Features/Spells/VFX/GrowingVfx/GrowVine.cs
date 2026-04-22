using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrowVineTUT : MonoBehaviour
{
    public List<MeshRenderer> growVinesMeshes;
    public float timeToGrow = 5f;

    [Range(0, 1)]
    public float minGrow = 1f;

    [Range(0, 1)]
    public float maxGrow = 0f;

    private List<Material> growVinesMaterials = new List<Material>();

    void Start()
    {
        for (int i = 0; i < growVinesMeshes.Count; i++)
        {
            Material[] mats = growVinesMeshes[i].materials;

            for (int j = 0; j < mats.Length; j++)
            {
                Material mat = mats[j];

                if (mat.HasProperty("Grow_")) // in the list of growing objects, check if it has a grow parameter that we use
                {
                    mat.SetFloat("Grow_", minGrow);
                    growVinesMaterials.Add(mat); //if so add to the list we are interpolating the grow variable through time
                }
            }
        }

        StartCoroutine(BeginGrowth()); // call coroutine on all meshes before game starts. 
    }

    IEnumerator BeginGrowth()
    {
        // Wait until the first frame has rendered
        yield return null; 

        float elapsed = 0f;

        while (elapsed < timeToGrow)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / timeToGrow);
            float growValue = Mathf.Lerp(minGrow, maxGrow, t);

            for (int i = 0; i < growVinesMaterials.Count; i++)
            {
                growVinesMaterials[i].SetFloat("Grow_", growValue);
            }

            yield return null;
        }

        StartCoroutine(BeginGrowth()); // continue running this co-routine. 
    }
}
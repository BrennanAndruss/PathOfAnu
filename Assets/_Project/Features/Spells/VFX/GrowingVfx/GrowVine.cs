using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrowVineTUT : MonoBehaviour
{
    public List<MeshRenderer> growVinesMeshes;
    public float timeToGrow = 5f;

    [Header("VFX")]
    public GameObject firefliesVFX;
    public GameObject earthSigilVFX;

    [Range(0, 1)] public float minGrow = 1f;
    [Range(0, 1)] public float maxGrow = 0f;

    private List<Material> growVinesMaterials = new List<Material>();

    private void OnEnable()
    {
        growVinesMaterials.Clear();

        for (int i = 0; i < growVinesMeshes.Count; i++)
        {
            Material[] mats = growVinesMeshes[i].materials;

            for (int j = 0; j < mats.Length; j++)
            {
                Material mat = mats[j];

                if (mat.HasProperty("Grow_"))
                {
                    mat.SetFloat("Grow_", minGrow);

                    if (!growVinesMaterials.Contains(mat))
                    {
                        growVinesMaterials.Add(mat);
                    }
                }
            }
        }

        if (firefliesVFX != null)
            firefliesVFX.SetActive(true);

        if (earthSigilVFX != null)
            earthSigilVFX.SetActive(true);

        StartCoroutine(BeginGrowth());
    }

    IEnumerator BeginGrowth()
    {
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

        for (int i = 0; i < growVinesMaterials.Count; i++)
        {
            growVinesMaterials[i].SetFloat("Grow_", maxGrow);
        }

        if (firefliesVFX != null)
            firefliesVFX.SetActive(false);

        if (earthSigilVFX != null)
            earthSigilVFX.SetActive(false);

        gameObject.SetActive(false);
    }
}
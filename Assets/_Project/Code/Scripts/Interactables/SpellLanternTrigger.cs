using System.Collections;
using System.Collections.Generic;
using _Project.Code.Scripts.Interactables;
using _Project.Features.Spells.Scripts;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Detects when a Fire spell projectile collides with this object and enables emission on its material.
/// Inherits from Interactable to ensure consistent spell interaction handling across all interactables.
/// </summary>
public class SpellLanternTrigger : Interactable
{
    [SerializeField] private string materialName = ""; // Name of the material to enable emission on
    [SerializeField] private GameObject vfxPrefab; // VFX effect to instantiate on trigger
    [SerializeField] private AudioClip triggerSound; // Sound to play on trigger
    [SerializeField] private AudioSource audioSource; // AudioSource component to play the sound
        [SerializeField] private bool disableEmissionOnStart = true;
        [SerializeField, Tooltip("Assign BeaconVFX2 from the scene to avoid runtime lookup (optional)")] private GameObject beaconObject;
        [SerializeField, Min(0.01f)] private float emissionFadeDuration = 1f;

        private readonly List<Material> lanternMaterials = new List<Material>();
        private Coroutine emissionRoutine;
        private static int lanternsLit = 0;
        private const int TOTAL_LANTERNS = 4;
        private const string BEACON_VFX_NAME = "BeaconVFX2";
        private bool hasCounted = false;
        private static bool beaconActivated = false;

        private void Awake()
        {
            CacheLanternMaterials();

            if (audioSource == null)
            {
                audioSource = GetComponent<AudioSource>();
            }

            // Ensure this lantern only triggers on Fire spells
            requiredType = SpellType.Fire;
            
            // Lantern stays on once triggered
            triggerOnlyOnce = true;

            if (disableEmissionOnStart)
            {
                SetEmissionInstant(false);
            }
        }
    protected override void OnInteract(Spell spell)
    {
        // Ensure this lantern only counts once
        if (hasCounted) return;

        // Start a linear fade-in to the material's emission color
        StartEmissionFade(true);
        PlayVFX();
        PlaySound();
        Debug.Log($"Fire spell hit {gameObject.name}, emission enabled");

        hasCounted = true;
        lanternsLit++;
        Debug.Log($"Lanterns lit: {lanternsLit}/{TOTAL_LANTERNS}");

        if (!beaconActivated && lanternsLit >= TOTAL_LANTERNS)
        {
            ActivateBeacon();
        }
    }

    protected override void OnInteractExit()
    {
        if (triggerOnlyOnce) return;
        // No fading/out behavior — keep lanterns lit once triggered
        Debug.Log($"Fire spell left {gameObject.name} (exit ignored because lanterns stay lit)");
    }

    private void CacheLanternMaterials()
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>();

        foreach (Renderer renderer in renderers)
        {
            foreach (Material material in renderer.materials)
            {
                if (material == null || !material.HasProperty("_EmissionColor"))
                {
                    continue;
                }

                if (!string.IsNullOrEmpty(materialName) && !material.name.Contains(materialName))
                {
                    continue;
                }

                if (!lanternMaterials.Contains(material))
                {
                    lanternMaterials.Add(material);
                }
            }
        }
    }

    private void SetEmissionInstant(bool enabled)
    {
        if (lanternMaterials.Count == 0)
        {
            return;
        }

        foreach (Material material in lanternMaterials)
        {
            if (material == null)
            {
                continue;
            }

            if (enabled)
            {
                material.EnableKeyword("_EMISSION");
            }
            else
            {
                material.DisableKeyword("_EMISSION");
            }
        }
    }

    private void StartEmissionFade(bool enable)
    {
        if (emissionRoutine != null)
        {
            StopCoroutine(emissionRoutine);
            emissionRoutine = null;
        }

        // Only fade in (enable=true) — we don't fade out to keep lanterns lit
        if (enable)
        {
            emissionRoutine = StartCoroutine(FadeEmission(true));
        }
        else
        {
            SetEmissionInstant(false);
        }
    }

    private IEnumerator FadeEmission(bool enable)
    {
        if (lanternMaterials.Count == 0)
        {
            yield break;
        }

        float duration = Mathf.Max(0.01f, emissionFadeDuration);
        float elapsed = 0f;
        var targetColors = new Dictionary<Material, Color>(lanternMaterials.Count);

        foreach (Material material in lanternMaterials)
        {
            if (material == null)
            {
                continue;
            }

            // Store the target emission color from the material
            targetColors[material] = material.GetColor("_EmissionColor");

            // Start from black and ensure keyword is enabled
            material.SetColor("_EmissionColor", Color.black);
            material.EnableKeyword("_EMISSION");
        }

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            foreach (Material material in lanternMaterials)
            {
                if (material == null)
                {
                    continue;
                }

                Color targetColor = targetColors.TryGetValue(material, out Color target) ? target : Color.white;
                material.SetColor("_EmissionColor", Color.Lerp(Color.black, targetColor, t));
            }

            yield return null;
        }

        SetEmissionInstant(true);
        emissionRoutine = null;
    }

    private void PlayVFX()
    {
        if (vfxPrefab == null)
        {
            Debug.LogWarning($"{gameObject.name}: VFX prefab not assigned", this);
            return;
        }

        Instantiate(vfxPrefab, transform.position, transform.rotation, transform);
    }

    private void PlaySound()
    {
        if (triggerSound == null)
        {
            Debug.LogWarning($"{gameObject.name}: Trigger sound not assigned", this);
            return;
        }

        if (audioSource == null)
        {
            Debug.LogWarning($"{gameObject.name}: AudioSource component not assigned", this);
            return;
        }

        audioSource.PlayOneShot(triggerSound);
    }

    private void ActivateBeacon()
    {
        Debug.Log($"ActivateBeacon called (from {gameObject.name}). lanternsLit={lanternsLit}, beaconActivated={beaconActivated}");

        // mark activated to prevent race conditions from multiple lanterns calling almost simultaneously
        beaconActivated = true;

        // Prefer an explicitly assigned beacon object from the inspector
        GameObject beacon = beaconObject != null ? beaconObject : FindObjectInSceneByName(BEACON_VFX_NAME);
        if (beaconObject != null)
        {
            Debug.Log($"ActivateBeacon: using inspector-assigned beacon '{beacon.name}'");
        }
        if (beacon == null)
        {
            Debug.LogError($"{BEACON_VFX_NAME} not found in scene!");
            return;
        }

        Debug.Log($"ActivateBeacon: found '{beacon.name}', activeInHierarchy={beacon.activeInHierarchy}");

        // Activate the beacon object if it's disabled
        if (!beacon.activeInHierarchy)
        {
            beacon.SetActive(true);
        }

        AudioSource beaconAudio = beacon.GetComponent<AudioSource>();
        if (beaconAudio != null)
        {
            beaconAudio.Play();
            Debug.Log("Beacon activated and sound playing!");
        }
        else
        {
            Debug.LogWarning($"{BEACON_VFX_NAME}: AudioSource component not found", beacon);
        }
    }

    // Finds a GameObject in the active scene by name, including inactive objects
    private GameObject FindObjectInSceneByName(string name)
    {
        var scene = SceneManager.GetActiveScene();
        var roots = scene.GetRootGameObjects();
        foreach (var root in roots)
        {
            var found = FindInChildrenRecursive(root.transform, name);
            if (found != null) return found.gameObject;
        }

        return null;
    }

    private Transform FindInChildrenRecursive(Transform parent, string name)
    {
        if (parent.name == name) return parent;
        foreach (Transform child in parent)
        {
            var found = FindInChildrenRecursive(child, name);
            if (found != null) return found;
        }

        return null;
    }
}

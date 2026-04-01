using System.Collections;
using System.Collections.Generic;
using Oculus.Interaction;
using Oculus.Interaction.Surfaces;
using UnityEngine;
using UnityEngine.Serialization;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class CubeGridPuzzle : MonoBehaviour
{
    private const int GridSize = 3;
    private const float TileSize = 1f;
    private const float TileSpacing = 1.35f;
    private const float RotateDuration = 0.2f;
    private const float ShakeDuration = 0.8f;
    private const float ShakeStrength = 0.12f;
    private const float BarrierMoveDuration = 1f;

    [FormerlySerializedAs("rock_prefab")]
    [SerializeField] private Object rockPrefab;

    private readonly List<PuzzleCubeTile> _tiles = new List<PuzzleCubeTile>();
    private bool _isSolved;
    private bool _isCelebrating;
    private bool _playerTrapped;
    private Transform _caveRoot;
    private Transform _barrierRock;
    private Vector3 _barrierOpenLocalPosition;
    private Vector3 _barrierClosedLocalPosition;
    private Vector3 _barrierLocalEulerAngles;
    private Vector3 _barrierLocalScale;
    private Vector3 _barrierColliderSize;
    private Coroutine _barrierRoutine;
    private bool _hasLoggedCameraDiscovery;

    private void Start()
    {
        Debug.Log($"[CubeGridPuzzle] Starting at world position {transform.position}", this);
        BuildCave();
        BuildGrid();
        EvaluateSolvedState();
    }

    private void Update()
    {
        if (_playerTrapped || _isSolved)
        {
            return;
        }

        if (IsPlayerInsideCave())
        {
            _playerTrapped = true;
            Debug.Log("[CubeGridPuzzle] Player entered cave trigger. Spawning and closing barrier.", this);
            SpawnBarrierRock();
            SetBarrierClosed(true);
        }
    }

    private void BuildCave()
    {
        if (_caveRoot != null)
        {
            return;
        }

        if (rockPrefab == null)
        {
            rockPrefab = TryLoadEditorRockPrefab();
            if (rockPrefab == null)
            {
                Debug.LogError("[CubeGridPuzzle] rockPrefab is not assigned. Cave will not spawn.", this);
                return;
            }
        }

        Debug.Log($"[CubeGridPuzzle] Building cave from prefab '{rockPrefab.name}'.", this);

        GameObject caveRootObject = new GameObject("PuzzleCave");
        caveRootObject.transform.SetParent(transform, false);
        caveRootObject.transform.localPosition = Vector3.zero;
        caveRootObject.transform.localRotation = Quaternion.identity;
        _caveRoot = caveRootObject.transform;

        GameObject caveLightObject = new GameObject("CaveLight");
        caveLightObject.transform.SetParent(_caveRoot, false);
        caveLightObject.transform.localPosition = new Vector3(0f, 3.2f, 0.4f);

        Light caveLight = caveLightObject.AddComponent<Light>();
        caveLight.type = LightType.Point;
        caveLight.color = new Color(0.5f, 0.65f, 0.9f);
        caveLight.intensity = 1.2f;
        caveLight.range = 12f;
        caveLight.shadows = LightShadows.None;

        SpawnRock(
            "CaveBack",
            new Vector3(0f, 1.4f, 6.4f),
            new Vector3(-12f, 180f, 4f),
            new Vector3(1.55f, 1f, 0.8f),
            new Vector3(7f, 12f, 2.4f));

        SpawnRock(
            "CaveLeft",
            new Vector3(-6.9f, 1.25f, 1.2f),
            new Vector3(-8f, 118f, 10f),
            new Vector3(1.15f, 1.08f, 0.7f),
            new Vector3(7f, 12f, 2.4f));

        SpawnRock(
            "CaveRight",
            new Vector3(6.9f, 1.25f, 1.2f),
            new Vector3(-8f, -118f, -10f),
            new Vector3(1.15f, 1.08f, 0.7f),
            new Vector3(7f, 12f, 2.4f));

        SpawnRock(
            "CaveFrontLeft",
            new Vector3(-4.7f, 1.3f, -4.9f),
            new Vector3(-6f, 52f, 8f),
            new Vector3(1.02f, 1.02f, 0.62f),
            new Vector3(7f, 12f, 2.4f));

        SpawnRock(
            "CaveFrontRight",
            new Vector3(4.7f, 1.3f, -4.9f),
            new Vector3(-6f, -52f, -8f),
            new Vector3(1.02f, 1.02f, 0.62f),
            new Vector3(7f, 12f, 2.4f));

        SpawnRock(
            "CaveRearLeft",
            new Vector3(-4.4f, 1.6f, 5.8f),
            new Vector3(-10f, 145f, 14f),
            new Vector3(1.05f, 1.12f, 0.65f),
            new Vector3(7f, 12f, 2.4f));

        SpawnRock(
            "CaveRearRight",
            new Vector3(4.4f, 1.6f, 5.8f),
            new Vector3(-10f, -145f, -14f),
            new Vector3(1.05f, 1.12f, 0.65f),
            new Vector3(7f, 12f, 2.4f));

        _barrierOpenLocalPosition = new Vector3(7.3f, 1f, -5f);
        _barrierClosedLocalPosition = new Vector3(0f, 1f, -8.2f);
        _barrierLocalEulerAngles = new Vector3(0f, 180f, 0f);
        _barrierLocalScale = new Vector3(1.05f, 1.15f, 0.48f);
        _barrierColliderSize = new Vector3(6f, 10f, 2.3f);

        Debug.Log($"[CubeGridPuzzle] Cave built. Barrier open at {_barrierOpenLocalPosition}, closed at {_barrierClosedLocalPosition}.", this);
    }

    private void BuildGrid()
    {
        if (_tiles.Count > 0)
        {
            return;
        }

        float centerOffset = (GridSize - 1) * TileSpacing * 0.5f;

        for (int row = 0; row < GridSize; row++)
        {
            for (int column = 0; column < GridSize; column++)
            {
                GameObject tileObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
                tileObject.name = $"PuzzleCube_{row}_{column}";
                tileObject.transform.SetParent(transform, false);
                tileObject.transform.localPosition = new Vector3(
                    column * TileSpacing - centerOffset,
                    0f,
                    row * TileSpacing - centerOffset);
                tileObject.transform.localScale = Vector3.one * TileSize;
                tileObject.layer = gameObject.layer;

                Renderer cubeRenderer = tileObject.GetComponent<Renderer>();
                cubeRenderer.material.color = new Color(0.7f, 0.74f, 0.8f);

                BoxCollider tileCollider = tileObject.GetComponent<BoxCollider>();

                GameObject markerObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
                markerObject.name = "GoalFace";
                markerObject.transform.SetParent(tileObject.transform, false);
                markerObject.transform.localPosition = new Vector3(0f, 0f, 0.49f);
                markerObject.transform.localScale = new Vector3(0.72f, 0.72f, 0.06f);

                Renderer markerRenderer = markerObject.GetComponent<Renderer>();
                markerRenderer.material.color = new Color(0.95f, 0.45f, 0.2f);

                Collider markerCollider = markerObject.GetComponent<Collider>();
                if (markerCollider != null)
                {
                    Destroy(markerCollider);
                }

                PuzzleCubeTile tile = tileObject.AddComponent<PuzzleCubeTile>();
                tile.Initialize(this, markerObject.transform, RotateDuration);
                tile.ConfigureInteraction(tileCollider);
                tile.SetRotation((row * GridSize + column) % 4);
                tile.UpdateSolvedVisual(transform.up);
                _tiles.Add(tile);
            }
        }
    }

    internal void NotifyTileChanged()
    {
        for (int i = 0; i < _tiles.Count; i++)
        {
            _tiles[i].UpdateSolvedVisual(transform.up);
        }

        EvaluateSolvedState();
    }

    private void EvaluateSolvedState()
    {
        bool solved = true;

        for (int i = 0; i < _tiles.Count; i++)
        {
            if (!_tiles[i].IsShowingGoalFace(transform.up))
            {
                solved = false;
                break;
            }
        }

        if (solved && !_isSolved)
        {
            _isSolved = true;
            Debug.Log("[CubeGridPuzzle] Puzzle solved. Opening barrier.", this);
            SetBarrierClosed(false);
            StartCoroutine(ShakeAllTiles());
            return;
        }

        if (!solved)
        {
            if (_isSolved)
            {
                Debug.Log("[CubeGridPuzzle] Puzzle left solved state.", this);
            }

            _isSolved = false;
        }
    }

    private IEnumerator ShakeAllTiles()
    {
        if (_isCelebrating)
        {
            yield break;
        }

        _isCelebrating = true;

        Dictionary<PuzzleCubeTile, Vector3> startPositions = new Dictionary<PuzzleCubeTile, Vector3>();
        for (int i = 0; i < _tiles.Count; i++)
        {
            startPositions[_tiles[i]] = _tiles[i].transform.localPosition;
        }

        float elapsed = 0f;
        while (elapsed < ShakeDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / ShakeDuration;
            float damper = 1f - Mathf.Clamp01(t);

            for (int i = 0; i < _tiles.Count; i++)
            {
                PuzzleCubeTile tile = _tiles[i];
                Vector3 startPosition = startPositions[tile];
                float x = Mathf.Sin((t * 35f) + i) * ShakeStrength * damper;
                float y = Mathf.Cos((t * 42f) + (i * 0.4f)) * ShakeStrength * 0.35f * damper;
                tile.transform.localPosition = startPosition + new Vector3(x, y, 0f);
            }

            yield return null;
        }

        for (int i = 0; i < _tiles.Count; i++)
        {
            PuzzleCubeTile tile = _tiles[i];
            tile.transform.localPosition = startPositions[tile];
        }

        _isCelebrating = false;
    }

    private bool IsPlayerInsideCave()
    {
        Camera mainCamera = Camera.main;
        if (mainCamera == null)
        {
            if (!_hasLoggedCameraDiscovery)
            {
                Debug.LogWarning("[CubeGridPuzzle] Camera.main is null, cave trigger cannot evaluate player entry.", this);
                _hasLoggedCameraDiscovery = true;
            }

            return false;
        }

        if (!_hasLoggedCameraDiscovery)
        {
            Debug.Log($"[CubeGridPuzzle] Using camera '{mainCamera.name}' for cave trigger checks.", this);
            _hasLoggedCameraDiscovery = true;
        }

        Vector3 localCameraPosition = transform.InverseTransformPoint(mainCamera.transform.position);
        Vector3 triggerCenter = new Vector3(0f, 1.3f, -0.2f);
        Vector3 triggerSize = new Vector3(12f, 8f, 12f);
        Vector3 halfSize = triggerSize * 0.5f;
        Vector3 delta = localCameraPosition - triggerCenter;

        bool insideTrigger = Mathf.Abs(delta.x) <= halfSize.x
            && Mathf.Abs(delta.y) <= halfSize.y
            && Mathf.Abs(delta.z) <= halfSize.z;

        if (insideTrigger)
        {
            Debug.Log($"[CubeGridPuzzle] Player is inside cave trigger at local position {localCameraPosition}.", this);
        }

        return insideTrigger;
    }

    private GameObject ResolveRockPrefab()
    {
        if (rockPrefab == null)
        {
            rockPrefab = TryLoadEditorRockPrefab();
            if (rockPrefab == null)
            {
                return null;
            }
        }

        GameObject prefabAsGameObject = rockPrefab as GameObject;
        if (prefabAsGameObject != null)
        {
            return prefabAsGameObject;
        }

        Component prefabAsComponent = rockPrefab as Component;
        if (prefabAsComponent != null)
        {
            return prefabAsComponent.gameObject;
        }

        Debug.LogError($"[CubeGridPuzzle] rockPrefab '{rockPrefab.name}' is not a GameObject or Component.", this);
        return null;
    }

    private Object TryLoadEditorRockPrefab()
    {
#if UNITY_EDITOR
        const string defaultRockPath = "Assets/Project/Environment/ProceduralRocks/pathofanu_rock_1_0_bakedHDA.prefab";
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(defaultRockPath);
        if (prefab != null)
        {
            Debug.Log($"[CubeGridPuzzle] Auto-loaded rock prefab from '{defaultRockPath}'.", this);
            return prefab;
        }
#endif
        return null;
    }

    private Transform SpawnRock(
        string rockName,
        Vector3 localPosition,
        Vector3 localEulerAngles,
        Vector3 localScale,
        Vector3 colliderSize)
    {
        GameObject sourcePrefab = ResolveRockPrefab();
        if (sourcePrefab == null)
        {
            Debug.LogError("[CubeGridPuzzle] rockPrefab is not assigned when SpawnRock was called.", this);
            return null;
        }

        Object instantiatedObject = Instantiate((Object)sourcePrefab);
        GameObject rockObject = instantiatedObject as GameObject;
        if (rockObject == null)
        {
            Debug.LogError("[CubeGridPuzzle] Instantiated rock is not a GameObject.", this);
            return null;
        }

        rockObject.name = rockName;
        rockObject.transform.SetParent(_caveRoot, false);

        Transform rockTransform = rockObject.transform;
        rockTransform.localPosition = localPosition;
        rockTransform.localRotation = Quaternion.Euler(localEulerAngles);
        rockTransform.localScale = localScale;

        BoxCollider rockCollider = rockObject.GetComponent<BoxCollider>();
        if (rockCollider == null)
        {
            rockCollider = rockObject.AddComponent<BoxCollider>();
        }

        rockCollider.center = Vector3.zero;
        rockCollider.size = colliderSize;

        return rockTransform;
    }

    private void SetBarrierClosed(bool closed)
    {
        if (_barrierRock == null)
        {
            Debug.LogWarning("[CubeGridPuzzle] Barrier rock is missing. Cannot animate barrier.", this);
            return;
        }

        if (_barrierRoutine != null)
        {
            StopCoroutine(_barrierRoutine);
        }

        Vector3 targetPosition = closed ? _barrierClosedLocalPosition : _barrierOpenLocalPosition;
        Debug.Log($"[CubeGridPuzzle] Moving barrier to {(closed ? "closed" : "open")} position {targetPosition}.", this);
        _barrierRoutine = StartCoroutine(AnimateBarrier(targetPosition));
    }

    private void SpawnBarrierRock()
    {
        if (_barrierRock != null)
        {
            return;
        }

        _barrierRock = SpawnRock(
            "CaveBarrier",
            _barrierClosedLocalPosition,
            _barrierLocalEulerAngles,
            _barrierLocalScale,
            _barrierColliderSize);

        if (_barrierRock != null)
        {
            Debug.Log("[CubeGridPuzzle] Spawned blocking barrier rock.", this);
        }
    }

    private IEnumerator AnimateBarrier(Vector3 targetPosition)
    {
        Vector3 startPosition = _barrierRock.localPosition;
        float elapsed = 0f;

        while (elapsed < BarrierMoveDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / BarrierMoveDuration);
            t = t * t * (3f - (2f * t));
            _barrierRock.localPosition = Vector3.Lerp(startPosition, targetPosition, t);
            yield return null;
        }

        _barrierRock.localPosition = targetPosition;
        _barrierRoutine = null;
    }
}

public class PuzzleCubeTile : MonoBehaviour
{
    private CubeGridPuzzle _puzzle;
    private Transform _goalFace;
    private float _rotateDuration;
    private Coroutine _rotateRoutine;
    private int _quarterTurns;
    private RayInteractable _rayInteractable;
    private Light _solvedLight;

    internal void Initialize(CubeGridPuzzle puzzle, Transform goalFace, float rotateDuration)
    {
        _puzzle = puzzle;
        _goalFace = goalFace;
        _rotateDuration = rotateDuration;
    }

    internal void ConfigureInteraction(BoxCollider tileCollider)
    {
        ColliderSurface colliderSurface = gameObject.AddComponent<ColliderSurface>();
        colliderSurface.InjectAllColliderSurface(tileCollider);

        _rayInteractable = gameObject.AddComponent<RayInteractable>();
        _rayInteractable.InjectAllRayInteractable(colliderSurface);
        _rayInteractable.WhenSelectingInteractorViewAdded += HandleSelect;

        _solvedLight = gameObject.AddComponent<Light>();
        _solvedLight.type = LightType.Point;
        _solvedLight.color = new Color(0.45f, 0.7f, 1f);
        _solvedLight.intensity = 0.6f;
        _solvedLight.range = 1.8f;
        _solvedLight.shadows = LightShadows.None;
        _solvedLight.enabled = false;
    }

    internal void SetRotation(int quarterTurns)
    {
        _quarterTurns = Mathf.RoundToInt(Mathf.Repeat(quarterTurns, 4));
        transform.localRotation = Quaternion.Euler(_quarterTurns * 90f, 0f, 0f);
    }

    internal bool IsShowingGoalFace(Vector3 puzzleForward)
    {
        return Vector3.Dot(_goalFace.forward, puzzleForward.normalized) > 0.98f;
    }

    internal void UpdateSolvedVisual(Vector3 puzzleForward)
    {
        if (_solvedLight != null)
        {
            _solvedLight.enabled = IsShowingGoalFace(puzzleForward);
        }
    }

    private void OnDestroy()
    {
        if (_rayInteractable != null)
        {
            _rayInteractable.WhenSelectingInteractorViewAdded -= HandleSelect;
        }
    }

    private void HandleSelect(IInteractorView interactorView)
    {
        if (!enabled || _rotateRoutine != null)
        {
            return;
        }

        _rotateRoutine = StartCoroutine(RotateRoutine());
    }

    private IEnumerator RotateRoutine()
    {
        Quaternion startRotation = transform.localRotation;
        _quarterTurns = (_quarterTurns + 1) % 4;
        Quaternion targetRotation = Quaternion.Euler(_quarterTurns * 90f, 0f, 0f);

        float elapsed = 0f;
        while (elapsed < _rotateDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / _rotateDuration);
            transform.localRotation = Quaternion.Slerp(startRotation, targetRotation, t);
            yield return null;
        }

        transform.localRotation = targetRotation;
        _rotateRoutine = null;
        _puzzle.NotifyTileChanged();
    }
}

using System.Collections;
using System.Collections.Generic;
using Oculus.Interaction;
using Oculus.Interaction.Surfaces;
using UnityEngine;

public class CubeGridPuzzle : MonoBehaviour
{
    private const int GridSize = 3;
    private const float TileSize = 1f;
    private const float TileSpacing = 1.35f;
    private const float RotateDuration = 0.2f;
    private const float ShakeDuration = 0.8f;
    private const float ShakeStrength = 0.12f;

    private readonly List<PuzzleCubeTile> _tiles = new List<PuzzleCubeTile>();
    private bool _isSolved;
    private bool _isCelebrating;

    private void Start()
    {
        BuildGrid();
        EvaluateSolvedState();
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
            StartCoroutine(ShakeAllTiles());
            return;
        }

        if (!solved)
        {
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

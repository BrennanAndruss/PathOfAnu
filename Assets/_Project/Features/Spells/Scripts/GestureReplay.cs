using System.Collections;
using System.Collections.Generic;
using _Project.Features.Spells.ScriptableObjects;
using UnityEngine;

namespace _Project.Features.Spells.Scripts
{
    public class GestureReplay : MonoBehaviour
    {
        [SerializeField] private GestureData gestureData;

        [Header("Visual Settings")]
        [SerializeField] private Vector2 targetSize = new Vector2(1f, 1f);
        [SerializeField] [Range(0.01f, 0.1f)] private float strokeWidth = 0.01f;
        [SerializeField] private Color strokeColor = Color.white;
        private Material _strokeMaterial;
        
        [Header("Replay Settings")]
        [SerializeField] private bool drawStatic = false;

        [SerializeField] private float totalDuration = 2.0f;
        [SerializeField] private float loopDelay = 1.0f;
        
        private readonly List<LineRenderer> _lineRenderers = new();
        private List<float[]> _strokeSegmentLengths = new();
        private List<Vector3[]> _localPositions = new();
        private float _totalGestureLength = 0f;

        private void Start()
        {
            if (gestureData == null)
            {
                Debug.Log("No gesture data assigned.");
                return;
            }
            
            _strokeMaterial = new Material(Shader.Find("Sprites/Default"));
            
            if (drawStatic)
            {
                PreallocateStrokes();
                RenderGestureStatic();
            }
            else
            {
                PreallocateStrokes();
                CalculatePathLengths();
                StartCoroutine(RenderGestureLoop());
            }
        }
        
        private void RenderGestureStatic()
        {
            for (int si = 0; si < _lineRenderers.Count; si++)
            {
                GesturePoint[] points = gestureData.strokes[si].points;
                LineRenderer line = _lineRenderers[si];
                line.positionCount = points.Length;

                for (int i = 0; i < points.Length; i++)
                {
                    Vector2 targetPos = points[i].Pos * targetSize;
                    line.SetPosition(i, new Vector3(targetPos.x, targetPos.y, 0f));
                }
            }
        }

        private void PreallocateStrokes()
        {
            for (int i = 0; i < gestureData.strokes.Count; i++)
            {
                GameObject strokeObj = new GameObject("SpellStroke");
                strokeObj.transform.SetParent(transform);
                strokeObj.transform.localPosition = Vector3.zero;
                strokeObj.transform.localRotation = Quaternion.identity;

                LineRenderer line = strokeObj.AddComponent<LineRenderer>();
                line.material = _strokeMaterial;
                line.startColor = line.endColor = strokeColor;
                line.startWidth = line.endWidth = strokeWidth;
                line.useWorldSpace = false;
                line.positionCount = 0;
                
                _lineRenderers.Add(line);
            }
        }

        private void CalculatePathLengths()
        {
            foreach (var stroke in gestureData.strokes)
            {
                int count = stroke.points.Length;
                float[] lengths = new float[count];
                Vector3[] localPositions = new Vector3[count];
                
                // Process the first point
                lengths[0] = 0f;
                Vector2 scaledPos = stroke.points[0].Pos * targetSize;
                localPositions[0] = new Vector3(scaledPos.x, scaledPos.y, 0f);

                for (int i = 1; i < stroke.points.Length; i++)
                {
                    scaledPos = stroke.points[i].Pos * targetSize;
                    localPositions[i] = new Vector3(scaledPos.x, scaledPos.y, 0f);
                    
                    lengths[i] = Vector2.Distance(localPositions[i - 1], localPositions[i]);
                    _totalGestureLength += lengths[i];
                }
                
                _strokeSegmentLengths.Add(lengths);
                _localPositions.Add(localPositions);
            }

            if (_totalGestureLength <= 0f)
                _totalGestureLength = 0.001f;
        }

        private IEnumerator RenderGestureLoop()
        {
            while (true)
            {
                // Clear previous drawings
                ResetLines();
                
                // Animate over fixed duration timeline
                float elapsedTime = 0f;
                while (elapsedTime < totalDuration)
                {
                    elapsedTime += Time.deltaTime;
                    float globalProgress = Mathf.Clamp01(elapsedTime / totalDuration);
                    float currentTargetDistance = globalProgress * _totalGestureLength;
                    
                    // Construct/extend line renderers up to current target distance
                    UpdateDrawingToDistance(currentTargetDistance);
                    yield return null;
                }
                
                // Snap to last point at end of timeline
                UpdateDrawingToDistance(_totalGestureLength);
                
                yield return new WaitForSeconds(loopDelay);
            }
        }

        private void UpdateDrawingToDistance(float targetDistance)
        {
            float accumulatedDistance = 0f;

            for (int si = 0; si < gestureData.strokes.Count; si++)
            {
                float[] lengths = _strokeSegmentLengths[si];
                Vector3[] localPositions = _localPositions[si];
                LineRenderer line = _lineRenderers[si];
                
                List<Vector3> activePositions = new() { localPositions[0] };

                for (int i = 1; i < localPositions.Length; i++)
                {
                    float segmentLength = lengths[i];
                    Vector2 startPos = localPositions[i - 1];
                    Vector2 endPos = localPositions[i];
                    
                    // Add the entire segment if fully contained
                    if (accumulatedDistance + segmentLength <= targetDistance)
                    {
                        activePositions.Add(endPos);
                        accumulatedDistance += segmentLength;
                    }
                    else
                    {
                        // Interpolate the current segment up to the current timeline
                        float remainingDistanceNeeded = targetDistance - accumulatedDistance;
                        float segmentT = remainingDistanceNeeded / Mathf.Max(0.001f, segmentLength);

                        Vector3 currentLerpPos = Vector3.Lerp(startPos, endPos, segmentT);
                        activePositions.Add(currentLerpPos);
                
                        accumulatedDistance = targetDistance;
                        break;
                    }
                }
                
                line.positionCount = activePositions.Count;
                line.SetPositions(activePositions.ToArray());

                // Clear any future lines if threshold destination is reached
                if (accumulatedDistance >= targetDistance)
                {
                    for (int k = si + 1; k < _lineRenderers.Count; k++)
                    {
                        _lineRenderers[k].positionCount = 0;
                    }
                    
                    // No further strokes should be evaluated
                    break;
                }
            }
        }

        private void ResetLines()
        {
            foreach (var line in _lineRenderers)
            {
                line.positionCount = 0;
            }
        }
    }
}
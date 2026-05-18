using System;
using System.Collections;
using System.Collections.Generic;
using _Project.Features.Spells.ScriptableObjects;
using UnityEngine;

namespace _Project.Features.Spells.Scripts
{
    public class GestureReplay : MonoBehaviour
    {
        [SerializeField] private GestureData gestureData;
        [SerializeField] private Vector2 targetSize = new Vector2(1f, 1f);

        [SerializeField] [Range(0.01f, 0.1f)] private float strokeWidth = 0.01f;
        [SerializeField] private Color strokeColor = Color.white;
        private Material _strokeMaterial;
        
        [SerializeField] private bool drawStatic = false;
        [SerializeField] private float drawSpeed = 5f;
        [SerializeField] private float strokeDelay = 0.2f;
        [SerializeField] private float loopDelay = 1.0f;
        
        private LineRenderer _staticStroke;
        private LineRenderer _currentStroke;
        private readonly List<GameObject> _strokeObjs = new();

        private void Start()
        {
            _strokeMaterial = new Material(Shader.Find("Sprites/Default"));
            
            if (drawStatic)
            {
                _staticStroke = gameObject.AddComponent<LineRenderer>();
                _staticStroke.material = new Material(Shader.Find("Sprites/Default"));
                _staticStroke.startColor = _staticStroke.endColor = strokeColor;
                _staticStroke.startWidth = _staticStroke.endWidth = strokeWidth;
                _staticStroke.useWorldSpace = false;
                
                RenderGestureStatic();
            }
            else
            {
                StartCoroutine(RenderGestureLoop());
            }
        }
        
        private void RenderGestureStatic()
        {
            GesturePoint[] points = gestureData.Points;
            _staticStroke.positionCount = points.Length;

            for (int i = 0; i < points.Length; i++)
            {
                // Map normalized coordinates target size
                Vector2 targetPos = points[i].Pos * targetSize;
                Vector3 localPos = new Vector3(targetPos.x, targetPos.y, 0f);
                _staticStroke.SetPosition(i, localPos);
            }
        }

        private IEnumerator RenderGestureLoop()
        {
            while (true)
            {
                yield return StartCoroutine(RenderGestureAnimated());
                yield return new WaitForSeconds(loopDelay);
                ClearStrokes();
            }
        }

        private IEnumerator RenderGestureAnimated()
        {
            foreach (var stroke in gestureData.strokes)
            {
                // Start new stroke
                GameObject strokeObj = new GameObject("SpellStroke");
                strokeObj.transform.SetParent(transform);
                strokeObj.transform.localPosition = Vector3.zero;
                strokeObj.transform.localRotation = Quaternion.identity;
                
                _currentStroke = strokeObj.AddComponent<LineRenderer>();
                _currentStroke.material = _strokeMaterial;
                _currentStroke.startColor = _currentStroke.endColor = strokeColor;
                _currentStroke.startWidth = _currentStroke.endWidth = strokeWidth;
                _currentStroke.useWorldSpace = false;
                
                // Track stroke object for cleanup
                _strokeObjs.Add(strokeObj);

                GesturePoint[] points = stroke.points;
                
                // Initialize the line with the first point
                // Map normalized coordinates target size
                _currentStroke.positionCount = 1;
                Vector2 targetPos = points[0].Pos * targetSize;
                Vector3 localPos = new Vector3(targetPos.x, targetPos.y, 0);
                _currentStroke.SetPosition(0, localPos);
                
                // Animate through the remaining points
                for (int i = 1; i < points.Length; i++)
                {
                    _currentStroke.positionCount++;
                    Vector2 startPos = points[i - 1].Pos * targetSize;
                    Vector2 endPos = points[i].Pos * targetSize;

                    float t = 0f;
                    while (t < 1f)
                    {
                        t += Time.deltaTime * drawSpeed * 1000;
                        
                        // Interpolate between the last point and the current target point
                        Vector2 currentLerpPos = Vector2.Lerp(startPos, endPos, t);
                        localPos = new Vector3(currentLerpPos.x, currentLerpPos.y, 0f);
                        
                        // Continually update the tip of the line renderer
                        _currentStroke.SetPosition(i, localPos);

                        yield return null;
                    }
                    
                    // Snap to final point
                    localPos = new Vector3(endPos.x, endPos.y, 0f);
                    _currentStroke.SetPosition(i, localPos);
                }
                
                // Pause to simulate player lifting their hand
                yield return new WaitForSeconds(strokeDelay);
            }
        }

        private void ClearStrokes()
        {
            foreach (var obj in _strokeObjs)
            {
                if (obj != null) Destroy(obj);
            }

            _strokeObjs.Clear();
        }
    }
}
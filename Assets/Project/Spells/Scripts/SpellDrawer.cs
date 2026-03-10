using System;
using System.Collections.Generic;
using UnityEngine;

namespace Project.Spells.Scripts
{
    public class SpellDrawer : MonoBehaviour
    {
        [Header("Wand Properties")] 
        [SerializeField] private Transform tip;
        [SerializeField] [Range(0.01f, 0.1f)] private float strokeWidth = 0.01f;
        private Color _strokeColor = Color.purple;
        
        [Header("Multi-Stroke Settings")] 
        [Tooltip("Time to wait for next stroke")] 
        [SerializeField] private float completionDelay = 0.8f;
        
        // Projection data
        private Vector3 _startPosition;
        private Vector3 _startForward;
        
        // Stroke data
        private int _numStrokes = 0;
        private LineRenderer _currentStroke;
        private readonly List<GameObject> _strokeObjs = new();
        private readonly List<WorldPoint> _allStrokePoints = new();
        private int _index;
        
        public Action<List<WorldPoint>> OnSpellDrawn;

        public void HandleDraw(bool triggerDown, bool triggerHeld, bool triggerUp)
        {
            if (triggerDown)
            {
                Debug.Log("[SpellDrawer] Stroke start");
                StartNewStroke();
            }

            if (triggerHeld)
            {
                UpdateCurrentStroke();
            }

            if (triggerUp)
            {
                Debug.Log("[SpellDrawer] Stroke end");
                _numStrokes++;
                _currentStroke = null;
            }
        }

        public void RequestFinalize()
        {
            // Ignore accidental clicks
            if (_allStrokePoints.Count < 5)
            {
                return;
            }

            FinalizeSpell();
        }

        private void StartNewStroke()
        {
            _index = 0;
            GameObject strokeObj = new GameObject("SpellStroke");
            _currentStroke = strokeObj.AddComponent<LineRenderer>();

            _currentStroke.material = new Material(Shader.Find("Sprites/Default"));
            _currentStroke.startColor = _currentStroke.endColor = _strokeColor;
            _currentStroke.startWidth = _currentStroke.endWidth = strokeWidth;
            _currentStroke.useWorldSpace = true;

            // Track stroke objects for cleanup
            _strokeObjs.Add(strokeObj);

            // Record the first point
            Vector3 point = tip.position;
            _currentStroke.positionCount = 1;
            _currentStroke.SetPosition(0, point); 
        }

        private void UpdateCurrentStroke()
        {
            Vector3 point = tip.position;
            Vector3 lastPoint = _currentStroke.GetPosition(_index);
            
            if (Vector3.Distance(lastPoint, point) > 0.01f)
            {
                _index++;
                _currentStroke.positionCount = _index + 1;
                _currentStroke.SetPosition(_index, point);

                // Index _allStrokePoints from the end
                _allStrokePoints.Add(new WorldPoint(point, _numStrokes));
            }
        }

        private void FinalizeSpell()
        {
            // Send RawStrokePoints to WandManager
            OnSpellDrawn?.Invoke(_allStrokePoints);

            // Cleanup stroke objects
            foreach (var obj in _strokeObjs)
            {
                Destroy(obj);
            }

            // Reset state for the next spell
            _numStrokes = 0;
            _strokeObjs.Clear();
            _allStrokePoints.Clear();
        }
    }
}

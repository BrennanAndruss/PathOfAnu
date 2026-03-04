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
        private Color _strokeColor;
        
        [Header("Multi-Stroke Settings")] 
        [Tooltip("Time to wait for next stroke")] 
        [SerializeField] private float completionDelay = 0.8f;
        
        // Stroke data
        private LineRenderer _currentStroke;
        private readonly List<GameObject> _strokeObjs = new();
        private readonly List<List<Vector3>> _activeSpellPoints = new();
        private int _index;
        
        private float _lastStrokeTime;
        private bool _isWaitingForNextStroke;
        
        public Action<SpellType> OnDrawingComplete;

        private void Start()
        {
            _strokeColor = Color.purple;
        }

        private void Update()
        {
            // Check if the spell is finished
            if (_isWaitingForNextStroke && (Time.time - _lastStrokeTime > completionDelay))
            {
                _isWaitingForNextStroke = false;
                FinalizeSpell();
            }
        }

        public void HandleDraw(bool triggerDown, bool triggerHeld, bool triggerUp)
        {
            if (triggerDown)
            {
                Debug.Log("[SpellDrawer] Stroke start");
                _isWaitingForNextStroke = false;
                StartNewStroke();
            }

            if (triggerHeld)
            {
                UpdateCurrentStroke();
            }

            if (triggerUp)
            {
                Debug.Log("[SpellDrawer] Stroke end");
                _currentStroke = null;
                _lastStrokeTime = Time.time;
                _isWaitingForNextStroke = true;
            }
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

            // Create a new list for this stroke's data
            _activeSpellPoints.Add(new List<Vector3>());

            // Record the first point
            Vector3 point = tip.position;
            _currentStroke.positionCount = 1;
            _currentStroke.SetPosition(0, point);
            _activeSpellPoints[^1].Add(point); // Index new stroke from end
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

                _activeSpellPoints[^1].Add(point);
            }
        }

        private void FinalizeSpell()
        {
            // Send _activeSpellPoints to SpellRecognizer...
            // SpellType spellType = SpellRecognizer.Recognize(_activeSpellPoints);
            SpellType spellType = SpellType.Prototype;
            OnDrawingComplete?.Invoke(spellType);

            // Cleanup stroke objects
            foreach (var obj in _strokeObjs)
            {
                Destroy(obj);
            }

            // Reset state for the next spell
            _strokeObjs.Clear();
            _activeSpellPoints.Clear();
        }
    }
}

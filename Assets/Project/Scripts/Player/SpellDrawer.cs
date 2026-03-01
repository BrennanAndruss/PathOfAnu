using System;
using System.Collections.Generic;
using UnityEngine;
using Spells;

namespace Player
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
        private float _lastStrokeTime;
        private bool _isWaitingForStrokes;

        private LineRenderer _currentStroke;
        private int _index;

        private readonly List<GameObject> _strokeObjs = new();
        private readonly List<List<Vector3>> _activeSpellPoints = new();
        
        public Action<SpellType> OnSpellRecognized;

        private void Start()
        {
            _strokeColor = Color.purple;
        }

        private void Update()
        {
            bool triggerHeld = OVRInput.Get(OVRInput.Button.SecondaryIndexTrigger);
            bool triggerDown = OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger);
            bool triggerUp = OVRInput.GetUp(OVRInput.Button.SecondaryIndexTrigger);

            if (triggerDown)
            {
                Debug.Log("[SpellDrawer] Stroke start");
                _isWaitingForStrokes = false;
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
                _isWaitingForStrokes = true;
            }

            // Check if the spell is finished
            if (_isWaitingForStrokes && (Time.time - _lastStrokeTime > completionDelay))
            {
                FinalizeSpell();
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
            _isWaitingForStrokes = false;

            // Send _activeSpellPoints to SpellRecognizer...
            // SpellType result = SpellRecognizer.Recognize(_activeSpellPoints);
            SpellType spellType = SpellType.Prototype;

            if (spellType != SpellType.Unknown)
            {
                OnSpellRecognized?.Invoke(spellType);
            }

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

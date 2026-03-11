using System;
using System.Collections.Generic;
using UnityEngine;

namespace Project.Spells.Scripts
{
    public enum WandState
    {
        Idle,
        Drawing,
        Processing,
        Chambered,
    }
    
    public class WandManager : MonoBehaviour
    {
        private WandState _wandState;

        // Projection variables
        [SerializeField] private Transform centerEyeAnchor;
        private Vector3 _projectionOrigin;
        private Quaternion _projectionRotation;
        
        // Spell script references
        private SpellDrawer _drawer;
        private SpellRecognizer _recognizer;
        private SpellCaster _caster;
        
#if UNITY_EDITOR
        [HideInInspector] public GesturePoint[] CapturedProjectedPoints;
#endif

        private void Awake()
        {
            _wandState = WandState.Idle;
            _drawer = GetComponent<SpellDrawer>();
            _recognizer = GetComponent<SpellRecognizer>();
            _caster = GetComponent<SpellCaster>();
        }

        private void OnEnable()
        {
            _drawer.OnSpellDrawn += HandleSpellDrawn;
            _recognizer.OnSpellRecognized += HandleSpellRecognized;
            _caster.OnSpellCasted += HandleSpellCasted;
#if UNITY_EDITOR
            _drawer.OnSpellTemplateDrawn += HandleSpellTemplateDrawn;
#endif
        }

        private void OnDisable()
        {
            _drawer.OnSpellDrawn -= HandleSpellDrawn;
            _recognizer.OnSpellRecognized -= HandleSpellRecognized;
            _caster.OnSpellCasted -= HandleSpellCasted;
#if UNITY_EDITOR
            _drawer.OnSpellTemplateDrawn -= HandleSpellTemplateDrawn;
#endif
        }

        private void Update()
        {
            bool indexDown = OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger);
            bool indexHeld = OVRInput.Get(OVRInput.Button.SecondaryIndexTrigger);
            bool indexUp = OVRInput.GetUp(OVRInput.Button.SecondaryIndexTrigger);

            if (indexDown) Debug.Log("index pressed");
            if (indexDown) Debug.Log(_wandState);

            bool handDown = OVRInput.GetDown(OVRInput.Button.SecondaryHandTrigger);
#if UNITY_EDITOR
            bool leftHandDown = OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger);
#endif

            switch (_wandState)
            {
                case WandState.Idle:
                    if (indexDown)
                    {
                        _projectionOrigin = centerEyeAnchor.position;
                        _projectionRotation = centerEyeAnchor.rotation;
                        
                        _wandState = WandState.Drawing;
                        _drawer.HandleDraw(true, indexHeld, indexUp);
                    }
                    break;
                
                case WandState.Drawing:
                    // Continue drawing on indexHeld
                    _drawer.HandleDraw(indexDown, indexHeld, indexUp);
                    
                    // Finish spell with handTriggerDown
                    if (handDown)
                    {
                        Debug.Log("do recognize");
                        _drawer.RequestFinalize();
                    }
#if UNITY_EDITOR
                    else if (leftHandDown)
                    {
                        _drawer.RequestRecord();
                    }
#endif
                    break;
                
                case WandState.Chambered:
                    if (indexDown)
                    {
                        Debug.Log("do cast");
                        _caster.CastSpell();
                    }
                    break;
            }
        }

        private void HandleSpellDrawn(List<WorldPoint> rawPoints)
        {
            Debug.Log("[WandManager] Spell drawn");
            _wandState = WandState.Processing;
            
            GesturePoint[] projectedSpellPoints = ProjectPoints(rawPoints);
            _drawer.ClearSpell();
            _recognizer.RecognizeSpell(projectedSpellPoints);
        }
        
#if UNITY_EDITOR
        private void HandleSpellTemplateDrawn(List<WorldPoint> rawPoints)
        {
            CapturedProjectedPoints = ProjectPoints(rawPoints);
            _wandState = WandState.Processing;
            
            Debug.Log("<color=green>Spell template buffered!</color> Ready to save in the Inspector.");
        }

        public void ClearCapturedGesture()
        {
            CapturedProjectedPoints = null;
            _drawer.ClearSpell();
            _wandState = WandState.Idle;
        }
#endif

        private void HandleSpellRecognized(SpellType spellType)
        {
            if (spellType != SpellType.Unknown)
            {
                Debug.Log("[WandManager] Spell recognized");
                _caster.PrepareSpell(spellType);
                Debug.Log(_wandState);
                _wandState = WandState.Chambered;
                Debug.Log(_wandState);
            }
            else
            {
                _wandState = WandState.Idle;
                Debug.Log("[WandManager] Spell not recognized");
            }
        }

        private void HandleSpellCasted()
        {
            _wandState = WandState.Idle;
        }

        private GesturePoint[] ProjectPoints(List<WorldPoint> rawPoints)
        {
            var projectedSpellPoints = new GesturePoint[rawPoints.Count];
            
            // Get inverse rotation to undo head tilt
            Quaternion invRotation = Quaternion.Inverse(_projectionRotation);
            
            // Project points to player view
            for (int i = 0; i < rawPoints.Count; i++)
            {
                var rawPoint = rawPoints[i];
                
                // Get the vector from eye to point
                Vector3 worldDelta = rawPoint.Position - _projectionOrigin;
                
                // Rotate the vector into local view space
                Vector3 localPoint = invRotation * worldDelta;
                
                // Project to the XY plane by ignoring depth from Z
                projectedSpellPoints[i] = new GesturePoint(localPoint.x, localPoint.y, rawPoint.StrokeId);
            }

            return projectedSpellPoints;
        }
    }
}
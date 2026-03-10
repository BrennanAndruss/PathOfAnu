using System.Collections.Generic;
using UnityEngine;

namespace Project.Spells.Scripts
{
    public enum WandState
    {
        Idle,
        Drawing,
        Chambered,
    }
    
    public class WandManager : MonoBehaviour
    {
        private WandState _wandState = WandState.Idle;

        // Projection variables
        [SerializeField] private Transform centerEyeAnchor;
        private Vector3 _projectionOrigin;
        private Quaternion _projectionRotation;
        
        [Header("References")] 
        [SerializeField] private SpellDrawer drawer;
        [SerializeField] private SpellRecognizer recognizer;
        [SerializeField] private SpellCaster caster;

        private void OnEnable()
        {
            drawer.OnSpellDrawn += HandleSpellDrawn;
            recognizer.OnSpellRecognized += HandleSpellRecognized;
            caster.OnSpellCasted += HandleSpellCasted;
        }

        private void OnDisable()
        {
            drawer.OnSpellDrawn -= HandleSpellDrawn;
            recognizer.OnSpellRecognized -= HandleSpellRecognized;
            caster.OnSpellCasted -= HandleSpellCasted;
        }

        private void Update()
        {
            bool indexDown = OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger);
            bool indexHeld = OVRInput.Get(OVRInput.Button.SecondaryIndexTrigger);
            bool indexUp = OVRInput.GetUp(OVRInput.Button.SecondaryIndexTrigger);

            bool handTriggerDown = OVRInput.GetDown(OVRInput.Button.SecondaryHandTrigger);

            switch (_wandState)
            {
                case WandState.Idle:
                    if (indexDown)
                    {
                        _projectionOrigin = centerEyeAnchor.position;
                        _projectionRotation = centerEyeAnchor.rotation;
                        
                        _wandState = WandState.Drawing;
                        drawer.HandleDraw(true, indexHeld, indexUp);
                    }
                    break;
                
                case WandState.Drawing:
                    // Continue drawing on indexHeld
                    drawer.HandleDraw(indexDown, indexHeld, indexUp);
                    
                    // Finish spell with handTriggerDown
                    if (handTriggerDown)
                    {
                        drawer.RequestFinalize();
                    }
                    break;
                
                case WandState.Chambered:
                    if (indexDown)
                    {
                        caster.CastSpell();
                    }
                    break;
            }
        }

        private void HandleSpellDrawn(List<WorldPoint> rawSpellPoints)
        {
            var projectedSpellPoints = new GesturePoint[rawSpellPoints.Count];
            
            // Get inverse rotation to undo head tilt
            Quaternion invRotation = Quaternion.Inverse(_projectionRotation);
            
            // Project points to player view
            for (int i = 0; i < rawSpellPoints.Count; i++)
            {
                var rawPoint = rawSpellPoints[i];
                
                // Get the vector from eye to point
                Vector3 worldDelta = rawPoint.Position - _projectionOrigin;
                
                // Rotate the vector into local view space
                Vector3 localPoint = invRotation * worldDelta;
                
                // Project to the XY plane by ignoring depth from Z
                projectedSpellPoints[i] = new GesturePoint(localPoint.x, localPoint.y, rawPoint.StrokeId);
            }
            
            recognizer.RecognizeSpell(projectedSpellPoints);
        }

        private void HandleSpellRecognized(SpellType spellType)
        {
            if (spellType != SpellType.Unknown)
            {
                caster.PrepareSpell(spellType);
                _wandState = WandState.Chambered;
                Debug.Log("[WandManager] Spell recognized");
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
    }
}
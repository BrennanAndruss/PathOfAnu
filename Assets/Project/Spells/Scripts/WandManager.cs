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
        
        [Header("References")] 
        [SerializeField] private SpellDrawer drawer;
        [SerializeField] private SpellCaster caster;

        private void OnEnable()
        {
            drawer.OnDrawingComplete += HandleSpellRecognized;
            caster.OnCastComplete += HandleCastFinished;
        }

        private void OnDisable()
        {
            drawer.OnDrawingComplete -= HandleSpellRecognized;
            caster.OnCastComplete -= HandleCastFinished;
        }

        private void Update()
        {
            bool triggerDown = OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger);
            bool triggerHeld = OVRInput.Get(OVRInput.Button.SecondaryIndexTrigger);
            bool triggerUp = OVRInput.GetUp(OVRInput.Button.SecondaryIndexTrigger);

            switch (_wandState)
            {
                case WandState.Idle:
                    if (triggerDown)
                    {
                        _wandState = WandState.Drawing;
                        drawer.HandleDraw(true, triggerHeld, triggerUp);
                    }
                    break;
                
                case WandState.Drawing:
                    drawer.HandleDraw(triggerDown, triggerHeld, triggerUp);
                    break;
                
                case WandState.Chambered:
                    if (triggerDown)
                    {
                        caster.CastSpell();
                    }
                    break;
            }
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

        private void HandleCastFinished()
        {
            _wandState = WandState.Idle;
        }
    }
}
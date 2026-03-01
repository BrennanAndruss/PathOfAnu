using UnityEngine;
using Spells;

namespace Player
{
    public class SpellCaster : MonoBehaviour
    {
        [SerializeField] private SpellDrawer spellDrawer;
        
        [Header("Wand Properties")] 
        [SerializeField] private Transform tip;
        
        private GameObject _chamberedSpell;
        public GameObject prototypeSpellPrefab;

        private void Update()
        {
            if (_chamberedSpell && OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger))
            {
                FireSpell();
            }
        }

        private void OnEnable()
        {
            // spellDrawer.OnSpellRecognized += HandleNewSpell;
        }

        private void OnDisable()
        {
            // spellDrawer.OnSpellRecognized -= HandleNewSpell;
        }

        private void HandleNewSpell(SpellType spellType)
        {
            spellDrawer.enabled = false;
            _chamberedSpell = Instantiate(prototypeSpellPrefab, tip.position, tip.rotation);
            _chamberedSpell.transform.SetParent(tip);
        }

        private void FireSpell()
        {
            spellDrawer.enabled = true;
            _chamberedSpell = null;
        }
    }
}
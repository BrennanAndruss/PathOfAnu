using System;
using UnityEngine;

namespace Project.Spells.Scripts
{
    public class SpellCaster : MonoBehaviour
    {
        [SerializeField] private SpellDrawer spellDrawer;
        
        [Header("Wand Properties")] 
        [SerializeField] private Transform tip;
        [SerializeField] private LineRenderer trajectory;
        
        private GameObject _chamberedSpell;
        private float _launchForce = 10f;

        public Action OnCastComplete;
        
        // temporary projectile for prototyping
        [Space(10)]
        public GameObject prototypeSpellPrefab;

        public void PrepareSpell(SpellType spellType)
        {
            spellDrawer.enabled = false;
            
            _chamberedSpell = Instantiate(prototypeSpellPrefab, tip.position, tip.rotation);
            _chamberedSpell.transform.SetParent(tip);
            trajectory.enabled = true;
        }

        public void CastSpell()
        {
            if (!_chamberedSpell) return;
            
            spellDrawer.enabled = true;
            _chamberedSpell.transform.SetParent(null);
            Rigidbody rb = _chamberedSpell.GetComponent<Rigidbody>();
            rb.AddForce(tip.up * _launchForce, ForceMode.Impulse);

            trajectory.enabled = false;
            _chamberedSpell = null;

            OnCastComplete?.Invoke();
        }
    }
}
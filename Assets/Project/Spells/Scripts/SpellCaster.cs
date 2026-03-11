using System;
using UnityEngine;

namespace Project.Spells.Scripts
{
    public class SpellCaster : MonoBehaviour
    {
        [Header("Wand Properties")] 
        [SerializeField] private Transform tip;
        [SerializeField] private LineRenderer trajectory;
        
        private GameObject _chamberedSpell;
        private float _launchForce = 10f;

        public Action OnSpellCasted;
        
        // temporary projectile for prototyping
        [Space(10)] 
        public GameObject[] spellPrefabs;

        private void Awake()
        {
            trajectory.enabled = false;
        }

        public void PrepareSpell(SpellType spellType)
        {
            GameObject spellPrefab = spellPrefabs[0];
            if (spellType == SpellType.Water)
            {
                spellPrefab = spellPrefabs[0];
            }
            else if (spellType == SpellType.Fire)
            {
                spellPrefab = spellPrefabs[1];
            }
            else if (spellType == SpellType.Electric)
            {
                spellPrefab = spellPrefabs[2];
            }
            
            
            _chamberedSpell = Instantiate(spellPrefab, tip.position, tip.rotation);
            _chamberedSpell.GetComponent<Rigidbody>().useGravity = false;
            _chamberedSpell.transform.SetParent(tip);
            trajectory.enabled = true;
        }

        public void CastSpell()
        {
            if (!_chamberedSpell) return;
            
            _chamberedSpell.transform.SetParent(null);
            Rigidbody rb = _chamberedSpell.GetComponent<Rigidbody>();
            rb.useGravity = true;
            rb.AddForce(tip.up * _launchForce, ForceMode.Impulse);

            trajectory.enabled = false;
            _chamberedSpell = null;

            OnSpellCasted?.Invoke();
        }
    }
}
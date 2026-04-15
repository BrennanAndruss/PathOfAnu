using System;
using System.Collections.Generic;
using _Project.Features.Spells.ScriptableObjects;
using Unity.VisualScripting;
using UnityEngine;

namespace _Project.Features.Spells.Scripts
{
    public class SpellCaster : MonoBehaviour
    {
        [Header("Wand Properties")]
        [SerializeField] private Transform tip;
        [SerializeField] private LineRenderer trajectory;
        
        [Header("Audio")]
        [SerializeField] private AudioSource oneShotSource;   // cast + launch
        [SerializeField] private AudioSource loopSource;      // waiting loop
        // one of these sources should move to Spell / Projectile (moving in the world)
        // the other will be moved to WandManager (for all spell drawing system sounds)

        [SerializeField] private AudioClip castSFX;         // wand sfx
        [SerializeField] private AudioClip projectileSFX;   // spell-specific
        [SerializeField] private AudioClip waitingSFX;      // wand fx

        [SerializeField] private float waitingVolume = 1f;

        // Chambered spell
        private SpellType _chamberedType;
        private GameObject _chamberedSpell;

        public Action OnSpellCasted;

        private void Awake()
        {
            trajectory.enabled = false;

            if (loopSource)
            {
                loopSource.loop = true;
                loopSource.playOnAwake = false;
            }

            if (oneShotSource)
            {
                oneShotSource.playOnAwake = false;
            }
        }

        public void PrepareSpell(SpellData spell)
        {
            _chamberedType = spell.spellType;
            Debug.Log("[SpellCaster] Preparing spell: " + spell.spellType);
            Debug.Log(spell.spellPrefab);
            
            var projectilePrefab = spell.spellPrefab;
            _chamberedSpell = Instantiate(projectilePrefab, tip.position, tip.rotation);
            _chamberedSpell.GetComponent<Rigidbody>().useGravity = false;
            _chamberedSpell.transform.SetParent(tip);
            _chamberedSpell.transform.localPosition = Vector3.zero;
            _chamberedSpell.transform.localRotation = Quaternion.identity;
            Debug.Log("[SpellCaster] Chambered spell: " + _chamberedType + " " + _chamberedSpell.name);
            
            trajectory.enabled = true;

            PlayWaitingSFX();
        }

        public void CastSpell()
        {
            if (!_chamberedSpell) return;
            Debug.Log("[SpellCaster] Casting spell");

            StopWaitingSFX();

            Rigidbody rb = _chamberedSpell.GetComponent<Rigidbody>();
            rb.useGravity = true;
            
            // Cast projectile
            SpellProjectile spellProjectile = _chamberedSpell.GetComponent<SpellProjectile>();
            spellProjectile.Cast(tip.up);
            if (oneShotSource || castSFX)
            {
                oneShotSource.PlayOneShot(castSFX);
            }
            
            PlayCastSFX();
            PlayProjectileSFX();

            trajectory.enabled = false;
            _chamberedSpell = null;

            OnSpellCasted?.Invoke();
            Debug.Log("[SpellCaster] Spell casted");
        }

        private void PlayCastSFX()
        {
            if (oneShotSource == null || castSFX == null) return;
            oneShotSource.PlayOneShot(castSFX);
        }

        private void PlayProjectileSFX()
        {
            if (oneShotSource == null || projectileSFX == null) return;
            oneShotSource.PlayOneShot(projectileSFX);
        }

        private void PlayWaitingSFX()
        {
            if (loopSource == null || waitingSFX == null) return;

            loopSource.clip = waitingSFX;
            loopSource.volume = waitingVolume;
            loopSource.loop = true;
            loopSource.Play();
        }

        private void StopWaitingSFX()
        {
            if (loopSource == null) return;

            if (loopSource.isPlaying)
            {
                loopSource.Stop();
            }

            loopSource.clip = null;
        }
    }
}
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
        // the other stays attached to the wand

        [SerializeField] private AudioClip castSFX;         // wand sfx
        [SerializeField] private AudioClip projectileSFX;   // spell-specific
        [SerializeField] private AudioClip waitingSFX;      // wand fx

        [SerializeField] private float waitingVolume = 1f;

        // Spell projectiles
        private Dictionary<SpellType, ProjectileData> _projectileData = new();
        private ProjectileData _chamberedData;
        private GameObject _chamberedProjectile;

        public Action OnSpellCasted;

        private void Awake()
        {
            trajectory.enabled = false;

            if (loopSource != null)
            {
                loopSource.loop = true;
                loopSource.playOnAwake = false;
            }

            if (oneShotSource != null)
            {
                oneShotSource.playOnAwake = false;
            }
        }

        public void SetProjectilePrefabs(ProjectileData[] projectiles)
        {
            foreach (var projectile in projectiles)
            {
                _projectileData.Add(projectile.spellType, projectile);
            }
        }

        public void PrepareSpell(SpellType spellType)
        {
            Debug.Log("[SpellCaster] Preparing spell: " + spellType);
            var projectileData = _projectileData[spellType];
            if (!projectileData) return;
            
            _chamberedData = projectileData;
            Debug.Log("[SpellCaster] Chambered projectile: " + _chamberedData.name);

            _chamberedProjectile = Instantiate(_chamberedData.prefab, tip.position, tip.rotation);
            _chamberedProjectile.GetComponent<Rigidbody>().useGravity = false;
            _chamberedProjectile.transform.SetParent(tip);
            _chamberedProjectile.transform.localPosition = Vector3.zero;
            _chamberedProjectile.transform.localRotation = Quaternion.identity;
            
            trajectory.enabled = true;

            PlayWaitingSFX();
        }

        public void CastSpell()
        {
            if (!_chamberedProjectile) return;
            Debug.Log("[SpellCaster] Casting spell");

            StopWaitingSFX();

            _chamberedProjectile.transform.SetParent(null);

            Rigidbody rb = _chamberedProjectile.GetComponent<Rigidbody>();
            rb.useGravity = true;
            rb.AddForce(tip.up * _chamberedData.launchForce, ForceMode.Impulse);
            
            // Start projectile life
            // SpellProjectile spellProjectile = _chamberedProjectile.GetComponent<SpellProjectile>();
            // spellProjectile.Launch();
            
            PlayCastSFX();
            PlayProjectileSFX();

            trajectory.enabled = false;
            _chamberedProjectile = null;

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
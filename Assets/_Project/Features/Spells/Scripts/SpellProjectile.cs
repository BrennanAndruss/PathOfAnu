using System;
using UnityEngine;

namespace _Project.Features.Spells.Scripts
{
    public class SpellProjectile : MonoBehaviour
    {
        [SerializeField] private SpellType spellType;
        
        [Header("Projectile Settings")]
        [SerializeField] private float lifetime = 5f;
        [SerializeField] private float launchForce = 15f;
        [SerializeField] private AudioClip sound;

        private Rigidbody _rb;
        private AudioSource _audioSource;

        private void Awake()
        {
            Debug.Log("[SpellProjectile] " + gameObject.name + " Awake");
            
            _rb = GetComponent<Rigidbody>();
            _audioSource = GetComponent<AudioSource>();
            if (_audioSource)
            {
                _audioSource.playOnAwake = false;
            }
        }

        public void Cast(Vector3 direction)
        {
            // Detach projectile from the wand
            transform.SetParent(null);

            // Cast projectile
            _rb.isKinematic = false;
            _rb.AddForce(direction * launchForce, ForceMode.Impulse);
            
            // Play projectile SFX
            if (_audioSource && sound)
            {
                _audioSource.PlayOneShot(sound);
            }
            
            // Start projectile lifetime
            Destroy(gameObject, lifetime);
        }

        public void OnCollisionEnter(Collision collision)
        {
            // Handle spell impact
            // rotation = normal vector to surface
            // Instantiate(impactEffect, transform.position, rotation)
            
            // need to handle collision with wand...
            // Destroy(gameObject);
        }
    }
}
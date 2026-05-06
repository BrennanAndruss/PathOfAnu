using System;
using UnityEngine;

namespace _Project.Features.Spells.Scripts
{
    public class SpellProjectile : Spell
    {
        [Header("Projectile Settings")]
        [SerializeField] private float lifetime = 5f;
        [SerializeField] private float launchForce = 15f;
        [SerializeField] private AudioClip sound;

        private Collider _collider;
        
        protected override void Awake()
        {
            Debug.Log("[SpellProjectile] " + gameObject.name + " created.");
            
            // Disable collider to ignore collisions when chambered
            _collider = GetComponent<Collider>();
            _collider.enabled = false;
            
            Rb = GetComponent<Rigidbody>();
            Rb.isKinematic = true;
            AudioSource = GetComponent<AudioSource>();
            if (AudioSource)
            {
                AudioSource.playOnAwake = false;
            }
        }
        
        public override void Cast()
        {
            // Detach projectile from the wand
            transform.SetParent(null);

            // Cast projectile
            _collider.enabled = true;
            Rb.isKinematic = false;
            Rb.AddForce(transform.forward * launchForce, ForceMode.Impulse);
            
            // Play projectile SFX
            if (AudioSource && sound)
            {
                AudioSource.PlayOneShot(sound);
            }
            
            // Start projectile lifetime
            Destroy(gameObject, lifetime);
        }

        public void OnCollisionEnter(Collision collision)
        {
            // Ignore collisions when the spell is chambered
            if (!_collider.enabled) return;
            
            // Ignore collisions with the player or wand
            if (collision.gameObject.layer == LayerMask.NameToLayer("Player")) return;
            
            // Handle spell impact
            // rotation = normal vector to surface
            // Instantiate(impactEffect, transform.position, rotation)
            
            Destroy(gameObject);
        }
    }
}
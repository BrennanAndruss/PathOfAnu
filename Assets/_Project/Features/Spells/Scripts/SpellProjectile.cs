using System;
using UnityEngine;

namespace _Project.Features.Spells.Scripts
{
    public class SpellProjectile : Spell
    {
        [SerializeField] private SpellType spellType;
        
        [Header("Projectile Settings")]
        [SerializeField] private float lifetime = 5f;
        [SerializeField] private float launchForce = 15f;
        [SerializeField] private AudioClip sound;

        public override void Cast()
        {
            // Detach projectile from the wand
            transform.SetParent(null);

            // Cast projectile
            Rb.isKinematic = false;
            var direction = transform.forward; // check this (was coming from spellcaster before)
            Rb.AddForce(direction * launchForce, ForceMode.Impulse);
            
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
            // Handle spell impact
            // rotation = normal vector to surface
            // Instantiate(impactEffect, transform.position, rotation)
            
            // need to handle collision with wand...
            // Destroy(gameObject);
        }
    }
}
using UnityEngine;

namespace _Project.Features.Spells.Scripts
{
    public class SpellProjectile : MonoBehaviour
    {
        [SerializeField] private float lifetime = 5f;

        public void Launch()
        {
            Destroy(gameObject, lifetime);
        }
    }
}
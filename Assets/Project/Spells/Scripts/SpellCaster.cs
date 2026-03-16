using System;
using UnityEngine;

namespace Project.Spells.Scripts
{
    public class SpellCaster : MonoBehaviour
    {
        [Header("Wand Properties")]
        [SerializeField] private Transform tip;
        [SerializeField] private LineRenderer trajectory;

        [Header("Audio")]
        [SerializeField] private AudioSource oneShotSource;   // cast + launch
        [SerializeField] private AudioSource loopSource;      // waiting loop

        [SerializeField] private AudioClip castSFX;
        [SerializeField] private AudioClip projectileSFX;
        [SerializeField] private AudioClip waitingSFX;

        [SerializeField] private float waitingVolume = 1f;

        private GameObject _chamberedSpell;
        private float _launchForce = 10f;

        public Action OnSpellCasted;

        [Space(10)]
        public GameObject[] spellPrefabs;

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

        public void PrepareSpell(SpellType spellType)
        {
            if (_chamberedSpell != null) return;

            GameObject spellPrefab = spellPrefabs[0];

            if (spellType == SpellType.Water)
            {
                spellPrefab = spellPrefabs[0];
            }
            else if (spellType == SpellType.Fire)
            {
                spellPrefab = spellPrefabs[1];
            }
            else if (spellType == SpellType.Earth)
            {
                spellPrefab = spellPrefabs[3];
            }
            else if (spellType == SpellType.Air)
            {
                spellPrefab = spellPrefabs[4];
            }
            else if (spellType == SpellType.Electric)
            {
                spellPrefab = spellPrefabs[2];
            }

            _chamberedSpell = Instantiate(spellPrefab, tip.position, tip.rotation);
            _chamberedSpell.GetComponent<Rigidbody>().useGravity = false;
            _chamberedSpell.transform.SetParent(tip);
            _chamberedSpell.transform.localPosition = Vector3.zero;
            _chamberedSpell.transform.localRotation = Quaternion.identity;

            trajectory.enabled = true;

            PlayWaitingSFX();
        }

        public void CastSpell()
        {
            if (!_chamberedSpell) return;

            StopWaitingSFX();

            _chamberedSpell.transform.SetParent(null);

            Rigidbody rb = _chamberedSpell.GetComponent<Rigidbody>();
            rb.useGravity = true;
            rb.AddForce(tip.up * _launchForce, ForceMode.Impulse);
            PlayCastSFX();
            PlayProjectileSFX();

            trajectory.enabled = false;
            _chamberedSpell = null;

            OnSpellCasted?.Invoke();
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
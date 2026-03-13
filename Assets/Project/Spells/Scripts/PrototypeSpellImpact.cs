using System.Collections;
using UnityEngine;

namespace Project.Spells.Scripts
{
    public class PrototypeSpellImpact : MonoBehaviour
    {
        [SerializeField] private float jumpHeight = 0.75f;
        [SerializeField] private float flipDuration = 0.8f;
        [SerializeField] private int flipCount = 2;

        private bool _hasImpacted;

        private void OnCollisionEnter(Collision collision)
        {
            if (_hasImpacted)
            {
                return;
            }

            Transform bowl = FindBowlTransform(collision.collider.transform);
            if (bowl == null)
            {
                return;
            }

            _hasImpacted = true;
            Debug.Log($"[PrototypeSpellImpact] Bowl hit: {bowl.name}");

            BowlFlipAnimator animator = bowl.GetComponent<BowlFlipAnimator>();
            if (animator == null)
            {
                animator = bowl.gameObject.AddComponent<BowlFlipAnimator>();
            }

            animator.PlayFlip(jumpHeight, flipDuration, flipCount);
            Destroy(gameObject);
        }

        private static Transform FindBowlTransform(Transform current)
        {
            while (current != null)
            {
                if (current.name.ToLower().Contains("bowl"))
                {
                    return current;
                }

                current = current.parent;
            }

            return null;
        }
    }

    public class BowlFlipAnimator : MonoBehaviour
    {
        private Coroutine _activeFlip;

        public void PlayFlip(float jumpHeight, float flipDuration, int flipCount)
        {
            if (_activeFlip != null)
            {
                StopCoroutine(_activeFlip);
            }

            _activeFlip = StartCoroutine(FlipRoutine(jumpHeight, flipDuration, flipCount));
        }

        private IEnumerator FlipRoutine(float jumpHeight, float flipDuration, int flipCount)
        {
            Vector3 startPosition = transform.position;
            Quaternion startRotation = transform.rotation;

            Rigidbody body = GetComponent<Rigidbody>();
            bool hadBody = body != null;
            bool previousKinematic = false;
            bool previousGravity = false;

            if (hadBody)
            {
                previousKinematic = body.isKinematic;
                previousGravity = body.useGravity;
                body.linearVelocity = Vector3.zero;
                body.angularVelocity = Vector3.zero;
                body.isKinematic = true;
                body.useGravity = false;
            }

            float totalSpin = 360f * flipCount;
            float elapsed = 0f;

            while (elapsed < flipDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / flipDuration);
                float verticalOffset = Mathf.Sin(t * Mathf.PI) * jumpHeight;

                transform.position = startPosition + Vector3.up * verticalOffset;
                transform.rotation = startRotation * Quaternion.AngleAxis(totalSpin * t, Vector3.right);

                yield return null;
            }

            transform.position = startPosition;
            transform.rotation = startRotation;

            if (hadBody)
            {
                body.isKinematic = previousKinematic;
                body.useGravity = previousGravity;
            }

            _activeFlip = null;
        }
    }
}

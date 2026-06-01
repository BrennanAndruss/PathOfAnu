using System;
using UnityEngine;

namespace _Project.Code.Scripts
{
    public class DialogueArea : MonoBehaviour
    {
        [SerializeField] private GameObject canvas;

        private void OnTriggerEnter(Collider other)
        {
            canvas.SetActive(true);
        }

        private void OnTriggerExit(Collider other)
        {
            canvas.SetActive(false);
        }
    }
}

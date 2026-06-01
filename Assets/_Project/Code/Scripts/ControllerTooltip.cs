using UnityEngine;

namespace _Project.Code.Scripts
{
    public class ControllerTooltip : MonoBehaviour
    {
        [Header("Anchor")] 
        [SerializeField] private Transform physicalButtonAnchor;
        [SerializeField] private Transform floatingCanvasAnchor;
        
        private LineRenderer _lineRenderer;

        private void Awake()
        {
            _lineRenderer = GetComponent<LineRenderer>();
            
            // Ensure the line renderer has exactly two endpoints (Start and Finish)
            _lineRenderer.positionCount = 2;
        }

        private void LateUpdate()
        {
            if (!physicalButtonAnchor || !floatingCanvasAnchor) return;

            // Update the positions in local space relative to the moving controller parent
            // Point 0 = Stays glued to the physical button mesh
            _lineRenderer.SetPosition(0, transform.InverseTransformPoint(physicalButtonAnchor.position));
            
            // Point 1 = Stays glued to the center of your floating text box
            _lineRenderer.SetPosition(1, transform.InverseTransformPoint(floatingCanvasAnchor.position));
        }
    }
}
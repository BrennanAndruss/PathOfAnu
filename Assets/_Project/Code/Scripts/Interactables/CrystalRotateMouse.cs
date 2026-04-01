using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class CrystalRotateMouse : MonoBehaviour
{
    [SerializeField] float _mouseSensitivity = 150f;
    [SerializeField] float minPitch = 0f;   // lowest angle (look down)
    [SerializeField] float maxPitch = 80f;    // highest angle (look up)
    [SerializeField] bool invertY = false;

    float _pitch;              // accumulated pitch in degrees (-180..180)

    InputAction _lookAction;

    void Awake()
    {
        _lookAction = InputSystem.actions.FindAction("Look", throwIfNotFound: true);
        // normalize current local X rotation into -180..180 so clamping behaves correctly
        _pitch = transform.localEulerAngles.x;
        if (_pitch > 180f) _pitch -= 360f;
    }

    void OnEnable()  => _lookAction.Enable();
    void OnDisable() => _lookAction.Disable();

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        Vector2 lookInput = _lookAction.ReadValue<Vector2>();

        // mouse Y usually controls pitch. Flip sign if you want opposite behavior.
        float deltaY = lookInput.y * _mouseSensitivity * Time.deltaTime;
        _pitch += invertY ? deltaY : -deltaY;   // change sign depending on desired direction

        // clamp the accumulated pitch
        _pitch = Mathf.Clamp(_pitch, minPitch, maxPitch);

        // apply rotation (localRotation so yaw stays independent if you add yaw later)
        transform.localRotation = Quaternion.Euler(_pitch, 0f, 0f);
    }

    // optional helpers you already had
    public Quaternion GetRotation() => transform.rotation;
    public Vector3 getUpVector() => transform.up;
}

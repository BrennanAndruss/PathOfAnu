using UnityEngine;

public class SetRefreshRate : MonoBehaviour
{
    void Start()
    {
        OVRManager.display.displayFrequency = 90f;
    }
}
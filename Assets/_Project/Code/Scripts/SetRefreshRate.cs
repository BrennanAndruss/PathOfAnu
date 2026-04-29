using UnityEngine;

public class etRefreshRate : MonoBehaviour
{
    void Start()
    {
        OVRManager.display.displayFrequency = 90f;
    }
}
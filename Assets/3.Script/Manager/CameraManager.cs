using Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance;

    public CinemachineVirtualCamera[] fixedCameras; 

    private void Awake()
    {
        Instance = this;
    }

    public void GetCameraByIndex(int index)
    {
        if (index >= 0 && index < fixedCameras.Length)
        {
            var vcam =fixedCameras[index];
            vcam.Priority = 20;
        }
    }
}
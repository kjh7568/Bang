using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerCameraController : MonoBehaviour
{
    [Header("Transforms")]
    [SerializeField] private Transform headTransform;    // Yaw+Pitch 적용 대상
    [SerializeField] private Transform cameraTransform;  // VirtualCamera Follow 대상 (head 자식)

    [Header("Sensitivity")]
    [SerializeField] private float sensitivity = 1.0f;

    [Header("Pitch Limit")]
    [SerializeField] private float minPitch = -15f;
    [SerializeField] private float maxPitch =  15f;

    [Header("Yaw Limit")]
    [SerializeField] private float minYawOffset = -45f;
    [SerializeField] private float maxYawOffset =  45f;

    private float pitchOffset = 0f;   // 상하(relative)
    private float yawOffset   = 0f;   // 좌우(relative)
    private Quaternion headOffset;    // 프리팹의 원래 회전 저장

    private bool isCameraActive = false;
    
    private void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        
        OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
    }
    
    private void Update()
    {
        UpdateCamera();
    }   
    
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        isCameraActive = (scene.name == "4. InGame");
        Debug.Log($"씬 전환: {scene.name}, isCameraActive = {isCameraActive}");

        if (isCameraActive)
        {
            InitializeCamera();
        }
    }
    
    private void InitializeCamera()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible    = false;

        // 1) 프리팹에 설정된 초기 헤드 회전 보존
        headOffset = headTransform.localRotation;

        // 2) 상대 오프셋 초기화
        pitchOffset = 0f;
        yawOffset   = 0f;

        // 3) VirtualCamera 셋업
        var vcam = FindObjectOfType<CinemachineVirtualCamera>();
        if (vcam != null)
        {
            vcam.Follow = cameraTransform;
            vcam.LookAt = cameraTransform;
        }
    }

    private void UpdateCamera()
    {
        if (isCameraActive == false) return;
        
        // 마우스 입력
        float mouseX = Input.GetAxis("Mouse X") * sensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity;

        // 1) 좌우(relative Yaw) 오프셋 누적 후 클램프
        yawOffset += mouseX;
        yawOffset  = Mathf.Clamp(yawOffset, minYawOffset, maxYawOffset);

        // 2) 상하(relative Pitch) 오프셋 누적 후 클램프
        pitchOffset -= mouseY;
        pitchOffset  = Mathf.Clamp(pitchOffset, minPitch, maxPitch);

        // 3) 머리 회전 = 초기 회전(headOffset) + 상대 회전
        Quaternion relative = Quaternion.Euler(pitchOffset, yawOffset, 0f);
        headTransform.localRotation = headOffset * relative;

        // 4) 카메라 시야는 Pitch만 (head의 자식이니 Yaw는 자동 따라감)
        cameraTransform.localRotation = Quaternion.Euler(pitchOffset, 0f, 0f);
    }
}

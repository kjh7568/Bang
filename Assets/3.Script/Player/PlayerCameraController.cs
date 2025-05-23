using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;
using Fusion;

public class PlayerCameraController : NetworkBehaviour
{
    [Header("Transforms")]
    [SerializeField] private Transform headTransform;    
    [SerializeField] private Transform cameraTransform;  

    [Header("Sensitivity")]
    [SerializeField] private float sensitivity = 1.0f;

    [Header("Pitch Limit")]
    [SerializeField] private float minPitch = -15f;
    [SerializeField] private float maxPitch =  15f;

    [Header("Yaw Limit")]
    [SerializeField] private float minYawOffset = -45f;
    [SerializeField] private float maxYawOffset =  45f;

    private float pitchOffset = 0f;   
    private float yawOffset   = 0f;   
    private Quaternion headOffset;    

    private bool isLocalPlayer = false;
    private bool isCameraActive = false;
    
    public override void Spawned()
    {
        // Fusion 권한 검사
        isLocalPlayer = Object.HasInputAuthority;

        // 로컬 플레이어만 활성화
        enabled = isLocalPlayer;
        
        // 씬이 이미 인게임 씬이라면, 여기서도 반드시 카메라 초기화
        if (isLocalPlayer && isCameraActive)
        {
            InitializeCamera();
        }
    }

    private void Awake()
    {
        // 최초엔 꺼두고, Spawned()에서 켜집니다
        enabled = false;
        
        // 씬 전환 상태만 판단
        SceneManager.sceneLoaded += OnSceneLoaded;
        OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        isCameraActive = (scene.name == "4. InGame Test");

        // 호스트 쪽에서만 일어나서 권한이 세팅됐던 시점엔 Initialize 안 됐을 수 있으니
        if (isLocalPlayer && isCameraActive)
        {
            InitializeCamera();
        }
    }

    private void InitializeCamera()
    {
        if (UIManager.Instance.isPanelOn)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible    = false;
        }

        headOffset = headTransform.localRotation;
        pitchOffset = 0f;
        yawOffset   = 0f;

        var vcam = FindObjectOfType<CinemachineVirtualCamera>();
        if (vcam != null)
        {
            vcam.Follow = cameraTransform;
            vcam.LookAt = cameraTransform;
            vcam.enabled = true;
        }
    }

    private void Update()
    {
        // 로컬 + 인게임 씬일 때만
        // if (!isLocalPlayer || !isCameraActive || UIManager.Instance.isPanelOn)
        //     return;
        
        if (!isCameraActive || UIManager.Instance.isPanelOn)
            return;
        
        float mouseX = Input.GetAxis("Mouse X") * sensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity;

        RPC_UpdateCamera(mouseX, mouseY);
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
    private void RPC_UpdateCamera(float mouseX, float mouseY)
    {
        Debug.Log("123");
        RotateCamera(mouseX, mouseY);
    }

    private void RotateCamera(float mouseX, float mouseY)
    {
        yawOffset = Mathf.Clamp(yawOffset + mouseX, minYawOffset, maxYawOffset);
        pitchOffset = Mathf.Clamp(pitchOffset - mouseY, minPitch, maxPitch);
        
        // 머리(고개) = 초기 오프셋 * 상대 회전
        Quaternion rel = Quaternion.Euler(pitchOffset, yawOffset, 0f);
        headTransform.localRotation   = headOffset * rel;

        // 카메라 시야 = Pitch만
        cameraTransform.localRotation = Quaternion.Euler(pitchOffset, 0f, 0f);
    }
}

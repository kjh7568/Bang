using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AnimationTrigger : NetworkBehaviour
{
    private Animator animator;

    private void Start()
    {
        
        if (!TryGetComponent(out animator))
        {
            Debug.LogWarning("Animator가 이 오브젝트에 없습니다.");
        }
        animator.enabled = false;
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().buildIndex  ==3)
        {
            animator.enabled = true;
        }

        if (HasInputAuthority) // 자신이 조작하는 캐릭터만 입력 받기
        {
            CheckInput();
        }
    }

    private void CheckInput()
    {
        if (Input.GetKeyDown(KeyCode.Q))
            RPC_PlayAnimation("shooting");
        else if (Input.GetKeyDown(KeyCode.W))
            RPC_PlayAnimation("pointing");
        else if (Input.GetKeyDown(KeyCode.E))
            RPC_PlayAnimation("dodging");
        else if (Input.GetKeyDown(KeyCode.R))
            RPC_PlayAnimation("drinking");
        else if (Input.GetKeyDown(KeyCode.T))
            RPC_PlayAnimation("dying");
        else if (Input.GetKeyDown(KeyCode.Y))
            RPC_PlayAnimation("picking");
        else if (Input.GetKeyDown(KeyCode.U))
            RPC_PlayAnimation("drawing");
    }

    private void PlayAnimation(string trigger)
    {
        if (animator != null)
        {
            animator.SetTrigger(trigger);
        }
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
    private void RPC_PlayAnimation(string trigger)
    {
        PlayAnimation(trigger);
    }
    
    
    
}

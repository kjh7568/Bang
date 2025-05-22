using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationTrigger : MonoBehaviour
{
    private Animator animator;

    

    private void Start()
    {
        // Animator 안전하게 가져오기
        if (!TryGetComponent(out animator))
        {
            Debug.LogWarning("Animator가 이 오브젝트에 없습니다.");
        }
    }

    private void Update()
    {
       
    }
    
    
    private void PlayAnimation()
    {
        if (animator != null)
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                PlayShooting();
            }
            else if (Input.GetKeyDown(KeyCode.W))
            {
                PlayPointing();
                
            }
            else if (Input.GetKeyDown(KeyCode.E))
            {
                PlayDodging();
            }
            else if (Input.GetKeyDown(KeyCode.R))
            {
                PlayDrinking();
            }
            else if (Input.GetKeyDown(KeyCode.T))
            {
                PlayDying();
            }




        }
        else
        {
            Debug.LogWarning("Animator가 이 오브젝트에 없습니다.");
        }
    }

    private void PlayShooting()
    {
        animator.SetTrigger("shooting");
    }
    private void PlayDying()
    {
        ActivateRagdoll();
        //animator.SetTrigger("dying");
    }

    private void PlayDodging()
    {
        animator.SetTrigger("dodging");
    }

    private void PlayDrinking()
    {
        animator.SetTrigger("drinking");
    }
    private void PlayPointing()
    {
        animator.SetTrigger("pointing");
    }
    private void ActivateRagdoll()
    {
        animator.enabled = false; // 애니메이션 멈춤

        foreach (var rb in GetComponentsInChildren<Rigidbody>())
        {
            rb.isKinematic = false;
            rb.velocity = Vector3.zero; // 깔끔한 전환
        }

        foreach (var col in GetComponentsInChildren<Collider>())
        {
            col.enabled = true;
        }

        Debug.Log("레그돌 활성화됨");
    }
}

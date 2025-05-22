using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationTrigger : MonoBehaviour
{
    private Animator animator;

    [SerializeField] private CardData testCard;

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
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (testCard != null)
            {
                testCard.UseCard(); // ScriptableObject 메서드 실행

                // 필요시 애니메이션 트리거도 설정
                if (animator != null)
                {
                    animator.SetTrigger("shooting"); // Animator에 "Use" 트리거가 있을 경우
                }
            }
            else
            {
                Debug.LogWarning("testCard가 할당되지 않았습니다.");
            }
        }
    }
}

using UnityEngine;
using UnityEngine.UI;

public class MyInfoPanel : MonoBehaviour
{
    public Button openPanelButton;
    public RectTransform MyInfoPanelTransform;
    public float moveSpeed = 5f;

    private Vector2 startPosition;
    private Vector2 hiddenPosition;
    private Vector2 targetPosition;
    private bool shouldMove = false;
    private bool isOpen = false;

    void Start()
    {
        openPanelButton.onClick.AddListener(OpenMyPanel);

        startPosition = MyInfoPanelTransform.anchoredPosition;
        hiddenPosition = startPosition + new Vector2(-800f, 0); 

        targetPosition = hiddenPosition; 
    }

    void Update()
    {
        if (shouldMove)
        {
            MyInfoPanelTransform.anchoredPosition = Vector2.Lerp(
                MyInfoPanelTransform.anchoredPosition,
                targetPosition,
                Time.deltaTime * moveSpeed
            );

            if (Vector2.Distance(MyInfoPanelTransform.anchoredPosition, targetPosition) < 0.1f)
            {
                MyInfoPanelTransform.anchoredPosition = targetPosition;
                shouldMove = false;
            }
        }
    }

    void OpenMyPanel()
    {
        if (isOpen)
        {
            targetPosition = startPosition; 
        }
        else
        {
            targetPosition = hiddenPosition; 
        }

        isOpen = !isOpen;
        shouldMove = true;
    }
}
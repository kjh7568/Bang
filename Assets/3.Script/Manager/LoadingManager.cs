using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class LoadingManager : MonoBehaviour
{
    [SerializeField] private GameObject loadingUI;
    [SerializeField] private Slider loadingBar;
    
    private void Awake()
    {
        loadingUI.SetActive(true);
        StartLoading();
        
        Invoke("EndLoading", 3f);
    }
    
    public void StartLoading()
    {
        loadingBar.value = 0;
        loadingBar.DOValue(1f, 3f).SetEase(Ease.InOutQuad);
    }

    public void EndLoading()
    {
        loadingUI.SetActive(false);
    }
}

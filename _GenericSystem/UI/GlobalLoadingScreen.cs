using System;
using System.Collections;
using MLTD.GenericSystem;
using UnityEngine;
using UnityEngine.UI;

public class GlobalLoadingScreen : MonoBehaviour
{   
    public float fakeTimer = 2f;
    public Slider loadingBar;

    private void Awake()
    {
    }

    private void OnEnable()
    {
        if(GlobalGameManager.Instance.bootCompleted)
            StartCoroutine(WaitLoadingScreens());
    }

    private void OnDisable()
    {
    }

    Action DisableLoading()
    {
        Action disableAction = () => gameObject.SetActive(false);
        return disableAction;
    }

    IEnumerator WaitLoadingScreens()
    {
        loadingBar.value = 0f;

        float timer = 0f;
        while (timer < fakeTimer)
        {
            timer += Time.deltaTime;
            loadingBar.value = Mathf.Clamp01(timer / fakeTimer);
            yield return null;
        }
        
        // 🔹 HOLD at full
            yield return new WaitForSeconds(1f); // 0.5–1 sec feels good
            
        gameObject.SetActive(false);

    }
}

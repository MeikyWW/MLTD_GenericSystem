using System;
using UnityEngine;
using UnityEngine.UI;

public class GlobalLoadingScreen : MonoBehaviour
{   
    // public float fakeTimer = 2f;
    // public Slider loadingBar;
    // public UI_Renderer renderer;

    // private void Awake()
    // {
    //     renderer.SetOpacityNoTransition(0f);
    // }

    // private void OnEnable()
    // {
    //     renderer.SetOpacity(1f);

    //     /*  */StartCoroutine(WaitLoadingScreens());
    // }

    // private void OnDisable()
    // {
    //     //when disabled Manually
    //     renderer.SetOpacityNoTransition(0f);
    //     MainGameplayManager.Instance.FreezePlayer(false);
    // }

    // Action DisableLoading()
    // {
    //     Action disableAction = () => gameObject.SetActive(false);
    //     return disableAction;
    // }

    // IEnumerator WaitLoadingScreens()
    // {
    //     loadingBar.value = 0f;

    //     float timer = 0f;
    //     while (timer < fakeTimer)
    //     {
    //         timer += Time.deltaTime;
    //         loadingBar.value = Mathf.Clamp01(timer / fakeTimer);

    //         if (MainGameplayManager.Instance.existingPlayer != null)
    //         {
    //             MainGameplayManager.Instance.FreezePlayer(true);
    //         }

    //         yield return null;
    //     }

    //     renderer.SetOpacity(0f, 0.2f, DisableLoading()); // after complete, it will call an event that Disables the Canvas Loading

    // }
}

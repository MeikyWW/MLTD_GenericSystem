using UnityEngine;
using UnityEngine.Events;
//using DG.Tweening;
using System;

public class UI_Renderer : MonoBehaviour
{
    public float opacity = 1f;
    private float currentOpacity = 0f;
    
    static float transitionDuration = 0.2f;


    // private Tween activeTween;

    public CanvasRenderer[] canvasRenderers;

    [Header("Events")]
    public UnityEvent onFadeComplete;

    
    void Awake()
    {
        // LoadAllUiRenderersFromChild();
    }

    void LoadAllUiRenderersFromChild()
    {
        canvasRenderers = GetComponentsInChildren<CanvasRenderer>(true);
    }

    public void SetOpacity()
    {
        // SetOpacity(opacity);
    }

    public void SetOpacity(float _opacity)
    {
        // SetOpacity(_opacity, transitionDuration, null);
    }

    public void SetOpacity(float _opacity, float _duration)
    {
        // SetOpacity(_opacity, _duration, null);
    }

    public void SetOpacity(float _opacity, float _duration, Action onComplete = null)
    {
        // if (canvasRenderers == null || canvasRenderers.Length == 0) return;

        // activeTween?.Kill(); // Cancel any existing tween

        // Sequence seq = DOTween.Sequence();

        // foreach (var renderer in canvasRenderers)
        // {
        //     float startAlpha = renderer.GetAlpha();
        //     seq.Join(DOTween.To(() => startAlpha, x =>
        //     {
        //         startAlpha = x;
        //         renderer.SetAlpha(x);
        //     }, _opacity, _duration));
        // }

        // activeTween = seq;

        // seq.OnComplete(() =>
        // {
        //     currentOpacity = _opacity;
        //     onFadeComplete?.Invoke();
        //     onComplete?.Invoke();
        // });
    }

    public void SetOpacityNoTransition(float _opacity)
    {
        // activeTween?.Kill();
        // foreach (var renderer in canvasRenderers)
        // {
        //     renderer.SetAlpha(_opacity);
        // }
        // currentOpacity = _opacity;
    }
}
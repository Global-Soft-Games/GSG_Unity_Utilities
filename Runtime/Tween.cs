using UnityEngine;
using System;
using System.Collections.Generic;

public class Tween
{
    public enum EaseType
    {
        Linear,
        EaseInQuad,
        EaseOutQuad,
        EaseInOutQuad,
        EaseInCubic,
        EaseOutCubic,
        EaseInOutCubic,
        EaseInQuart,
        EaseOutQuart,
        EaseInOutQuart,
        EaseInExpo,
        EaseOutExpo,
        EaseInOutExpo,
        EaseInBack,
        EaseOutBack,
        EaseInOutBack,
        EaseInElastic,
        EaseOutElastic,
        EaseInOutElastic,
        EaseInBounce,
        EaseOutBounce,
        EaseInOutBounce
    }

    public enum LoopType
    {
        None,
        Restart,
        PingPong
    }

    private TweenValue tweenValue;
    private float duration;
    private float elapsedTime;
    private bool isPlaying;
    private Action onComplete;
    private EaseType easeType;
    private LoopType loopType = LoopType.None;
    private int loopCount = 0;
    private float delay = 0f;
    private float initialDelay;
    private bool isPaused = false;

    private Tween(TweenValue tweenValue, float duration, EaseType easeType = EaseType.Linear)
    {
        this.tweenValue = tweenValue;
        this.duration = duration;
        this.easeType = easeType;
        this.elapsedTime = 0f;
        this.isPlaying = false;
    }

    // 靜態創建方法
    public static Tween To(float from, float to, float duration, Action<float> onUpdate)
        => new Tween(new FloatTweenValue(from, to, onUpdate), duration);

    public static Tween To(Vector3 from, Vector3 to, float duration, Action<Vector3> onUpdate)
        => new Tween(new Vector3TweenValue(from, to, onUpdate), duration);

    public static Tween To(Color from, Color to, float duration, Action<Color> onUpdate)
        => new Tween(new ColorTweenValue(from, to, onUpdate), duration);

    public void Start()
    {
        isPlaying = true;
        elapsedTime = 0f;
        TweenManager.Instance.AddTween(this);
    }

    public void Stop()
    {
        isPlaying = false;
        TweenManager.Instance.RemoveTween(this);
    }

    public Tween SetDelay(float delay)
    {
        this.delay = delay;
        this.initialDelay = delay;
        return this;
    }

    public Tween SetLoopType(LoopType type, int count = -1)
    {
        this.loopType = type;
        this.loopCount = count;
        return this;
    }

    public Tween SetEase(EaseType ease)
    {
        this.easeType = ease;
        return this;
    }

    public void Pause()
    {
        isPaused = true;
    }

    public void Resume()
    {
        isPaused = false;
    }

    public void Update()
    {
        if (!isPlaying || isPaused) return;

        if (delay > 0)
        {
            delay -= Time.deltaTime;
            return;
        }

        elapsedTime += Time.deltaTime;
        float t = Mathf.Clamp01(elapsedTime / duration);
        
        float easedT = ApplyEasing(t);
        tweenValue.UpdateValue(easedT);

        if (t >= 1f)
        {
            HandleLooping();
        }
    }

    private void HandleLooping()
    {
        switch (loopType)
        {
            case LoopType.None:
                Complete();
                break;

            case LoopType.Restart:
                if (loopCount != -1)
                {
                    loopCount--;
                    if (loopCount == 0)
                    {
                        Complete();
                        return;
                    }
                }
                elapsedTime = 0f;
                delay = initialDelay;
                break;

            case LoopType.PingPong:
                if (loopCount != -1)
                {
                    loopCount--;
                    if (loopCount == 0)
                    {
                        Complete();
                        return;
                    }
                }
                tweenValue.ReverseValues();
                elapsedTime = 0f;
                delay = initialDelay;
                break;
        }
    }

    private void Complete()
    {
        isPlaying = false;
        onComplete?.Invoke();
        TweenManager.Instance.RemoveTween(this);
    }

    public Tween OnComplete(Action callback)
    {
        onComplete = callback;
        return this;
    }

    private float ApplyEasing(float t)
    {
        switch (easeType)
        {
            case EaseType.Linear: return t;
            case EaseType.EaseInQuad: return t * t;
            case EaseType.EaseOutQuad: return t * (2 - t);
            case EaseType.EaseInOutQuad: return t < 0.5f ? 2 * t * t : -1 + (4 - 2 * t) * t;
            case EaseType.EaseInCubic: return t * t * t;
            case EaseType.EaseOutCubic: return 1 - Mathf.Pow(1 - t, 3);
            case EaseType.EaseInOutCubic: return t < 0.5f ? 4 * t * t * t : 1 - Mathf.Pow(-2 * t + 2, 3) / 2;
            case EaseType.EaseInExpo: return t == 0 ? 0 : Mathf.Pow(2, 10 * t - 10);
            case EaseType.EaseOutExpo: return t == 1 ? 1 : 1 - Mathf.Pow(2, -10 * t);
            case EaseType.EaseInElastic: return ElasticEaseIn(t);
            case EaseType.EaseOutElastic: return ElasticEaseOut(t);
            case EaseType.EaseInBounce: return 1 - BounceEaseOut(1 - t);
            case EaseType.EaseOutBounce: return BounceEaseOut(t);
            default: return t;
        }
    }

    private float ElasticEaseOut(float t)
    {
        const float c4 = (2 * Mathf.PI) / 3;
        return t == 0 ? 0 : t == 1 ? 1 : Mathf.Pow(2, -10 * t) * Mathf.Sin((t * 10 - 0.75f) * c4) + 1;
    }

    private float ElasticEaseIn(float t)
    {
        const float c4 = (2 * Mathf.PI) / 3;
        return t == 0 ? 0 : t == 1 ? 1 : -Mathf.Pow(2, 10 * t - 10) * Mathf.Sin((t * 10 - 10.75f) * c4);
    }

    private float BounceEaseOut(float t)
    {
        if (t < 1 / 2.75f)
            return 7.5625f * t * t;
        else if (t < 2 / 2.75f)
            return 7.5625f * (t -= 1.5f / 2.75f) * t + 0.75f;
        else if (t < 2.5 / 2.75f)
            return 7.5625f * (t -= 2.25f / 2.75f) * t + 0.9375f;
        else
            return 7.5625f * (t -= 2.625f / 2.75f) * t + 0.984375f;
    }
} 
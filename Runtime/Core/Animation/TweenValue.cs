using System;
using UnityEngine;

public abstract class TweenValue
{
    protected float startValue;
    protected float endValue;

    public abstract void UpdateValue(float t);
    public abstract void ReverseValues();
}

public class FloatTweenValue : TweenValue
{
    private Action<float> onUpdate;

    public FloatTweenValue(float from, float to, Action<float> onUpdate)
    {
        this.startValue = from;
        this.endValue = to;
        this.onUpdate = onUpdate;
    }

    public override void UpdateValue(float t)
    {
        float current = Mathf.Lerp(startValue, endValue, t);
        onUpdate?.Invoke(current);
    }

    public override void ReverseValues()
    {
        float temp = startValue;
        startValue = endValue;
        endValue = temp;
    }
}

public class Vector3TweenValue : TweenValue
{
    private Vector3 fromVector;
    private Vector3 toVector;
    private Action<Vector3> onUpdate;

    public Vector3TweenValue(Vector3 from, Vector3 to, Action<Vector3> onUpdate)
    {
        this.fromVector = from;
        this.toVector = to;
        this.onUpdate = onUpdate;
    }

    public override void UpdateValue(float t)
    {
        Vector3 current = Vector3.Lerp(fromVector, toVector, t);
        onUpdate?.Invoke(current);
    }

    public override void ReverseValues()
    {
        Vector3 temp = fromVector;
        fromVector = toVector;
        toVector = temp;
    }
}

public class ColorTweenValue : TweenValue
{
    private Color fromColor;
    private Color toColor;
    private Action<Color> onUpdate;

    public ColorTweenValue(Color from, Color to, Action<Color> onUpdate)
    {
        this.fromColor = from;
        this.toColor = to;
        this.onUpdate = onUpdate;
    }

    public override void UpdateValue(float t)
    {
        Color current = Color.Lerp(fromColor, toColor, t);
        onUpdate?.Invoke(current);
    }

    public override void ReverseValues()
    {
        Color temp = fromColor;
        fromColor = toColor;
        toColor = temp;
    }
} 
using System;
using UnityEngine;

public abstract class ScreenGesture<TIn, TOut>
{
    protected bool hasStarted = false;

    public delegate bool GetStartValueDelegate(int index, out TIn startValue);
    protected readonly GetStartValueDelegate GetStartValue;

    protected const float slopInches = 0.1f;

    public ScreenGesture(GetStartValueDelegate getStartValue) => GetStartValue = getStartValue;
    public abstract void StartOrUpdate(TIn currentValue, Action<TOut> onStarted, Action<TOut> onUpdated);
    public abstract void Complete(TIn currentValue, Action<TOut> onCompleted);
}

public struct Pinch
{
    public float amount;
    public Vector2 position;
}

public struct ScreenPoint
{
    public int screenPointId;
    public Vector2 position;
}
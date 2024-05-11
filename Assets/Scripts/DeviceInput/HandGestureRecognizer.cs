using System;
using UnityEngine;
using System.Collections.Generic;

public class HandGestureRecognizer
{
    public event Action<Pinch> PinchStarted, PinchChanged, PinchCompleted;
    public event Action<ScreenPoint> DragStarted, DragMoved, DragCompleted;
    public event Action<ScreenPoint> TapStarted, TapCompleted, LongTapCompleted;

    protected InputControl input;

    protected Tapping tapping;
    protected Tapping longtapping;
    protected Scrolling scrollGesture;
    protected Dragging dragging;
    protected Pinching pinching;

    private readonly Dictionary<int, ScreenPoint> screenPoints = new Dictionary<int, ScreenPoint>();

    public HandGestureRecognizer(InputControl input)
    {
        this.input = input;
        input.TouchBegan += OnTouchBegan;
        input.TouchMoved += OnTouchMoved;
        input.TouchStationary += OnTouchStationary;
        input.TouchEnded += OnTouchEnded;
        input.MouseScroll += OnMouseScroll;

        InitializeGestures();
    }

    public void Dispose()
    {
        if (null == input) return;

        input.TouchBegan -= OnTouchBegan;
        input.TouchMoved -= OnTouchMoved;
        input.TouchStationary -= OnTouchStationary;
        input.TouchEnded -= OnTouchEnded;
        input.MouseScroll -= OnMouseScroll;
    }

    private void OnTouchBegan(ScreenPoint screenPoint)
    {
        if (!screenPoints.ContainsKey(screenPoint.screenPointId)) screenPoints.Add(screenPoint.screenPointId, screenPoint);
        else screenPoints[screenPoint.screenPointId] = screenPoint;

        tapping.StartOrUpdate(screenPoint, TapStarted, null);
        longtapping.StartOrUpdate(screenPoint, null, null);
    }

    private void OnTouchMoved(ScreenPoint screenPoint)
    {
        if (!screenPoints.ContainsKey(screenPoint.screenPointId)) return;

        dragging.StartOrUpdate(screenPoint, DragStarted, DragMoved);
        pinching.StartOrUpdate(screenPoint, PinchStarted, PinchChanged);
        tapping.StartOrUpdate(screenPoint, null, null);
        longtapping.StartOrUpdate(screenPoint, null, null);
    }

    private void OnTouchStationary(ScreenPoint screenPoint)
    {
        if (!screenPoints.ContainsKey(screenPoint.screenPointId)) return;

        tapping.StartOrUpdate(screenPoint, null, null);
        longtapping.StartOrUpdate(screenPoint, null, null);
    }

    private void OnTouchEnded(ScreenPoint screenPoint)
    {
        if (!screenPoints.ContainsKey(screenPoint.screenPointId)) return;

        dragging.Complete(screenPoint, DragCompleted);
        pinching.Complete(screenPoint, PinchCompleted);
        tapping.Complete(screenPoint, TapCompleted);
        longtapping.Complete(screenPoint, LongTapCompleted);
        screenPoints.Remove(screenPoint.screenPointId);
    }

    private void OnMouseScroll(float rawScrollDelta)
    {
        if (Mathf.Epsilon <= Mathf.Abs(rawScrollDelta)) scrollGesture.StartOrUpdate(rawScrollDelta, PinchStarted, PinchChanged);
        else scrollGesture.Complete(rawScrollDelta, PinchCompleted);
    }

    private void InitializeGestures()
    {
        const float threshold = 0.5f;

        dragging = new Dragging(GetScreenPoint);
        pinching = new Pinching(GetScreenPoint);
        scrollGesture = new Scrolling((int i, out float s) => { s = 0; return true; });
        tapping = new Tapping(GetScreenPoint) { maxDuration = threshold };
        longtapping = new Tapping(GetScreenPoint) { minDuration = threshold };
    }

    private bool GetScreenPoint(int id, out ScreenPoint screenPoint) => screenPoints.TryGetValue(id, out screenPoint);
}

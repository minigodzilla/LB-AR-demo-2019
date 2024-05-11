using System;
using UnityEngine;

public class Tapping : ScreenGesture<ScreenPoint, ScreenPoint>
{
    public float minDuration = 0;
    public float maxDuration = Mathf.Infinity;
    
    private ScreenPoint screenPoint;
    private bool wasCancelled = false;
    private float duration = 0;

    public Tapping(GetStartValueDelegate getStartValue) : base(getStartValue) { }

    public override void StartOrUpdate(ScreenPoint pointer, Action<ScreenPoint> onStarted, Action<ScreenPoint> onUpdated)
    {
        if (!hasStarted && !wasCancelled) Start(pointer, onStarted);
        else Update(pointer, onUpdated);
    }

    public override void Complete(ScreenPoint screenPoint, Action<ScreenPoint> onCompleted)
    {
        wasCancelled = false;

        if (!hasStarted) { Reset(); return; }
        if (this.screenPoint.screenPointId != screenPoint.screenPointId) { Reset(); return; }
        if (duration < minDuration) { Reset(); return; }

        onCompleted?.Invoke(screenPoint);

        Reset();
    }

    private void Start(ScreenPoint screenPoint, Action<ScreenPoint> onStarted)
    {
        this.screenPoint = screenPoint;
        hasStarted = true;
        onStarted?.Invoke(screenPoint);
    }

    private void Update(ScreenPoint screenPoint, Action<ScreenPoint> onUpdated)
    {
        if (this.screenPoint.screenPointId != screenPoint.screenPointId) return;
        if (!GetStartValue(screenPoint.screenPointId, out ScreenPoint startPointer)) return;

        float diff = (screenPoint.position - startPointer.position).magnitude;

        if (InputExtensions.PixelsToInches(diff) >= slopInches) Cancel();
        else
        {
            duration += Time.deltaTime;

            if (duration > maxDuration) Cancel();
            else
            {
                this.screenPoint = screenPoint;
                onUpdated?.Invoke(screenPoint);
            }
        }
    }

    private void Cancel()
    {
        wasCancelled = true;
        Reset();
    }

    private void Reset()
    {
        hasStarted = false;
        duration = 0;
    }
}

public class Pinching : ScreenGesture<ScreenPoint, Pinch>
{
    private ScreenPoint screenPoint1;
    private ScreenPoint screenPoint2;
    private float lastGap;

    public Pinching(GetStartValueDelegate getStartValue) : base(getStartValue) { }

    public override void StartOrUpdate(ScreenPoint screenPoint, Action<Pinch> onStarted, Action<Pinch> onUpdated)
    {
        if (screenPoint.screenPointId == screenPoint1.screenPointId) screenPoint1 = screenPoint;
        else if (screenPoint.screenPointId == screenPoint2.screenPointId) screenPoint2 = screenPoint;
        else if (int.MaxValue == screenPoint1.screenPointId) screenPoint1 = screenPoint;
        else if (int.MaxValue == screenPoint2.screenPointId) screenPoint2 = screenPoint;

        if (screenPoint.screenPointId != screenPoint1.screenPointId && screenPoint.screenPointId != screenPoint2.screenPointId) return;
        if (screenPoint1.screenPointId == int.MaxValue || screenPoint2.screenPointId == int.MaxValue) return;

        if (!hasStarted) TryStart(onStarted);
        else Update(onUpdated);
    }

    public override void Complete(ScreenPoint screenPoint, Action<Pinch> onCompleted)
    {
        if (hasStarted) Update(onCompleted);
        Reset();
    }

    private void TryStart(Action<Pinch> pichStarted)
    {
        bool canStart = true;
        ScreenPoint startscreenPoint1;
        canStart &= GetStartValue(screenPoint1.screenPointId, out startscreenPoint1);
        ScreenPoint startscreenPoint2;
        canStart &= GetStartValue(screenPoint2.screenPointId, out startscreenPoint2);

        if (!canStart) return;

        float startGap = Gap(startscreenPoint1, startscreenPoint2);
        float gap = Gap(screenPoint1, screenPoint2);
        float gapDelta = InputExtensions.PixelsToInches(Mathf.Abs(gap - startGap));

        if (gapDelta >= slopInches)
        {
            hasStarted = true;
            pichStarted?.Invoke(MakeZoom((gap - startGap) / Screen.height));
        }

        lastGap = gap;
    }

    private void Update(Action<Pinch> onUpdated)
    {
        float gap = Gap(screenPoint1, screenPoint2);

        onUpdated?.Invoke(MakeZoom((lastGap - gap) / Screen.height));
        lastGap = gap;
    }

    private float Gap(ScreenPoint a, ScreenPoint b) => (a.position - b.position).magnitude;

    private Pinch MakeZoom(float gapDelta)
    {
        return new Pinch
        {
            amount = gapDelta,
            position = Vector2.Lerp(screenPoint1.position, screenPoint2.position, 0.5f)
        };
    }

    private void Reset()
    {
        screenPoint1.screenPointId = int.MaxValue;
        screenPoint2.screenPointId = int.MaxValue;
        hasStarted = false;
    }
}

public class Dragging : ScreenGesture<ScreenPoint, ScreenPoint>
{
    private ScreenPoint screenPoint;

    public Dragging(GetStartValueDelegate getStartValue) : base(getStartValue) { }

    public override void StartOrUpdate(ScreenPoint screenPoint, Action<ScreenPoint> onStarted, Action<ScreenPoint> onUpdated)
    {
        if (!hasStarted)
        {
            if (!GetStartValue(screenPoint.screenPointId, out ScreenPoint startscreenPoint)) return;

            float diff = (screenPoint.position - startscreenPoint.position).sqrMagnitude;

            if (InputExtensions.PixelsToInches(diff) >= slopInches)
            {
                this.screenPoint = screenPoint;
                hasStarted = true;
                onStarted?.Invoke(screenPoint);
            }
        }
        else
        {
            if (this.screenPoint.screenPointId == screenPoint.screenPointId) onUpdated?.Invoke(screenPoint);
        }
    }

    public override void Complete(ScreenPoint screenPoint, Action<ScreenPoint> onCompleted)
    {
        if (!hasStarted || this.screenPoint.screenPointId != screenPoint.screenPointId) return;

        onCompleted?.Invoke(screenPoint);
        hasStarted = false;
    }
}

public class Scrolling : ScreenGesture<float, Pinch>
{
    private const float scrollSpeedMultiplier = 0.03f;

    public Scrolling(GetStartValueDelegate getStartValue) : base(getStartValue) { }

    public override void Complete(float rawScroll, Action<Pinch> onCompleted)
    {
        if (!hasStarted) return;
        hasStarted = false;
        onCompleted?.Invoke(MakePinch(rawScroll));
    }

    public override void StartOrUpdate(float rawScrollDelta, Action<Pinch> onStarted, Action<Pinch> onUpdated)
    {
        if (TryStart()) onStarted?.Invoke(MakePinch(rawScrollDelta));
        else onUpdated?.Invoke(MakePinch(rawScrollDelta));
    }

    private bool TryStart()
    {
        if (hasStarted) return false;

        hasStarted = true;
        return true;
    }

    private Pinch MakePinch(float rawScrollDelta)
    {
        return new Pinch
        {
            amount = rawScrollDelta * scrollSpeedMultiplier,
            position = Input.mousePosition
        };
    }
}
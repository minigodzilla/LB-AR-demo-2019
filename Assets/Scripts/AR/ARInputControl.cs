using System;
using UnityEngine;

public class ARInputControl
{
    public float dragFrequency;

    private Action<GameObject> TapCompleted;
    private Action<GameObject, Vector3> DragStarted;
    private Action<Vector3> DragMoved;
    private Action DragEnded;
    private Action PinchStarted;
    private Action<Pinch> PinchChanged;
    private Action PinchEnded;

    private InputControl Input => Home.Resources.Input;
    private HandGestureRecognizer gestureRecognizer;

    private Camera camera;
    private bool isInversed;

    private float lastDragPos;

    public void Initialize(Camera camera)
    {
        this.camera = camera;
        gestureRecognizer = new HandGestureRecognizer(Input);

        Subscribe();
    }

    public void AssignInputEvents(Action<GameObject> TapCompleted, Action<GameObject, Vector3> DragStarted,
        Action<Vector3> DragMoved, Action DragEnded, Action PinchStarted, Action<Pinch> PinchChanged,
        Action PinchEnded, bool isInversed)
    {
        this.TapCompleted = TapCompleted;

        this.DragStarted = DragStarted;
        this.DragMoved = DragMoved;
        this.DragEnded = DragEnded;

        this.PinchStarted = PinchStarted;
        this.PinchChanged = PinchChanged;
        this.PinchEnded = PinchEnded;

        this.isInversed = isInversed;
    }

    public void Unsubscribe()
    {
        if (gestureRecognizer == null) return;

        gestureRecognizer.TapCompleted -= OnTapCompleted;

        gestureRecognizer.DragStarted -= OnDragStarted;
        gestureRecognizer.DragMoved -= OnDragMoved;
        gestureRecognizer.DragCompleted -= OnDragEnded;

        gestureRecognizer.PinchStarted -= OnPinchStarted;
        gestureRecognizer.PinchChanged -= OnPinchChanged;
        gestureRecognizer.PinchCompleted -= OnPinchEnded;
    }

    private void Subscribe()
    {
        gestureRecognizer.TapCompleted += OnTapCompleted;

        gestureRecognizer.DragStarted += OnDragStarted;
        gestureRecognizer.DragMoved += OnDragMoved;
        gestureRecognizer.DragCompleted += OnDragEnded;

        gestureRecognizer.PinchStarted += OnPinchStarted;
        gestureRecognizer.PinchChanged += OnPinchChanged;
        gestureRecognizer.PinchCompleted += OnPinchEnded;
    }

    private void OnTapCompleted(ScreenPoint screenPoint)
    {
        Ray raycast = camera.ScreenPointToRay(screenPoint.position);
        RaycastHit rayHit;

        if (Physics.Raycast(raycast, out rayHit))
        {
            TapCompleted?.Invoke(rayHit.transform.gameObject);
        }
    }

    private void OnDragStarted(ScreenPoint screenPoint)
    {
        if (DragStarted == null) return;

        Ray raycast = camera.ScreenPointToRay(screenPoint.position);
        RaycastHit rayHit;

        if (Physics.Raycast(raycast, out rayHit))
        {
            DragStarted?.Invoke(rayHit.transform.gameObject, screenPoint.position);
        }

        lastDragPos = screenPoint.position.x;
    }

    private void OnDragMoved(ScreenPoint screenPoint)
    {
        if (DragMoved == null) return;

        dragFrequency = Mathf.Abs(lastDragPos - screenPoint.position.x);

        if (isInversed)
        {
            Vector3 vector = new Vector3(screenPoint.position.x, screenPoint.position.y, camera.nearClipPlane);
            Vector3 screenPos = camera.ScreenToWorldPoint(vector);
            DragMoved?.Invoke(camera.transform.InverseTransformPoint(screenPos));
        }
        else DragMoved?.Invoke(screenPoint.position);

        lastDragPos = screenPoint.position.x;
    }

    private void OnDragEnded(ScreenPoint pointer) => DragEnded?.Invoke();

    private void OnPinchStarted(Pinch pinch) => PinchStarted?.Invoke();
    private void OnPinchChanged(Pinch pinch) => PinchChanged?.Invoke(pinch);
    private void OnPinchEnded(Pinch pinch) => PinchEnded?.Invoke();
}
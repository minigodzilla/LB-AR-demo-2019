using System;
using UnityEngine;
using UnityEngine.EventSystems;

[DisallowMultipleComponent]
public class InputControl : MonoBehaviour, IControl
{
    public enum TapResult { Succeeded, TooFar, Ignored }

    public Camera InteractionCam { get; private set; }

    public event Action<ScreenPoint> TouchBegan;
    public event Action<ScreenPoint> TouchMoved;
    public event Action<ScreenPoint> TouchEnded;
    public event Action<ScreenPoint> TouchStationary;

    public event Action<float> MouseScroll;

#if UNITY_EDITOR
    private bool wasMouseDown = false;
    private Vector2 oldMousePos = Vector2.zero;
#endif

    private const float pointerRadiusInches = 0.25f;
    private const int leftMouseButtonId = -1;

    public void Initialize(Home home, Action onInitialized)
    {
        InteractionCam = Camera.main;
        Input.simulateMouseWithTouches = false;
        onInitialized?.Invoke();
    }

    public float PointerRadiusWS(Camera cam)
    {
        if (!cam) cam = InteractionCam;
        return cam.PhysicalToWorldDistance(pointerRadiusInches);
    }

    private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene)
    { 
        InteractionCam = Camera.main;
    }

    private void Update()
    {
        HandleTouch();

#if UNITY_EDITOR
        if (Input.GetKey(KeyCode.Escape)) Application.Quit();

        HandleMouse(0, leftMouseButtonId);
#endif
    }

    private void HandleTouch()
    {
        for (int i = 0; i < Input.touchCount; i++)
        {
            Touch touch = Input.GetTouch(i);
            ScreenPoint screenPoint = new ScreenPoint() { position = touch.position, screenPointId = touch.fingerId };

            if (null != EventSystem.current && EventSystem.current.IsPointerOverGameObject(screenPoint.screenPointId)) return;

            switch (touch.phase)
            {
                case TouchPhase.Began: TouchBegan?.Invoke(screenPoint); break;
                case TouchPhase.Moved: TouchMoved?.Invoke(screenPoint); break;
                case TouchPhase.Stationary: TouchStationary?.Invoke(screenPoint); break;
                case TouchPhase.Ended: TouchEnded?.Invoke(screenPoint); break;
                case TouchPhase.Canceled: default: break;
            }
        }
    }

#if UNITY_EDITOR
    private void HandleMouse(int button, int pointerId)
    {
        MouseScroll?.Invoke(Input.mouseScrollDelta.y);

        ScreenPoint screenPoint = new ScreenPoint() { position = Input.mousePosition, screenPointId = pointerId };

        if (null != EventSystem.current && EventSystem.current.IsPointerOverGameObject(screenPoint.screenPointId)) return;

        bool isMouseDown = Input.GetMouseButton(button);

        if (!wasMouseDown && isMouseDown) TouchBegan?.Invoke(screenPoint);
        else if (wasMouseDown && !isMouseDown) TouchEnded?.Invoke(screenPoint);
        else if (wasMouseDown && isMouseDown)
        {
            if (oldMousePos != screenPoint.position) TouchMoved?.Invoke(screenPoint);
            else TouchStationary?.Invoke(screenPoint);
        }

        wasMouseDown = isMouseDown;
        oldMousePos = screenPoint.position;
    }
#endif
}

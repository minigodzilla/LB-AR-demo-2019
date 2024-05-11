using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TimelineInputControl : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    public bool clipEnded;
    public RawImage viewField;
    public RectTransform virtualAnchor;

    private ScrollRect mainScroll;
    private AnimationClip clip;
    private Animation locomotion;
    private Camera renderCam;

    private Vector3 lastPoint;
    private bool isDragging;
    private bool isInitialized;

    private float timelineSpeed;

    private const float smoothTime = 2.5f;
    private const float maxDragNormalizedTime = 0.9f;
    private const float minDragNormalizedTime = 0.1f;
    private const float maxNormalizedTime = 0.99f;
    private const float minNormalizedTime = 0.01f;

    public void OverrideReferences(RawImage viewField, RectTransform virtualAnchor, ScrollRect mainScroll, AnimationClip clip, Animation locomotion, Camera renderCam)
    {
        this.viewField = viewField;
        this.virtualAnchor = virtualAnchor;
        this.mainScroll = mainScroll;
        this.clip = clip;
        this.locomotion = locomotion;
        this.renderCam = renderCam;

        locomotion[clip.name].speed = 0;
        locomotion.Play(clip.name);
        isInitialized = true;
    }

    public void EnableRenderer(bool enable) => renderCam.TrySetEnable(enable);

    public void OnBeginDrag(PointerEventData eventData)
    {
        lastPoint = eventData.position;
        isDragging = true;
        timelineSpeed = 0;
    }

    public void OnDrag(PointerEventData eventData)
    {
        OverrideDragBehaviour(eventData);

        if (!isDragging) return;

        timelineSpeed = eventData.position.y - lastPoint.y;
        lastPoint = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData) => isDragging = false;

    private void Update()
    {
        if (!isInitialized) return;

        OverrideTimelineBehaviour();
        TimelineController();

        timelineSpeed = Mathf.Lerp(timelineSpeed, 0, Time.deltaTime * smoothTime);
    }

    private void TimelineController()
    {
        const float divider = 28;

        locomotion[clip.name].speed = timelineSpeed / divider;
    }

    private void OverrideDragBehaviour(PointerEventData eventData)
    {
        if ((lastPoint.y < eventData.position.y && locomotion[clip.name].normalizedTime >= maxDragNormalizedTime) ||
            (lastPoint.y > eventData.position.y && locomotion[clip.name].normalizedTime <= minDragNormalizedTime))
        {
            clipEnded = true;
            viewField.raycastTarget = false;
            mainScroll.enabled = true;
            locomotion[clip.name].speed = 0;
            timelineSpeed = 0;
            isDragging = false;
        }
    }

    private void OverrideTimelineBehaviour()
    {
        if (clipEnded) return;

        if ((locomotion[clip.name].normalizedTime <= minNormalizedTime ||
            locomotion[clip.name].normalizedTime >= maxNormalizedTime) &&
            Mathf.Abs(timelineSpeed) > 0 && !isDragging)
        {
            clipEnded = true;
            viewField.raycastTarget = false;
            mainScroll.enabled = true;
            locomotion[clip.name].speed = 0;
            timelineSpeed = 0;

            locomotion[clip.name].normalizedTime =
                locomotion[clip.name].normalizedTime <= minNormalizedTime ?
                minNormalizedTime : maxNormalizedTime;
        }
    }
}

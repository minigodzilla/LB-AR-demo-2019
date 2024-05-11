using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class ScrollControl : MonoBehaviour
{
    [Serializable]
    public class Content
    {
        public string description;
        public Color color;
    }

    [Serializable]
    public class RawInput
    {
        public RawImage viewField;
        public RectTransform virtualAnchor;
        public AnimationClip clip;
        public Content[] contents;
    }

    [SerializeField] private RenderTexture renderTexture = default;
    [SerializeField] private Camera mCam = default;
    [SerializeField] private TimelineRenderer timelineRenderer = default;
    [SerializeField] private ScrollRect scrollRect = default;
    [SerializeField] private Button homeButton = default;

    [SerializeField] private RawInput[] rawInputs = default;

    [SerializeField] private bool isKapyong;

    private Vector2 screenCenter = new Vector2(0.5f, 0.5f);

    private readonly List<TimelineInputControl> timelines = new List<TimelineInputControl>();

    private const float distLimit = 100.05f;
    private const string HomeScreen = "Home";
    private const string KapyongHomeScreen = "Kapyong_Home";

    private void Awake()
    {
        AssingReferences();
        AdjustScrollView();
        homeButton.onClick.AddListener(LoadHomeScreen);
    }

    private void Update()
    {
        UpdateScrollEvents();
    }

    private void LoadHomeScreen()
    {
        string scene = isKapyong ? KapyongHomeScreen : HomeScreen;
        SceneManager.LoadScene(scene);
    }

    private void AssingReferences()
    {
        for (int i = 0; i < rawInputs.Length; i++)
        {
            TimelineRenderer tr = Instantiate(timelineRenderer);
            tr.AssignTextBlocks(rawInputs[i].contents);
            Camera rc = tr.renderCam;
            Animation anim = rc.gameObject.GetComponent<Animation>();
            anim.clip = rawInputs[i].clip;
            anim.AddClip(rawInputs[i].clip, rawInputs[i].clip.name);
            anim.playAutomatically = true;

            TimelineInputControl controller = rawInputs[i].viewField.gameObject.AddComponent<TimelineInputControl>();
            controller.OverrideReferences(rawInputs[i].viewField, rawInputs[i].virtualAnchor, scrollRect, rawInputs[i].clip, anim, rc);
            timelines.Add(controller);
        }
    }

    private void AdjustScrollView()
    {
        renderTexture.height = Screen.height;
        renderTexture.width = Screen.width;
    }

    private void UpdateScrollEvents()
    {
        Vector2 viewport = mCam.ViewportToScreenPoint(screenCenter);
        TimelineInputControl target = ClosestTimeline();

        Vector2 rect = RectTransformUtility.WorldToScreenPoint(mCam, target.virtualAnchor.gameObject.transform.position);

        float distanceToTarget = Vector3.Distance(mCam.transform.position, target.virtualAnchor.gameObject.transform.position);
        float dist = Mathf.Abs(Mathf.Abs(viewport.y) - Mathf.Abs(rect.y));

        OverrideRenderers(target);

        if (distanceToTarget <= distLimit && !target.viewField.raycastTarget && !target.clipEnded)
        {
            scrollRect.enabled = false;
            target.viewField.raycastTarget = true;
        }

        if (distanceToTarget > distLimit)
        {
            target.clipEnded = false;
        }
    }

    private TimelineInputControl ClosestTimeline()
    {
        TimelineInputControl bestTarget = null;
        float closestDistance = Mathf.Infinity;

        for (int i = 0; i < timelines.Count; i++)
        {
            float distanceToTarget = Vector3.Distance(mCam.transform.position, timelines[i].virtualAnchor.gameObject.transform.position);

            if (distanceToTarget < closestDistance)
            {
                closestDistance = distanceToTarget;
                bestTarget = timelines[i];
            }
        }

        return bestTarget;
    }

    private void OverrideRenderers(TimelineInputControl target)
    {
        for (int i = 0; i < timelines.Count; i++)
        {
            timelines[i].EnableRenderer(target == timelines[i]);
        }
    }
}

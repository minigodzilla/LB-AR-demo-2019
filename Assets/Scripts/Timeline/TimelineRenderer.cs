using System;
using UnityEngine;

public class TimelineRenderer : MonoBehaviour
{
    [Serializable]
    public class Cardboard
    {
        public TextMesh textBlock;
        public SpriteRenderer label;
    }

    public Camera renderCam;
    public GameObject cardHolder;
    public Cardboard[] cardboards;

    public void AssignTextBlocks(ScrollControl.Content[] contents)
    {
        for (int i = 0; i < cardboards.Length; i++)
        {
            cardboards[i].textBlock.text = contents[i].description;
            cardboards[i].label.color = contents[i].color;
        }
    }

    private void Update()
    {
        cardHolder.TrySetActive(renderCam.enabled);
    }
}

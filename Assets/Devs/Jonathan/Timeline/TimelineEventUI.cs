using UnityEngine;
using UnityEngine.UI;

public class TimelineEventUI : MonoBehaviour 
{
    public Image icon;
    public float eventTime;

    public void SetData(Sprite iconSprite, float time)
        {
        eventTime = time;
        icon.sprite = iconSprite;
    }
}

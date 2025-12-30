using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using Citadel;

public class TimeUIController : MonoBehaviour
{
    [Header("Time Source")]
    public TimeManager timeManager;

    [Header("UI")]
    public TMP_Text dateText;
    public TMP_Text timeText;
    public Image circleProgress;

    [Header("Icons")]
    public Image dayStateIcon;
    public Sprite dayIcon;
    public Sprite eveningIcon;
    public Sprite nightIcon;

    //속도 조절 함수

    public void Pause()
    {
        timeManager.SetTimeScale(0f);
    }

    public void Play()
    {
        timeManager.SetTimeScale(1f);
    }

    public void Speed2x()
    {
        timeManager.SetTimeScale(2f);
    }

    public void Speed4x()
    {
        timeManager.SetTimeScale(4f);
    }


    void Start()
    {
        RefreshAll();
    }

    void OnEnable()
    {
        timeManager.OnHourChange += OnHourChanged;
    }

    void OnDisable()
    {
        timeManager.OnHourChange -= OnHourChanged;
    }

    void Update()
    {
        UpdateTimeText();
        UpdateProgress();
    }

    void OnHourChanged(int hour)
    {
        UpdateDayStateIcon(hour);
    }

    void RefreshAll()
    {
        UpdateTimeText();
        UpdateProgress();
        UpdateDayStateIcon(timeManager.TimeElapsed.Hour);
    }

    void UpdateTimeText()
    {
        DateTime t = timeManager.TimeElapsed;

        dateText.text = $"Day {t.Day}";
        timeText.text = $"{t.Hour:00}:{t.Minute:00}";
    }

    void UpdateProgress()
    {
        DateTime t = timeManager.TimeElapsed;
        float percent = (t.Hour * 60f + t.Minute) / 1440f;
        circleProgress.fillAmount = percent;
    }

    void UpdateDayStateIcon(int hour)
    {
        if (hour >= 6 && hour < 18)
            dayStateIcon.sprite = dayIcon;
        else if (hour >= 18 && hour < 21)
            dayStateIcon.sprite = eveningIcon;
        else
            dayStateIcon.sprite = nightIcon;
    }
}

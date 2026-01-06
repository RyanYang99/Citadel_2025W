using Citadel;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

    [Header("하이라이트")]
    public Button pauseButton;
    public Button playButton;
    public Button speed2xButton;
    public Button speed4xButton;



    //속도 조절 함수
    public void Pause()
    {
        timeManager.SetTimeScale(0f);
        ResetAll();
        Highlight(pauseButton);
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
        timeManager.OnTimeScaleChange += OnTimeScaleChanged;
        Highlight(playButton);
    }

    private void OnDestroy()
    {
        timeManager.OnTimeScaleChange -= OnTimeScaleChanged;
    }

    private float currentScale;
    // ===== 하이라이트 처리 =====
    private void OnTimeScaleChanged(float scale)
    {
       
        if (Mathf.Approximately(currentScale, scale))
            return;

        Debug.Log("TimeScale Changed: " + scale);

        ResetAll();
        if (scale <= 0.01f)
            Highlight(pauseButton);
        else if (scale < 1.5f)
            Highlight(playButton);
        else if (scale < 3f)
            Highlight(speed2xButton);
        else
            Highlight(speed4xButton);

    }
    private void ResetAll()
    {
        ResetButton(pauseButton);
        ResetButton(playButton);
        ResetButton(speed2xButton);
        ResetButton(speed4xButton);
    }

    private void Highlight(Button btn)
    {
        ToggleOutline(btn, true);
        btn.transform.localScale = Vector3.one * 1.15f;
    }

    private void ResetButton(Button btn)
    {
        ToggleOutline(btn, false);
        btn.transform.localScale = Vector3.one;
    }

    private void ToggleOutline(Button btn, bool on)
    {
        Outline outline = btn.GetComponent<Outline>();
        if (outline != null)
            outline.enabled = on;
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

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

    [Header("Arc Move (반원)")]
    public RectTransform arcRoot;     // 반원 중심
    public RectTransform sunIconRect;
    public RectTransform moonIconRect;
    public RectTransform cloudIconRect; 

    [Header("Progress Colors")]
    public Color dayFillColor = new Color(0.45f, 0.80f, 1.00f, 1f);     // 하늘색
    public Color eveningFillColor = new Color(0.90f, 0.65f, 0.35f, 1f); // 노을 
    public Color nightFillColor = new Color(0.10f, 0.15f, 0.35f, 1f);   // 남색

    public float arcRadius = 60f;     // 반원 반지름
    public float dayStartAngle = 180f; // 왼쪽
    public float dayEndAngle = 0f;     // 오른쪽
    public float nightStartAngle = 180f;
    public float nightEndAngle = 0f;

    //반원 메인 시간 프로그레스바

    Vector2 GetArcPos(float angleDeg, float radius)
    {
        float rad = angleDeg * Mathf.Deg2Rad;
        return new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)) * radius;
    }

    void UpdateDayStateAndArc(DateTime t)
    {
        float minutes = t.Hour * 60f + t.Minute;

        // 상태 구간
        bool isDay = minutes >= 360f && minutes < 1080f;          // 06-18
        bool isEvening = minutes >= 1080f && minutes < 1260f;     // 18-21
        bool isNight = !isDay && !isEvening;                      // 21-06

        //상태 아이콘 스왑
        if (dayStateIcon != null)
        {
            if (isDay) dayStateIcon.sprite = dayIcon;
            else if (isEvening) dayStateIcon.sprite = eveningIcon;
            else dayStateIcon.sprite = nightIcon;
        }

        //색 변경 
        if (circleProgress != null)
        {
            if (isDay) circleProgress.color = dayFillColor;
            else if (isEvening) circleProgress.color = eveningFillColor;
            else circleProgress.color = nightFillColor;
        }

        //반원 이동
        if (arcRoot == null) return;

        if (sunIconRect) sunIconRect.gameObject.SetActive(isDay);
        if (cloudIconRect) cloudIconRect.gameObject.SetActive(isEvening);
        if (moonIconRect) moonIconRect.gameObject.SetActive(isNight);

 
        float u;
        if (isDay)
        {
            u = Mathf.InverseLerp(360f, 1080f, minutes); // 06-18
            SetArcPos(sunIconRect, u);
        }
        else if (isEvening)
        {
            u = Mathf.InverseLerp(1080f, 1260f, minutes); // 18-21
            SetArcPos(cloudIconRect, u);
        }
        else // night 21-06
        {
            
            if (minutes >= 1260f) // 21-24
                u = Mathf.InverseLerp(1260f, 1440f, minutes) * 0.5f; // 0~0.5
            else // 0~6
                u = 0.5f + Mathf.InverseLerp(0f, 360f, minutes) * 0.5f; // 0.5~1

            SetArcPos(moonIconRect, u);
        }
    }

    void SetArcPos(RectTransform icon, float u01)
    {
        if (icon == null) return;

        float angle = Mathf.Lerp(dayStartAngle, dayEndAngle, u01);
        float rad = angle * Mathf.Deg2Rad;

        Vector2 pos = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)) * arcRadius;
        icon.anchoredPosition = pos;
    }

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
        UpdateDayStateAndArc(timeManager.TimeElapsed);
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

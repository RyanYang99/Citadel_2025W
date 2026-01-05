using UnityEngine;
using System;

//시대, 만족도 등급 구분
public enum Era { Medieval, Industrial, Modern }
public enum HappinessTier { worst, NotWell, Normal, satisfied, Happy }

public class HappinessManager : MonoBehaviour
{
    //싱글톤 인스턴스 설정
    public static HappinessManager Instance;

    [Header("Game State")]
    public Era currentEra = Era.Medieval; // 현재 시대 설정

    [Header("Input Stats (0 ~ 100)")]
    [Range(0, 100)] public float foodScore = 50f;
    [Range(0, 100)] public float religionScore = 50f;
    [Range(0, 100)] public float ecoScore = 50f;
    [Range(0, 100)] public float taxScore = 50f;

    [Header("Output Result")]
    [SerializeField] private float totalHappiness; // 최종 만족도
    public HappinessTier currentTier;              // 현재 만족도 등급


    public float PopMultiplier { get; private set; } = 1f;
    public float TaxMultiplier { get; private set; } = 1f;


    public float TotalHappiness => totalHappiness; //읽기전용

    // 티어가 변할 때만 알림을 주기 위한 변수
    private HappinessTier previousTier;

    // 다른 스크립트(UI, 효과 적용기)가 구독할 수 있는 이벤트
    public event Action<HappinessTier> OnTierChanged;

    void Awake()
    {
        // 싱글톤 초기화
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Update()
    {
        //만족도 계산
        totalHappiness = CalculateTotalHappiness();

        //티어 갱신 및 이벤트 발생
        UpdateTierState();
    }

    //시대별 가중치 계산 로직
    private float CalculateTotalHappiness()
    {
        float total = 0;
        switch (currentEra)
        {
            case Era.Medieval:
                // 중세: 음식(30%), 종교(50%), 환경(0%), 세금(20%)
                total = (foodScore * 0.3f) + (religionScore * 0.5f)+ (ecoScore * 0f) + (taxScore * 0.2f);
                break;
            case Era.Industrial:
                // 산업: 음식(30%), 종교(10%), 환경(10%), 세금(50%)
                total = (foodScore * 0.3f) + (religionScore * 0.1f) + (ecoScore * 0.1f) + (taxScore * 0.5f);
                break;
            case Era.Modern:
                // 현대: 음식(20%), 종교(10%), 환경(40%), 세금(30%)
                total = (foodScore * 0.2f) + (religionScore * 0.1f) + (ecoScore * 0.4f) + (taxScore * 0.3f);
                break;
        }
        return Mathf.Clamp(total, 0f, 100f);
    }

    // 점수에 따라 티어를 결정하는 로직
    private void UpdateTierState()
    {
        HappinessTier newTier = HappinessTier.Normal;

        if (totalHappiness > 90) newTier = HappinessTier.Happy;            //91~100
        else if (totalHappiness > 60) newTier = HappinessTier.satisfied;   //61~90
        else if (totalHappiness > 40) newTier = HappinessTier.Normal;      //41~60
        else if (totalHappiness > 10) newTier = HappinessTier.NotWell;     //11~40
        else newTier = HappinessTier.worst;                                 //0~10

        currentTier = newTier;

        //만족도에 따른 배율 수치
        switch (currentTier)
        {
            case HappinessTier.Happy:
                PopMultiplier = 2.0f; TaxMultiplier = 1.2f;
                break;
            case HappinessTier.satisfied:
                PopMultiplier = 1.3f; TaxMultiplier = 1.1f; 
                break;
            case HappinessTier.Normal:
                PopMultiplier = 1.0f; TaxMultiplier = 1.0f; 
                break;
            case HappinessTier.NotWell:
                PopMultiplier = 0.4f; TaxMultiplier = 0.8f; 
                break;
            case HappinessTier.worst:
                PopMultiplier = -1.0f; TaxMultiplier = 0.4f;
                break;
        }


        // 티어가 이전과 달라졌다면
        if (newTier != previousTier)
        {
            previousTier = newTier;

            // "티어가 바뀜" 이벤트 알림
            OnTierChanged?.Invoke(newTier);

            Debug.Log($"[Happiness System] 상태 변경됨: {newTier} (점수: {totalHappiness:F1})");
        }
    }
}
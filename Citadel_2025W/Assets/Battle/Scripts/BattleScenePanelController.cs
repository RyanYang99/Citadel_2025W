using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BattleScenePanelController : MonoBehaviour
{
    [SerializeField] private Button backButton;
    [SerializeField] private Button instantWinButton;
    [SerializeField] private BattleManager battleManager;

    //정상 릴리즈 버전 
    //    private void Awake()
    //    {
    //        backButton.onClick.AddListener(OnClickBack);

    //#if UNITY_EDITOR || DEVELOPMENT_BUILD
    //        instantWinButton.onClick.AddListener(() =>
    //        {
    //            battleManager.DebugForceWin();
    //        });
    //#endif
    //    }
    
    //for Beta Test
    private void Awake()
    {
        // 돌아가기
        backButton.onClick.AddListener(OnClickBack);

        // 바로 승리
        instantWinButton.onClick.AddListener(OnClickInstantWin);
    }
    private void OnClickBack()
    {
        SceneManager.LoadScene("MainScene");
    }

    private void OnClickInstantWin()
    {
        if (battleManager == null)
        {
            Debug.LogError("[BattleUI] BattleManager reference missing.");
            return;
        }

        battleManager.ForceWinAndExit();
    }
}

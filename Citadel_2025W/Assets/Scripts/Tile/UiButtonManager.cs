using UnityEngine;
using UnityEngine.UI;

public class UIButtonManager : MonoBehaviour
{

    public GameObject[] baseTilePrefabs;      // Inspector에서 등록할 타일 프리팹들
    public GameObject buttonTemplate;         // 버튼 프리팹 (Image + Button)
    public Transform buttonParent;            // 버튼들을 담을 Panel (GridLayoutGroup)


        void Start()
        {
            foreach (GameObject prefab in baseTilePrefabs)
            {
                GameObject btn = Instantiate(buttonTemplate, buttonParent);

                // 버튼 이미지 = 프리팹의 SpriteRenderer에서 가져오기
                SpriteRenderer sr = prefab.GetComponent<SpriteRenderer>();
                if (sr != null)
                    btn.GetComponent<Image>().sprite = sr.sprite;

                // 버튼 클릭 이벤트 연결
                btn.GetComponent<Button>().onClick.AddListener(() =>
                {
                  
                });
            }
        }
}

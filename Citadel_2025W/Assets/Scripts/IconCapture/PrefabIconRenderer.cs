using UnityEngine;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class PrefabIconRenderer : MonoBehaviour
{
    
        [Header("Render")]
        public Camera iconCamera;
        public RenderTexture renderTexture;

        [Header("렌더링 변환 프리팹")]
        public GameObject[] prefabs;  

        [Header("저장 위치")]
        public string saveFolder = "Assets/Icons/Generated/";

        public void RenderAll()
        {
            foreach (var prefab in prefabs)
            {
                if (prefab == null) continue;
                RenderAndSave(prefab);
            }
        }

    


    public Sprite RenderAndSave(GameObject prefab)
    {
        // 기존 자식 제거
        foreach (Transform child in transform)
            DestroyImmediate(child.gameObject);

        // 프리팹 생성
        GameObject obj = Instantiate(prefab, transform);
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localRotation = Quaternion.identity;
        obj.transform.localScale = Vector3.one;

        //렌더
        iconCamera.targetTexture = renderTexture;
        iconCamera.Render();
        RenderTexture.active = renderTexture;
        Texture2D tex = new Texture2D(
            renderTexture.width,
            renderTexture.height,
            TextureFormat.ARGB32,
            false
        );
        tex.ReadPixels(
            new Rect(0, 0, renderTexture.width, renderTexture.height),
            0, 0
        );
        tex.Apply();
        RenderTexture.active = null;

#if UNITY_EDITOR
        // 저장 폴더 보장
        if (!Directory.Exists(saveFolder))
            Directory.CreateDirectory(saveFolder);

        // PNG 저장
        string path = $"{saveFolder}{prefab.name}_Icon.png";
        File.WriteAllBytes(path, tex.EncodeToPNG());

        //Unity Asset으로 등록
        AssetDatabase.Refresh();

        TextureImporter importer =
            AssetImporter.GetAtPath(path) as TextureImporter;

        importer.textureType = TextureImporterType.Sprite;
        importer.spritePixelsPerUnit = 100;
        importer.alphaIsTransparency = true;
        importer.SaveAndReimport();

        //Sprite Asset 반환
        return AssetDatabase.LoadAssetAtPath<Sprite>(path);
#else
        // 런타임에서는 메모리 Sprite만 반환
        return Sprite.Create(
            tex,
            new Rect(0, 0, tex.width, tex.height),
            new Vector2(0.5f, 0.5f)
        );
#endif
    }
}

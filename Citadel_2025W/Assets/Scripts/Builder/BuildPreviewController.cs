using UnityEngine;
using static UnityEditor.PlayerSettings;

namespace Citadel
{
    public sealed class BuildPreviewController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Camera mainCamera;
        [SerializeField] private BuildingManager buildingManager;

        [Header("Layers")]
        [SerializeField] private LayerMask groundLayer;
        [SerializeField] private LayerMask buildingLayer;

        [Header("Preview Materials")]
        [SerializeField] private Material previewValidMat;
        [SerializeField] private Material previewInvalidMat;

        // Build Preview
        private GameObject buildPreviewInstance;
        private Renderer[] buildPreviewRenderers;

        // Destroy Preview
        private Renderer[] destroyTargetRenderers;
        private Material[][] destroyOriginalMaterials;

        private BuildMode currentMode = BuildMode.Build;

        private int currentRotationIndex = 0;
        private readonly float[] rotations = { 0f, 90f, 180f, 270f };

        public Quaternion CurrentRotation => buildPreviewInstance != null
        ? buildPreviewInstance.transform.rotation
        : Quaternion.identity;

        void Update()
        {
            if (currentMode == BuildMode.Build)
                UpdateBuildPreview();
            else if (currentMode==BuildMode.Destroy)
            UpdateDestroyPreview();
        }

        public void SetMode(BuildMode mode)
        {
            currentMode = mode;

            if (mode == BuildMode.Build)
                ClearDestroyPreview();
            else if(mode==BuildMode.None)
            {
                ClearDestroyPreview();
                ClearBuildPreview();
            }
            else
                ClearBuildPreview();
        }

        // BUILD PREVIEW
        private void OnEnable()
        {
            buildingManager.OnBuildingChanged += OnBuildingChanged;
        }

        private void OnDisable()
        {
            buildingManager.OnBuildingChanged -= OnBuildingChanged;
        }

        public void OnBuildingChanged() 
        {
            Debug.Log("Preview: Building Changed");

            ClearBuildPreview();
            CreateBuildPreview();
        }

        private void UpdateBuildPreview()
        {

            if (buildingManager.CurrentBuilding == null)
            {
                ClearBuildPreview();
                return;
            }

            if (!RaycastGround(out RaycastHit hit))
            {
                SetBuildPreviewVisible(false);
                return;
            }

            if (buildPreviewInstance == null)
                CreateBuildPreview();

            Vector3 pos = hit.collider.transform.position;
            buildPreviewInstance.transform.position = hit.point;
            buildPreviewInstance.SetActive(true);

            bool canPlace = buildingManager.CanPlaceBuildingAt(pos);
            ApplyMaterial(buildPreviewRenderers,
                canPlace ? previewValidMat : previewInvalidMat);

        }

        private void CreateBuildPreview()
        {
            ClearBuildPreview();

            currentRotationIndex = 0;

            buildPreviewInstance = Instantiate(
                buildingManager.CurrentBuilding.prefab,
                Vector3.zero,
                Quaternion.identity
            );

            buildPreviewInstance.name = "[BUILD PREVIEW]";

            foreach (Collider c in buildPreviewInstance.GetComponentsInChildren<Collider>())
                c.enabled = false;

            buildPreviewRenderers = buildPreviewInstance.GetComponentsInChildren<Renderer>();
            ApplyMaterial(buildPreviewRenderers, previewInvalidMat);

            Debug.Log("Preview created at " + buildPreviewInstance.transform.position);
        }

        public void RotatePreview()
        {
            if (buildPreviewInstance == null)
                return;

            currentRotationIndex = (currentRotationIndex + 1) % rotations.Length;

            buildPreviewInstance.transform.rotation =
                Quaternion.Euler(0f, rotations[currentRotationIndex], 0f);
        }


        private void ClearBuildPreview()
        {
            if (buildPreviewInstance != null)
                Destroy(buildPreviewInstance);

            buildPreviewInstance = null;
            buildPreviewRenderers = null;
        }

        private void SetBuildPreviewVisible(bool visible)
        {
            if (buildPreviewInstance != null)
                buildPreviewInstance.SetActive(visible);
        }

        // DESTROY PREVIEW
        private void UpdateDestroyPreview()
        {
            if (!RaycastBuilding(out RaycastHit hit))
            {
                ClearDestroyPreview();
                return;
            }

            Renderer[] renderers = hit.transform.GetComponentsInChildren<Renderer>();

            if (destroyTargetRenderers == renderers)
                return;

            ClearDestroyPreview();

            destroyTargetRenderers = renderers;
            destroyOriginalMaterials = new Material[renderers.Length][];

            for (int i = 0; i < renderers.Length; i++)
            {
                destroyOriginalMaterials[i] = renderers[i].materials;

                Material[] mats = new Material[renderers[i].materials.Length];
                for (int j = 0; j < mats.Length; j++)
                    mats[j] = previewInvalidMat;

                renderers[i].materials = mats;
            }
        }

        private void ClearDestroyPreview()
        {
            if (destroyTargetRenderers == null)
                return;

            for (int i = 0; i < destroyTargetRenderers.Length; i++)
            {
                if (destroyTargetRenderers[i] != null)
                    destroyTargetRenderers[i].materials = destroyOriginalMaterials[i];
            }

            destroyTargetRenderers = null;
            destroyOriginalMaterials = null;
        }

        // RAYCAST
        private bool RaycastGround(out RaycastHit hit)
        {
            return Physics.Raycast(
                mainCamera.ScreenPointToRay(Input.mousePosition),
                out hit,
                Mathf.Infinity,
                groundLayer
            );
        }

        private bool RaycastBuilding(out RaycastHit hit)
        {
            return Physics.Raycast(
                mainCamera.ScreenPointToRay(Input.mousePosition),
                out hit,
                Mathf.Infinity,
                buildingLayer
            );
        }

        // MATERIAL
        private void ApplyMaterial(Renderer[] renderers, Material mat)
        {
            foreach (Renderer r in renderers)
            {
                Material[] mats = new Material[r.materials.Length];
                for (int i = 0; i < mats.Length; i++)
                    mats[i] = mat;

                r.materials = mats;
            }
        }
    }
}

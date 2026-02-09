using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Citadel
{
    public class AnimationManager : MonoBehaviour
    {
        [Header("Prefabs")]
        [SerializeField] private GameObject workerPrefab;
        [SerializeField] private GameObject dustEffectPrefab;

        [Header("Settings")]
        [SerializeField] private float buildTime = 2.5f;
        [SerializeField] private int workersCount = 3;

        [Header("VFX Prefabs")]
        [SerializeField] private GameObject fireEffectPrefab;
        [SerializeField] private GameObject smokeEffectPrefab;

        // 물레방아 회전
        public class WheelRotator : MonoBehaviour
        {
            public float rotationSpeed = 50f;
            void Update()
            {
                transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);
            }
        }

        // 창고 톱니바퀴 회전
        public class SawRotator : MonoBehaviour
        {
            public float rotationSpeed = 100f;
            void Update()
            {
                transform.Rotate(Vector3.right * rotationSpeed * Time.deltaTime);
            }
        }

        public void ApplyConstructionEffect(GameObject building)
        {
            if (building == null) return;
            StartCoroutine(ConstructionRoutine(building));
        }
        public void PlayDustAndHide(GameObject building)
        {
            if (building == null) return;

            if (dustEffectPrefab != null)
            {
                GameObject dust = Instantiate(dustEffectPrefab, building.transform.position, Quaternion.identity);

                dust.transform.localScale = building.transform.localScale;

                Destroy(dust, 2.0f);
            }

            building.SetActive(false);
        }

        private IEnumerator ConstructionRoutine(GameObject building)
        {
            Vector3 targetPos = building.transform.position;
            Vector3 finalScale = building.transform.localScale;

            // 시작 시 건물을 납작하게
            building.transform.localScale = new Vector3(finalScale.x, 0.01f, finalScale.z);

            float buildingWidth = 1.0f;
            BoxCollider box = building.GetComponentInChildren<BoxCollider>();
            if (box != null)
            {
                buildingWidth = Mathf.Max(box.size.x * building.transform.lossyScale.x,
                                          box.size.z * building.transform.lossyScale.z) * 0.5f;
            }
            float spawnRadius = buildingWidth + 0.5f;

            GameObject dust = null;
            if (dustEffectPrefab) dust = Instantiate(dustEffectPrefab, targetPos, Quaternion.identity);

            // 일꾼 생성
            List<GameObject> workers = new List<GameObject>();
            for (int i = 0; i < workersCount; i++)
            {
                GameObject worker = Instantiate(workerPrefab, targetPos, Quaternion.identity);

                var agent = worker.GetComponent<UnityEngine.AI.NavMeshAgent>();
                if (agent != null) agent.enabled = false;

                float offsetAngle = i * (360f / workersCount);
                Vector3 rotationDir = Quaternion.Euler(0, offsetAngle, 0) * Vector3.forward;
                worker.transform.position = targetPos + (rotationDir * spawnRadius);

                workers.Add(worker);
            }

            // 건설 애니메이션 진행
            float elapsed = 0;
            float totalRotation = 360f * 0.5f;

            while (elapsed < buildTime)
            {
                elapsed += Time.deltaTime;
                float progress = elapsed / buildTime;
                float smoothProgress = Mathf.SmoothStep(0, 1, progress);

                building.transform.localScale = new Vector3(finalScale.x, finalScale.y * progress, finalScale.z);

                float currentRotationAngle = smoothProgress * totalRotation;

                for (int i = 0; i < workers.Count; i++)
                {
                    if (workers[i] == null) continue;

                    float offsetAngle = i * (360f / workersCount);
                    float finalAngle = offsetAngle + currentRotationAngle;

                    Vector3 rotationDir = Quaternion.Euler(0, finalAngle, 0) * Vector3.forward;
                    workers[i].transform.position = targetPos + (rotationDir * spawnRadius);
                    workers[i].transform.LookAt(targetPos);
                }

                yield return null;
            }

            // 건설 완료 처리
            building.transform.localScale = finalScale;
            if (dust) Destroy(dust);
            foreach (var w in workers) if (w != null) Destroy(w);

            ActivateWaterwheel(building.transform);
            ActivateSawWheel(building.transform);
            ActivateForgeFire(building.transform);
            ActivateSmoke(building.transform);
        }

        private void ActivateWaterwheel(Transform buildingTransform)
        {
            //  wheel 오브젝트를 찾기
            Transform wheel = FindChildRecursive(buildingTransform, "wheel");

            if (wheel != null)
            {
                if (wheel.gameObject.GetComponent<WheelRotator>() == null)
                {
                    wheel.gameObject.AddComponent<WheelRotator>();
                }
            }
        }

        private void ActivateSawWheel(Transform buildingTransform)
        {
            //  saw 오브젝트를 찾기
            Transform wheel = FindChildRecursive(buildingTransform, "saw");

            if (wheel != null)
            {
                if (wheel.gameObject.GetComponent<SawRotator>() == null)
                {
                    wheel.gameObject.AddComponent<SawRotator>();
                }
            }
        }

        private void ActivateForgeFire(Transform buildingTransform)
        {
            // FirePos 오브젝트 찾기
            Transform firePos = FindChildRecursive(buildingTransform, "FirePos");

            if (firePos != null && fireEffectPrefab != null)
            {
                GameObject fire = Instantiate(fireEffectPrefab, firePos.position, firePos.rotation);
                fire.transform.SetParent(firePos);
            }
        }

        private void ActivateSmoke(Transform buildingTransform)
        {
            // SmokePos 오브젝트 찾기
            Transform smokePos = FindChildRecursive(buildingTransform, "SmokePos");

            if (smokePos != null && smokeEffectPrefab != null)
            {
                GameObject smoke = Instantiate(smokeEffectPrefab, smokePos.position, smokePos.rotation);
                smoke.transform.SetParent(smokePos);
            }
        }

        private Transform FindChildRecursive(Transform parent, string nameSnippet)
        {
            foreach (Transform child in parent)
            {
                if (child.name.ToLower().Contains(nameSnippet.ToLower()))
                    return child;

                Transform found = FindChildRecursive(child, nameSnippet);
                if (found != null) return found;
            }
            return null;
        }
    }
}
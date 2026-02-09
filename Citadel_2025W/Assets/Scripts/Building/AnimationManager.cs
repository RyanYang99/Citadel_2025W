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

        [Header("VFX Settings")]
        [SerializeField] private Vector3 dustScale = new Vector3(0.5f, 0.5f, 0.5f);

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

            float offsetFromCenter = 1.0f;
            Vector3 spawnCenter = targetPos;

            Collider buildingCollider = building.GetComponentInChildren<Collider>();
            if (buildingCollider != null)
            {
                offsetFromCenter = buildingCollider.bounds.extents.x;
                spawnCenter = buildingCollider.bounds.center;
            }

            Vector3 spawnPos = targetPos + (building.transform.right * (offsetFromCenter + 0.7f));

            // 먼지 효과
            GameObject dust = null;
            if (dustEffectPrefab != null)
            {
                dust = Instantiate(dustEffectPrefab, spawnCenter, Quaternion.identity);
                dust.transform.localScale = dustScale;
            }

            // 시작 시 건물을 납작하게
            building.transform.localScale = new Vector3(finalScale.x, 0.01f, finalScale.z);

            // 일꾼 생성
            GameObject worker = Instantiate(workerPrefab, spawnPos, Quaternion.identity);
            worker.transform.LookAt(new Vector3(targetPos.x, worker.transform.position.y, targetPos.z));

            Animator workerAnim = worker.GetComponent<Animator>();

            if (workerAnim != null)
            {
                for (int i = 0; i < 3; i++)
                {
                    workerAnim.Play("attack-melee-right", 0, 0f);

                    yield return null;

                    AnimatorStateInfo stateInfo = workerAnim.GetCurrentAnimatorStateInfo(0);

                    while (stateInfo.normalizedTime < 1.0f)
                    {
                        stateInfo = workerAnim.GetCurrentAnimatorStateInfo(0);

                        float currentProgress = (i + Mathf.Clamp01(stateInfo.normalizedTime)) / 3f;
                        building.transform.localScale = new Vector3(
                            finalScale.x,
                            Mathf.Lerp(0.01f, finalScale.y, currentProgress),
                            finalScale.z
                        );

                        yield return null;
                    }
                }
            }

            // 건설 완료
            building.transform.localScale = finalScale;
            if (worker != null) Destroy(worker);
            if (dust != null) Destroy(dust);

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
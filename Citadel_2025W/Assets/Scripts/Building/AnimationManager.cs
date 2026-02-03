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

            building.transform.localScale = finalScale;
            if (dust) Destroy(dust);
            foreach (var w in workers) if (w != null) Destroy(w);
        }
    }
}
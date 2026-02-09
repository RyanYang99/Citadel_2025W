using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Citadel
{
    public class NpcController : MonoBehaviour
    {
        [Header("NPC 설정")]
        public GameObject npcPrefab;
        public int npcCount = 2;
        public float moveSpeed = 1.5f;

        private List<GameObject> residents = new List<GameObject>();
        private List<Vector3> targetPositions = new List<Vector3>();

        private float minRadius;
        private float maxRadius;
        private bool isInitialized = false;

        void Start()
        {
            if (gameObject.name.Contains("[BUILD PREVIEW]"))
            {
                this.enabled = false;
                return;
            }
            StartCoroutine(InitializeAfterAnimation());
        }

        IEnumerator InitializeAfterAnimation()
        {
            yield return new WaitForSeconds(0.2f);

            CalculateRadius();
            SpawnResidents();
            isInitialized = true;
        }

        void CalculateRadius()
        {
            MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer>();
            if (renderers.Length > 0)
            {
                Bounds combinedBounds = renderers[0].bounds;
                foreach (var renderer in renderers)
                {
                    combinedBounds.Encapsulate(renderer.bounds);
                }

                float buildingSize = Mathf.Max(combinedBounds.extents.x, combinedBounds.extents.z);

                minRadius = buildingSize + 0.5f;
                maxRadius = minRadius + 1.0f;
            }
            //else
            //{
            //    minRadius = 1.5f;
            //    maxRadius = 3.0f;
            //}
        }

        void SpawnResidents()
        {
            for (int i = 0; i < npcCount; i++)
            {
                Vector3 spawnPos = transform.position + GetRandomRingPoint();

                GameObject npc = Instantiate(npcPrefab, spawnPos, Quaternion.identity);

                residents.Add(npc);
                targetPositions.Add(transform.position + GetRandomRingPoint());
            }
        }

        void Update()
        {
            if (!isInitialized) return;

            for (int i = 0; i < residents.Count; i++)
            {
                if (residents[i] != null)
                {
                    MoveResident(i);
                }
            }
        }

        void MoveResident(int index)
        {
            GameObject npc = residents[index];
            Vector3 target = targetPositions[index];
            Animator anim = npc.GetComponent<Animator>();

            float distance = Vector3.Distance(npc.transform.position, target);

            if (distance > 0.1f)
            {
                npc.transform.position = Vector3.MoveTowards(npc.transform.position, target, moveSpeed * Time.deltaTime);
                Vector3 direction = (target - npc.transform.position).normalized;
                if (direction != Vector3.zero)
                    npc.transform.forward = direction;

                if (anim != null) anim.SetBool("isWalking", true);
            }
            else
            {
                if (anim != null) anim.SetBool("isWalking", false);
                targetPositions[index] = transform.position + GetRandomRingPoint();
            }
        }

        Vector3 GetRandomRingPoint()
        {
            float angle = Random.Range(0, 360f) * Mathf.Deg2Rad;
            Vector3 direction = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle));
            float distance = Random.Range(minRadius, maxRadius);
            return direction * distance;
        }
        
        private void OnDestroy()
        {
            foreach (var npc in residents)
            {
                if (npc != null)
                {
                    Destroy(npc);
                }
            }
        }
    }
}
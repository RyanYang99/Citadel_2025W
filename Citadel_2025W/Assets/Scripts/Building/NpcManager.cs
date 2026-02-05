using UnityEngine;
using System.Collections.Generic;

public class NpcController : MonoBehaviour
{
    public GameObject npcPrefab;
    public int npcCount = 2;
    public float moveSpeed = 1.5f;

    private List<GameObject> residents = new List<GameObject>();
    private List<Vector3> targetPositions = new List<Vector3>();

    private float minRadius;
    private float maxRadius;

    void Start()
    {
        CalculateRadius();
        SpawnResidents();
    }

    void CalculateRadius()
    {
        BoxCollider box = GetComponentInChildren<BoxCollider>();
        if (box != null)
        {
            float buildingSize = Mathf.Max(box.size.x * transform.lossyScale.x,
                                          box.size.z * transform.lossyScale.z) * 0.5f;

            minRadius = buildingSize + 0.8f;
            maxRadius = minRadius + 1.5f;
        }
        else
        {
            minRadius = 1.0f;
            maxRadius = 2.5f;
        }
    }

    void SpawnResidents()
    {
        for (int i = 0; i < npcCount; i++)
        {
            Vector3 spawnPos = transform.position + GetRandomRingPoint();
            GameObject npc = Instantiate(npcPrefab, spawnPos, Quaternion.identity, transform);
            residents.Add(npc);
            targetPositions.Add(transform.position + GetRandomRingPoint());
        }
    }

    void Update()
    {
        for (int i = 0; i < residents.Count; i++)
        {
            MoveResident(i);
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
            if (direction != Vector3.zero) npc.transform.forward = direction;

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
}
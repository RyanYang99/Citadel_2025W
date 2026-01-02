using UnityEngine;
using Citadel;

public class ReSourceUIController : MonoBehaviour
{
    [Header("Slots")]
    public ResourceUI[] slots;

    void Start()
    {
        // ResourceUI가 OnEnable에서 알아서 다 처리함
    }
}

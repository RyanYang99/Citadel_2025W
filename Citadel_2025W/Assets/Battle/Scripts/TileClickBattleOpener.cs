using Citadel;
using UnityEngine;
using UnityEngine.EventSystems;

public sealed class TileClickBattleOpener : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private BattleLauncher battleLauncher;


    [Header("Raycast")]
    [SerializeField] private Camera cam;
    [SerializeField] private LayerMask tileMask;
    [SerializeField] private float maxDistance = 500f;

    private void Awake()
    {
        if (cam == null) cam = Camera.main;
        if (battleLauncher == null) battleLauncher = FindFirstObjectByType<BattleLauncher>();
    }

    private void Update()
    {
        if (!Input.GetMouseButtonDown(0)) return;

        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;


        if (cam == null || battleLauncher == null) return;

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out RaycastHit hit, maxDistance, tileMask)) return;

        var tile = hit.collider.GetComponentInParent<LockedTile>();
        if (tile == null) return;

        if (!tile.Locked) return;

        battleLauncher.EnterBattle(tile.ZoneId, tile.Level);
    }
}
#if UNITY_EDITOR || DEVELOPMENT_BUILD

using UnityEngine;

namespace Citadel
{
    public sealed class LockedTileDebugInput : MonoBehaviour
    {
        [SerializeField] private int debugLevel = 1;
        [SerializeField] private int maxLevel = 20;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.LeftBracket))
                debugLevel = Mathf.Max(1, debugLevel - 1);

            if (Input.GetKeyDown(KeyCode.RightBracket))
                debugLevel = Mathf.Min(maxLevel, debugLevel + 1);

            if (Input.GetKeyDown(KeyCode.Return))
                LockedTile.UnlockByLevel(debugLevel);
        }
    }
}

#endif

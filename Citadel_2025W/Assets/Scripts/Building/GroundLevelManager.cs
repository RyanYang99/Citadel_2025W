using System;
using System.Linq;
using UnityEngine;

namespace Citadel
{
    public sealed class GroundLevelManager : MonoBehaviour
    {
        private int _currentLevel;
        public int CurrentLevel
        {
            get => _currentLevel;

            set
            {
                if (value < 0)
                {
                    Debug.LogError($"{CurrentLevel} can not be a negative number.");
                    return;
                }
                
                _currentLevel = value;
                if (_currentLevel == 0)
                    return;
                
                for (int i = 1; i <= _currentLevel; ++i)
                    Unlock(i);
                
                OnGroundLevelChanged?.Invoke();
            }
        }

        public Action OnGroundLevelChanged;

        private static void Unlock(int level)
        {
            LockedTile[] copy = LockedTile.LockedTiles.Where(lockedTile => lockedTile.Locked && lockedTile.Level == level).ToArray();
            
            foreach (LockedTile lockedTile in copy)
                lockedTile.Locked = false;
        }

        private void Update()
        {
            if (Input.GetKeyUp(KeyCode.Keypad1))
            {
                Debug.Log("1");
                CurrentLevel = 1;
            }
            
            if (Input.GetKeyUp(KeyCode.Keypad2))
            {
                Debug.Log("2");
                CurrentLevel = 2;
            }
        }
    }
}
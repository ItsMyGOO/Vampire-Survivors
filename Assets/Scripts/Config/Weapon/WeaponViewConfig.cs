using System;
using System.Collections.Generic;
using UnityEngine;

namespace ConfigHandler
{
    // =========================
    // JSON Root
    // =========================
    [Serializable]
    public class WeaponViewConfigRoot
    {
        public Dictionary<string, WeaponViewDef> weapons;
    }

    // =========================
    // Weapon View Definition
    // =========================
    [Serializable]
    public class WeaponViewDef
    {
        // UI
        public string name;
        public string icon;
        public string description;

        // Render / Entity
        public WeaponSpriteDef sprite;

        public void Validate(string weaponId)
        {
            if (string.IsNullOrEmpty(name))
                Debug.LogWarning($"[{weaponId}] view.name is empty");

            if (sprite == null)
            {
                Debug.LogWarning($"[{weaponId}] sprite config missing");
            }
            else
            {
                if (string.IsNullOrEmpty(sprite.sheet))
                    Debug.LogWarning($"[{weaponId}] sprite.sheet empty");
                if (string.IsNullOrEmpty(sprite.key))
                    Debug.LogWarning($"[{weaponId}] sprite.key empty");
            }
        }
    }

    // =========================
    // Sprite Definition
    // =========================
    [Serializable]
    public class WeaponSpriteDef
    {
        public string sheet;
        public string key;
    }
}
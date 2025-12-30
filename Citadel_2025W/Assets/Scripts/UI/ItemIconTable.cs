using UnityEngine;
using System;
using Citadel;

[Serializable]
public class ItemIconPair
{
    public Item item;
    public Sprite icon;
    public ItemCategory category;
}

public enum ItemCategory
{
    Basic,
    Advanced
}

public class ItemIconTable : MonoBehaviour
{
    public ItemIconPair[] items;

    public ItemIconPair Get(Item item)
    {
        foreach (var i in items)
            if (i.item == item)
                return i;

        return null;
    }
}

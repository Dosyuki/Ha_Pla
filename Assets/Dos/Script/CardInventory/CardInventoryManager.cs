using System;
using UnityEngine;
using System.Collections.Generic;

public class CardInventoryManager : Singleton<CardInventoryManager>
{
    [SerializeField] private List<Sprite> normalSprites = new List<Sprite>();
    [SerializeField] private List<Sprite> selectedSprites = new List<Sprite>();
    
    private Dictionary<FishRarity,Sprite> rarityNormalSprites = new Dictionary<FishRarity, Sprite>();
    private Dictionary<FishRarity,Sprite> raritySelectedSprites = new Dictionary<FishRarity, Sprite>();

    private void Start()
    {
        int index = 0;
        foreach (Sprite sprite in normalSprites)
        {
            rarityNormalSprites.Add((FishRarity)index, sprite);
            index++;
        }
        index = 0;
        foreach (Sprite sprite in selectedSprites)
        {
            raritySelectedSprites.Add((FishRarity)index, sprite);
            index++;
        }
    }

    public Sprite getSprite(FishRarity fishRarity,bool selected)
    {
        Debug.Log(fishRarity + ":" + selected);
        if (selected) return raritySelectedSprites[fishRarity];
        return rarityNormalSprites[fishRarity];
    }
}

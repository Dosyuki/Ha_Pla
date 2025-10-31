using UnityEngine;
using System.Collections.Generic;

public class FishManager : Singleton<FishManager>
{
    public List<BaseFish> fishPrefabsRedZone;
    [SerializeField] private List<BaseFish> zoneA_Fish;
    private Dictionary<string, BaseFish> fishLookup;
    protected void Awake()
    {
        BuildLookupDictionary();
    }
    public Fish RandomFish(float luckMultiplier = 1f, float weightMultiplier = 1f, Bait bait = null)
    {
        float totalChance = 0f;

        // รวมโอกาสสุ่ม โดยใช้ LuckMultiplier ช่วยเพิ่มโอกาสปลาหายาก
        Dictionary<BaseFish, float> adjustedChances = new Dictionary<BaseFish, float>();

        foreach (var fish in fishPrefabsRedZone)
        {
            float adjustedChance = fish.DropChance;

            // ถ้าปลามี Rarity สูง → ให้ LuckMultiplier มีผลมากขึ้น
            switch (fish.Rarity)
            {
                case FishRarity.Basic:
                    adjustedChance *= 1f; // Luck ไม่ช่วยปลาธรรมดา
                    break;
                case FishRarity.Rare:
                    adjustedChance *= Mathf.Lerp(1f, luckMultiplier, 0.6f);
                    break;
                case FishRarity.Epic:
                    adjustedChance *= luckMultiplier;
                    break;
                case FishRarity.Legendary:
                    adjustedChance *= luckMultiplier * 1.5f;
                    break;
            }

            adjustedChances[fish] = adjustedChance;
            totalChance += adjustedChance;
        }

        // ทอยลูกเต๋า
        float roll = Random.Range(0, totalChance);
        float cumulative = 0f;

        foreach (var kv in adjustedChances)
        {
            cumulative += kv.Value;
            if (roll <= cumulative)
            {
                return new Fish(kv.Key, luckMultiplier, weightMultiplier, bait);
            }
        }

        // fallback
        return new Fish(fishPrefabsRedZone[0], luckMultiplier, weightMultiplier, bait);
    }
    private void BuildLookupDictionary()
    {
        fishLookup = new Dictionary<string, BaseFish>();
        
        // (คุณอาจต้องรวมปลาจากทุก Zone ที่นี่)
        foreach (BaseFish fish in fishPrefabsRedZone)
        {
            if (fish != null && !fishLookup.ContainsKey(fish.Name))
            {
                fishLookup.Add(fish.Name, fish);
            }
        }
        // ... (เพิ่ม List ปลาจากโซนอื่น) ...
    }
    public BaseFish GetBaseFishByName(string name)
    {
        if (string.IsNullOrEmpty(name)) return null;
        
        fishLookup.TryGetValue(name, out BaseFish fish);
        if (fish == null)
            Debug.LogWarning($"BaseFish not found in database: {name}");
        return fish;
    }
}
using UnityEngine;
using System.Collections.Generic;

// สร้าง Struct เพื่อเก็บข้อมูลแยกแต่ละโซน
[System.Serializable]
public struct FishingZone
{
    public string ZoneName;        // ชื่อโซน (เช่น Zone A, Zone B)
    public LayerMask WaterLayer;   // Layer ของน้ำในโซนนี้
    public List<BaseFish> FishPool;// รายชื่อปลาที่ตกได้ในโซนนี้
}

public class FishManager : Singleton<FishManager>
{
    [Header("Fishing Zones Setup")]
    public List<FishingZone> fishingZones = new List<FishingZone>(); // แทนที่ fishPrefabsRedZone เดิม

    private Dictionary<string, BaseFish> fishLookup;

    protected void Awake()
    {
        BuildLookupDictionary();
    }

    // --- เพิ่ม Parameter waterLayerIndex เข้ามา ---
    public Fish RandomFish(int waterLayerIndex, float luckMultiplier = 1f, float weightMultiplier = 1f, float thrownLuck = 1f, Bait bait = null)
    {
        // 1. หา Pool ปลาจาก Layer ที่ส่งมา
        List<BaseFish> currentPool = GetPoolByLayer(waterLayerIndex);

        // ป้องกัน Error ถ้าหา Pool ไม่เจอ
        if (currentPool == null || currentPool.Count == 0)
        {
            Debug.LogError("No fish pool found for this layer!");
            return null;
        }

        float totalChance = 0f;
        Dictionary<BaseFish, float> adjustedChances = new Dictionary<BaseFish, float>();
        float newLuckMultipier = luckMultiplier + bait.LuckMultiplier + thrownLuck;

        // 2. ลูปสุ่มเฉพาะปลาใน currentPool
        foreach (var fish in currentPool)
        {
            float adjustedChance = fish.DropChance;

            switch (fish.Rarity)
            {
                case FishRarity.Basic:
                    adjustedChance *= 1f;
                    break;
                case FishRarity.Rare:
                    adjustedChance *= Mathf.Lerp(1f, newLuckMultipier, 0.6f);
                    break;
                case FishRarity.Epic:
                    adjustedChance *= newLuckMultipier;
                    break;
                case FishRarity.Legendary:
                    adjustedChance *= newLuckMultipier * 1.5f;
                    break;
            }

            adjustedChances[fish] = adjustedChance;
            totalChance += adjustedChance;
        }

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
        return new Fish(currentPool[0], luckMultiplier, weightMultiplier, bait);
    }

    // ฟังก์ชันเช็คว่า Layer ที่ตกน้ำไป ตรงกับ Zone ไหน
    private List<BaseFish> GetPoolByLayer(int layerIndex)
    {
        foreach (var zone in fishingZones)
        {
            // เช็คว่า layerIndex อยู่ใน LayerMask ของ Zone นี้หรือไม่
            if ((zone.WaterLayer.value & (1 << layerIndex)) != 0)
            {
                return zone.FishPool;
            }
        }
        
        // ถ้าหาไม่เจอจริงๆ ให้คืนค่าโซนแรก (Fallback)
        if (fishingZones.Count > 0) return fishingZones[0].FishPool;
        return null;
    }

    private void BuildLookupDictionary()
    {
        fishLookup = new Dictionary<string, BaseFish>();
        
        // ต้องลูปเอาปลาจาก "ทุกโซน" มาใส่ Dictionary ให้หมด เผื่อตอน Load Save
        foreach (var zone in fishingZones)
        {
            foreach (BaseFish fish in zone.FishPool)
            {
                if (fish != null && !fishLookup.ContainsKey(fish.Name))
                {
                    fishLookup.Add(fish.Name, fish);
                }
            }
        }
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
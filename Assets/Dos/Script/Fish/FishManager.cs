using UnityEngine;
using System.Collections.Generic;

public class FishManager : Singleton<FishManager>
{
    public List<BaseFish> fishPrefabsRedZone;

    public Fish RandomFish(float luckMultiplier = 1f, float weightMultiplier = 1f, Bait bait = null)
    {
        float totalChance = 0f;
        foreach (var fish in fishPrefabsRedZone)
        {
            totalChance += fish.DropChance;
        }

        float roll = Random.Range(0, totalChance);
        float cumulative = 0f;

        foreach (var fish in fishPrefabsRedZone)
        {
            cumulative += fish.DropChance;
            if (roll <= cumulative)
            {
                return new Fish(fish, luckMultiplier, weightMultiplier,bait);
            }
        }

        return new Fish(fishPrefabsRedZone[0], luckMultiplier, weightMultiplier,bait);
    }
}
using System;
using System.Collections.Generic;
using UnityEngine;

public class BaitManager : Singleton<BaitManager>
{
    [SerializeField] private List<BaseBait> allBait;
    private Dictionary<string, BaseBait> baitLookup;

    private void Awake()
    {
        BuildLookupDictionary();
    }

    public Bait GetBait(BaseBait bait,int amount = 0)
    {
        if (!allBait.Contains(bait))
        {
            Debug.LogError("Bait is not found");
        }

        return new Bait(bait,amount);
    }
    private void BuildLookupDictionary()
    {
        baitLookup = new Dictionary<string, BaseBait>();
        foreach (BaseBait bait in allBait)
        {
            if (bait != null && !baitLookup.ContainsKey(bait.Name)) //
            {
                baitLookup.Add(bait.Name, bait);
            }
        }
    }
    public BaseBait GetBaseBaitByName(string name)
    {
        if (string.IsNullOrEmpty(name)) return null;

        baitLookup.TryGetValue(name, out BaseBait bait);
        if (bait == null)
            Debug.LogWarning($"BaseBait not found in database: {name}");
        return bait;
    }
}

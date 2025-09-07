using System.Collections.Generic;
using UnityEngine;

public class BaitManager : Singleton<BaitManager>
{
    [SerializeField] private List<BaseBait> allBait;

    public Bait GetBait(BaseBait bait,int amount = 0)
    {
        if (!allBait.Contains(bait))
        {
            Debug.LogError("Bait is not found");
        }

        return new Bait(bait,amount);
    }
}

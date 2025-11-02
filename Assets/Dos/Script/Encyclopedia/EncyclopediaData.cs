// ไฟล์ใหม่: EncyclopediaData.cs
using System.Collections.Generic;

[System.Serializable]
public class EncyclopediaData
{
    // เราจะเซฟ List ของปลาแต่ละชนิดที่เราค้นพบ
    public List<EncyclopediaEntry> entries;
    public bool hasClaimedZoneAReward;

    public EncyclopediaData()
    {
        entries = new List<EncyclopediaEntry>();
        hasClaimedZoneAReward = false;
    }
}
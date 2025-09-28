using System;
using TMPro;
using UnityEngine;

public class DebugUI : MonoBehaviour
{
    [SerializeField] private TMP_Text DebugText;

    private void Update()
    {
        var rod = Inventory.Instance.CurrentRod;
        var bait = Inventory.Instance.currentBait;

        // If bait is null, default multipliers = 1
        float baitLuck = bait?.LuckMultiplier ?? 1f;
        float baitWeight = bait?.WeightMultiplier ?? 1f;

        float weatherLuck = TimeSystem.Instance.GetDynamicCondition()[0];
        float weatherWeight = TimeSystem.Instance.GetDynamicCondition()[1];

        DebugText.text =
            $"Luck Multiplier\n" +
            $"Fishing Rod : {rod.LuckMultiplier}\n" +
            $"Bait : {baitLuck}\n" +
            $"Weather Condition : {weatherLuck}\n" +
            $"Total : {1 * rod.LuckMultiplier * baitLuck * weatherLuck}\n\n" +
            $"Weight Multiplier\n" +
            $"Fishing Rod : {rod.WeightMultiplier}\n" +
            $"Bait : {baitWeight}\n" +
            $"Weather Condition : {weatherWeight}\n" +
            $"Total : {1 * rod.WeightMultiplier * baitWeight * weatherWeight}";
    }
}
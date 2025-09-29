using System;
using UnityEngine;

public class ShipStorage : MonoBehaviour
{
    [SerializeField] private LayerMask playerMask;
    [SerializeField] private CanvasGroup chestUIGroup;

    private bool playerInside = false;

    private void Start()
    {
        if (chestUIGroup == null)
            chestUIGroup = GetComponentInChildren<CanvasGroup>();

        chestUIGroup.alpha = 0;
    }

    private void Update()
    {
        if (!playerInside) return;

        if (Input.GetKeyDown(KeyCode.F))
        {
            InventoryUI.Instance.CreateCardUI(true);
            return;
        }
        else if (Input.GetKeyDown(KeyCode.R) && PlayerStats.Instance.GetMoney() >= Inventory.Instance.UpgradeCost())
        {
            Inventory.Instance.UpgradeTier();
            InventoryUI.Instance.UpdateText();
            PlayerStats.Instance.SetMoney(PlayerStats.Instance.GetMoney() - Inventory.Instance.UpgradeCost());
        }

        if (InventoryUI.Instance.gameObject.activeInHierarchy && 
            (Input.GetKeyDown(KeyCode.F) || Input.GetKeyDown(KeyCode.Escape)))
        {
            InventoryUI.Instance.CloseCardUI(InventorySource.Ship);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((playerMask.value & (1 << other.gameObject.layer)) > 0)
        {
            chestUIGroup.alpha = 1;
            playerInside = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if ((playerMask.value & (1 << other.gameObject.layer)) > 0)
        {
            chestUIGroup.alpha = 0;
            playerInside = false;

            // auto-close inventory when leaving
            if (InventoryUI.Instance.gameObject.activeInHierarchy)
                InventoryUI.Instance.CloseCardUI(InventorySource.Ship);
        }
    }
}
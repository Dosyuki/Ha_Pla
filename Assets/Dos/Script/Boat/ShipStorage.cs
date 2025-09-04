using System;
using UnityEngine;

public class ShipStorage : MonoBehaviour
{
    [SerializeField] private LayerMask playerMask;
    [SerializeField] private float hitboxSize;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
     private Collider[] colliders;
     private CanvasGroup chestUIGroup;
    void Start()
    {
        chestUIGroup = transform.GetComponentInChildren<CanvasGroup>();
    }

    // Update is called once per frame
    void Update()
    {
        colliders = Physics.OverlapBox(transform.position, (transform.lossyScale / 2f) * hitboxSize, Quaternion.identity, playerMask);
        if (colliders.Length > 0)
        {
            chestUIGroup.alpha = 1;
            if (Input.GetKeyDown(KeyCode.F))
            {
                InventoryUI.Instance.CreateCardUI(true);
                return;
            }
            else if (Input.GetKeyDown((KeyCode.R)) && PlayerStats.Instance.GetMoney() >= Inventory.Instance.UpgradeCost())
            {
                Inventory.Instance.UpgradeTier();
                InventoryUI.Instance.UpdateText();
                PlayerStats.Instance.SetMoney(PlayerStats.Instance.GetMoney() - Inventory.Instance.UpgradeCost());
            }
            else if (InventoryUI.Instance.gameObject.activeInHierarchy && (Input.GetKeyDown(KeyCode.F) 
                                                                           || Input.GetKeyDown(KeyCode.Escape)))
            {
                InventoryUI.Instance.CloseCardUI(InventorySource.Ship);
            }
        }
        else if(colliders.Length == 0)
            chestUIGroup.alpha = 0;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, transform.lossyScale * hitboxSize);
    }
}

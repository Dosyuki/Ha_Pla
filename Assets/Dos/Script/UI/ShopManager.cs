using System;
using System.Collections.Generic;
using UnityEngine;

public class ShopManager : Singleton<ShopManager>
{
    [SerializeField] private LayerMask playerMask;
    [SerializeField] private float hitboxSize;
    
    private Collider[] colliders;
    private CanvasGroup shopUIGroup;
    
    [SerializeField] private List<Fish> selectedFish = new List<Fish>();
    [SerializeField] private bool isOpen = false;

    private void Start()
    {
        shopUIGroup = transform.GetComponentInChildren<CanvasGroup>();
    }


    private void Update()
    {
        colliders = Physics.OverlapBox(transform.position, (transform.lossyScale / 2f) * hitboxSize, Quaternion.identity, playerMask);
        if (colliders.Length > 0)
        {
            shopUIGroup.alpha = 1;
            if (Input.GetKeyDown(KeyCode.F))
            {
                if (!isOpen)  // only open if not already open
                {
                    isOpen = true;
                    InventoryUI.Instance.CreateCardUI(false);
                    return;
                    // DO NOT call UIManager.ChangeState here
                }
            }
            if (isOpen && (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.F)))
            {
                isOpen = false;
                InventoryUI.Instance.CloseCardUI(InventorySource.Shop);
                selectedFish.Clear();
            }
        }
        else if (colliders.Length == 0)
            shopUIGroup.alpha = 0;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, transform.lossyScale * hitboxSize);
    }
    public void AddSelectedFish(Fish fish)
    {
        selectedFish.Add(fish);
    }

    public void RemoveSelectedFish(Fish fish)
    {
        selectedFish.Remove(fish);
    }
    public bool GetIsOpen() => isOpen;
    
}

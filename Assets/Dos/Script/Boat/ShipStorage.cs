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
        colliders = Physics.OverlapBox(transform.position, transform.lossyScale / 2f, Quaternion.identity, playerMask);
        if (colliders.Length > 0)
        {
            chestUIGroup.alpha = 1;
            if (Input.GetKeyDown(KeyCode.F))
                InventoryUI.Instance.CreateCardUI();
        }
        else if (colliders.Length == 0 ||
                 (InventoryUI.Instance.gameObject.activeInHierarchy && Input.GetKeyDown(KeyCode.F)))
        {
            InventoryUI.Instance.CloseCardUI();
            chestUIGroup.alpha = 0;
        }
        
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, transform.lossyScale * hitboxSize);
    }
}

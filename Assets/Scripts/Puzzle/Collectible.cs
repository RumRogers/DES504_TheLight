using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectible : MonoBehaviour
{
    [SerializeField] Inventory.InventoryItems m_itemID;
    [SerializeField] protected bool m_pickedUp = false;

    private void Update()
    {
        transform.Rotate(0, 1, 0);
    }

    private void OnTriggerStay(Collider other)
    {
        if(m_pickedUp)
        {
            return;
        }

        if(other.CompareTag("Player") && Input.GetButtonDown("Action"))
        {
            Inventory.Instance.PickUp(m_itemID);
            m_pickedUp = true;
            gameObject.SetActive(false);
            print(string.Format("{0} added to inventory", Inventory.Instance.GetItemName(m_itemID)));
        }
    }
}

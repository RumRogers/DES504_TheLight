using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Collectible : Resettable
{
    [SerializeField] Inventory.InventoryItems m_itemID;
    [SerializeField] protected bool m_pickedUp = false;
    [SerializeField] private Vector3 m_rotationAxis = new Vector3(0, 1, 0);

    private void Update()
    {
        transform.Rotate(m_rotationAxis, Space.World);
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

    public override void Reset()
    {
        base.Reset();
        m_pickedUp = false;
    }
}

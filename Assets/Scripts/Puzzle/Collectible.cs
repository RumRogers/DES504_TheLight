using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Collectible : Resettable
{
    private static string m_msgPrefix = "You grabbed a";
    private static string m_msgPostfix = "Use it wisely.";
    private static string m_msgTimerStarting = "You got the money... Time to abscond!";

    [SerializeField] Inventory.InventoryItems m_itemID;
    [SerializeField] protected bool m_pickedUp = false;
    [SerializeField] private Vector3 m_rotationAxis = new Vector3(0, 1, 0);
    private Transform m_particles;

    private void Awake()
    {
        m_particles = transform.Find("CollectibleParticles");
        if(m_particles != null)
        {
            m_particles.SetParent(m_particles.parent.parent);
            m_particles.localScale = new Vector3(1f, 1f, 1f);
        }
    }
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
            if(m_particles != null)
            {
                m_particles.gameObject.SetActive(false);
            }
            gameObject.SetActive(false);

            switch(m_itemID)
            {
                case Inventory.InventoryItems.Gold:                    
                    GameManager.Instance.SetLowerHUDText(m_msgTimerStarting, 2.5f);
                    GameManager.Instance.StartTimer();                    
                    GameManager.Instance.UpdatePlayerRespawnPoint(transform.position);
                    break;
                default:
                    GameManager.Instance.UIAddToInventory(m_itemID);                    
                    GameManager.Instance.SetLowerHUDText(m_msgPrefix + " " + Inventory.Instance.GetItemName(m_itemID) + ". Use it wisely.", 2.5f);
                    break;
            }
            
            //print(string.Format("{0} added to inventory", Inventory.Instance.GetItemName(m_itemID)));
        }
    }

    public override void Reset()
    {
        base.Reset();
        m_pickedUp = false;
    }
}

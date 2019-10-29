using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory
{
    public enum InventoryItems
    {
        Crowbar, MonkeyWrench, Gold, None
    }

    private const int m_possibleItems = 3;
    private int[] m_itemsCarried = new int[m_possibleItems];
    private static Inventory m_instance = null;
    private Dictionary<InventoryItems, string> m_itemNames = new Dictionary<InventoryItems, string>();
    //public 


    public static Inventory Instance {
        get
        {
            if (m_instance == null)
            {
                m_instance = new Inventory();
            }

            return m_instance;
        }
    }


    private Inventory()
    {
        m_itemNames[InventoryItems.Crowbar] = "Crowbar";
        m_itemNames[InventoryItems.MonkeyWrench] = "Monkey wrench";
        m_itemNames[InventoryItems.Gold] = "Loot";
    }

    
    public void PickUp(InventoryItems item)
    {
        SoundManager.Instance.PlaySound(SoundManager.SoundID.ItemPickUp);
        m_itemsCarried[(int)item]++;
        if(item == InventoryItems.Gold)
        {
            GameManager.Instance.ShowScreen(GameManager.UIScreen.MissionComplete);
        }
    }

    public string GetItemName(InventoryItems item)
    {
        return m_itemNames[item];
    }

    public bool ContainsItem(InventoryItems item)
    {
        return m_itemsCarried[(int)item] > 0;
    }

    public void Empty()
    {
        for(int i = 0; i < m_possibleItems; i++)
        {
            m_itemsCarried[i] = 0;
        }
    }
}

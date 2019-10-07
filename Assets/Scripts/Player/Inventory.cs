using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory
{
    public enum InventoryItems
    {
        Crowbar, MonkeyWrench, Gold
    }

    private const int m_PossibleItems = 3;
    private int[] m_itemsCarried = new int[m_PossibleItems];
    private static Inventory m_instance = null;
    private Dictionary<InventoryItems, string> m_itemNames = new Dictionary<InventoryItems, string>();

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
        m_itemsCarried[(int)item]++;
    }

    public string GetItemName(InventoryItems item)
    {
        return m_itemNames[item];
    }
}

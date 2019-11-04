using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitLevel : MonoBehaviour
{
    private void OnTriggerStay(Collider other)
    {
        if(Input.GetButtonDown("Action"))
        {
            if(Inventory.Instance.ContainsItem(Inventory.InventoryItems.Gold))
            {
                GameManager.Instance.ShowScreen(GameManager.UIScreen.MissionComplete, "You did it! Nice job, dude!");
            }
            else
            {
                GameManager.Instance.SetLowerHUDText("You need to steal the money first, you moron.");
            }
        }
    }
}

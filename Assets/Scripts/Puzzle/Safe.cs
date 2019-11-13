using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Safe : MonoBehaviour
{
    private Animator m_animator;
    private Collider m_collider;
    private PlayerController m_playerController;
    [SerializeField] private string m_useBareHandsString = "You obviously can't open it with your bare hands.";
    [SerializeField] private string m_useMonkeyWrenchString = "You try to hit the safe with the wrench, but it reveals to be useless.";

    private void Awake()
    {
        m_animator = GetComponent<Animator>();
        m_collider = GetComponent<Collider>();
        m_playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    private void OnTriggerStay(Collider other)
    {
        if(Input.GetButtonDown("Action"))
        {
            switch(m_playerController.CurrentItem)
            {
                case Inventory.InventoryItems.None:
                    GameManager.Instance.SetLowerHUDText(m_useBareHandsString, 3f);
                break;
                case Inventory.InventoryItems.MonkeyWrench:
                    GameManager.Instance.SetLowerHUDText(m_useMonkeyWrenchString, 3f);
                    break;
                case Inventory.InventoryItems.Crowbar:
                    m_collider.enabled = false;
                    m_animator.SetTrigger("breakOpen");
                break;
            }
        }
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeath : MonoBehaviour
{
    [SerializeField] private CameraFollowTarget m_cameraFollowTarget;    
    private PlayerController m_playerController;
    private StateManager m_stateManager;

    private void Awake()
    {
        m_stateManager = GameObject.Find("StateManager").GetComponent<StateManager>();
        m_playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            //m_playerController.Respawn();           
            //Inventory.Instance.Empty();
            //m_stateManager.ResetAll();
            GameManager.Instance.ShowScreen(GameManager.UIScreen.MissionFailed, "You fell to your death. Whoops.");
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeath : MonoBehaviour
{
    [SerializeField] private CameraFollowTarget m_cameraFollowTarget;
    [SerializeField] private Transform m_respawnPoint;
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
            m_playerController.Respawn(m_respawnPoint.position);           
            Inventory.Instance.Empty();
            m_stateManager.ResetAll();
        }
    }
}

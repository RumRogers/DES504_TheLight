using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Respawn_judge : MonoBehaviour
{
    [SerializeField] private PlayerController m_playerController;
    [SerializeField] private CheckingPoints m_respawnpoints;
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player") && !m_playerController.Dead)
        {
            m_respawnpoints.changeCheckingPoints(this.transform);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorsCheckerManager : MonoBehaviour
{
    [SerializeField] private PlayerController m_playerController;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            if(!m_playerController.ChangingFloor && !m_playerController.Dead)
            {
                m_playerController.Die();
            }
        }
    }
}

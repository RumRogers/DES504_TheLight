using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckingPoints : MonoBehaviour
{
   // [SerializeField] private PlayerController m_playerController;
    public Transform[] m_repawn;
    public void changeCheckingPoints(Transform respawn)
    {
        for(int i = 0; i < m_repawn.Length; i++)
        {
            if (m_repawn[i].position == respawn.position)
                transform.position = m_repawn[i].position;
        }
    }
}

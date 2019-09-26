using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeath : MonoBehaviour
{
    [SerializeField] private Transform m_respawnPoint;

    private void OnTriggerEnter(Collider other)
    {
        print("heh");
        if(other.name.Equals("Player"))
        {
            other.transform.position = m_respawnPoint.position;
        }
    }
}

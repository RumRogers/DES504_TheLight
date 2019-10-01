using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pipe : MonoBehaviour
{
    enum PipeDirection
    {
        Left,
        Right
    }

    [SerializeField] private PipeDirection m_direction;
    [SerializeField] private bool m_isStuck = false;
    private PlayerController m_playerController;

    private void Awake()
    {

        GameObject player = GameObject.Find("Player");
        if(player != null)
        {
            m_playerController = player.GetComponent<PlayerController>();
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            m_playerController.SetCurrentPipe(gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            m_playerController.SetCurrentPipe(null);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pipe : MonoBehaviour
{
    public enum PipeDirection
    {
        Left,
        Right
    }

    [SerializeField] private PipeDirection m_direction;
    [SerializeField] private Transform m_pipeJumpHotspot;
    [SerializeField] private Transform m_pipeEnd;
    [SerializeField] private bool m_isStuck = false;
    
    private PlayerController m_playerController;

    private void Awake()
    {

        GameObject gameObject = GameObject.Find("Player");
        if(gameObject != null)
        {
            m_playerController = gameObject.GetComponent<PlayerController>();
        }

      
    }

    /*private void OnTriggerEnter(Collider other)
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
    }*/

    private void OnTriggerStay(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            if(Input.GetButtonDown("Action"))
            {
                StartCoroutine(m_playerController.GrabPipe(m_pipeJumpHotspot.position, m_pipeEnd.position, m_direction));
            }
        }
    }
}

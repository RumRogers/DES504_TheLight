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
    [SerializeField] private bool m_isStuck = false;
    private PlayerController m_playerController;
    [SerializeField] private PlayerTimeline m_playerTimeline;

    private void Awake()
    {

        GameObject gameObject = GameObject.Find("Player");
        if(gameObject != null)
        {
            m_playerController = gameObject.GetComponent<PlayerController>();
        }

        gameObject = GameObject.Find("PlayerTimeline");
        if(gameObject != null)
        {
            m_playerTimeline = gameObject.GetComponent<PlayerTimeline>();
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

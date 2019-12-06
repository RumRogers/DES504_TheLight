using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hotspot : MonoBehaviour
{
    [SerializeField] private string m_hotspotMessage;
    private BoxCollider m_collider;
    private Transform m_player;
    private bool m_displaying = false;

    private void Awake()
    {
        m_player = GameObject.FindGameObjectWithTag("Player").transform;
        m_collider = GetComponent<BoxCollider>();
    }

    private void Update()
    {
        if (!m_displaying && m_collider.bounds.Contains(m_player.position))
        {
            m_displaying = true;
            GameManager.Instance.SetLowerHUDText(m_hotspotMessage);
        }
        else if(m_displaying && !m_collider.bounds.Contains(m_player.position))
        {
            m_displaying = false;
            GameManager.Instance.SetLowerHUDText("");
        }
        
    }
}

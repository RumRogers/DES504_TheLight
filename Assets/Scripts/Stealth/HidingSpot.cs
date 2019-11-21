using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HidingSpot : MonoBehaviour
{
    private Transform m_hidingLocation;
    private float m_defaultZPos;
    private bool m_occupied = false;
    private float m_waitingTime = 0;
    private const float m_maxWaitingTime = .1f;
    private PlayerController m_playerController;
    private void Awake()
    {
        m_hidingLocation = transform.GetChild(0);
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        Debug.Assert(player != null, "Player is null!");
        m_playerController = player.GetComponent<PlayerController>();
        m_defaultZPos = player.transform.position.z;
    }

    private void Update()
    {
        if(m_waitingTime > 0)
        {
            m_waitingTime = Mathf.Max(m_waitingTime - Time.deltaTime, 0);
        }
    }

    private void OnTriggerStay(Collider other)
    {        
        if(m_waitingTime > 0 || m_playerController.GotBusted)
        {
            return;
        }

        if(Input.GetAxis("Vertical") != 0)
        {
            m_waitingTime = m_maxWaitingTime;
        }

        if(Input.GetAxis("Vertical") > 0)
        {
            m_occupied = true;
            StartCoroutine(m_playerController.HideTo(m_hidingLocation.position));
        }
        else if(Input.GetAxis("Vertical") < 0)
        {
            m_occupied = false;

            Vector3 oldPos = m_hidingLocation.position;
            oldPos.z = m_defaultZPos;
            StartCoroutine(m_playerController.LeaveHidingSpot(oldPos));
        }
    }
}

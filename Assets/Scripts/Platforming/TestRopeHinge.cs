using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestRopeHinge : MonoBehaviour
{
    private Rigidbody m_rigidbody;
    [SerializeField] private Vector3 m_force;
    [SerializeField] private float m_forceMultiplier;
    [SerializeField] private Transform m_playerTransform;
    [SerializeField] private bool m_playerAttached = false;
   
    private Rigidbody m_playerRigidbody;
    private PlayerController m_playerController;

    private void Awake()
    {
        m_rigidbody = GetComponent<Rigidbody>();
        m_playerRigidbody = m_playerTransform.GetComponent<Rigidbody>();
        m_playerController = m_playerTransform.GetComponent<PlayerController>();
    }

    private void FixedUpdate()
    {
        if(m_playerAttached)
        {
            float x = Input.GetAxis("Horizontal");

            if(Input.GetButtonDown("Jump"))
            {
                m_playerAttached = false;
                m_playerTransform.SetParent(null);
            }
            else if (x != 0)
            {
                m_rigidbody.AddForce(x * m_force * m_forceMultiplier);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            other.transform.SetParent(transform);
            m_playerAttached = true;
            m_playerController.IgnoreInput = true;
            m_playerController.Swinging = true;
            //m_rigidbody.
        }
    }
}

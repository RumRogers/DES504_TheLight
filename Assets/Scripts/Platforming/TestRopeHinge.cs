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
    private Animator m_playerAnimator;
    private BoxCollider m_triggerCollider;
    
    private void Awake()
    {
        m_rigidbody = GetComponent<Rigidbody>();
        m_playerRigidbody = m_playerTransform.GetComponent<Rigidbody>();
        m_playerController = m_playerTransform.GetComponent<PlayerController>();
        m_playerAnimator = m_playerTransform.GetComponent<Animator>();
        m_triggerCollider = GetComponent<BoxCollider>();
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
                m_playerAnimator.enabled = true;
                m_playerController.IgnoreInput = false;
                m_playerController.Swinging = false;
                StartCoroutine(DeactivateTriggerForSeconds(1f));
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
            m_playerAnimator.enabled = false;
            //m_rigidbody.
        }
    }

    private IEnumerator DeactivateTriggerForSeconds(float seconds)
    {
        m_triggerCollider.enabled = false;
        yield return new WaitForSeconds(seconds);
        m_triggerCollider.enabled = true;
    }
}

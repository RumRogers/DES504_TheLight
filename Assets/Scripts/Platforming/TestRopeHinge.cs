using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: add jump force to player when he leaves the platform
public class TestRopeHinge : MonoBehaviour
{
    private Rigidbody m_rigidbody;
    [SerializeField] private Vector3 m_force;
    [SerializeField] private float m_forceMultiplier;
    private Transform m_playerTransform;
    [SerializeField] private bool m_playerAttached = false;
    
    private Rigidbody m_playerRigidbody;
    private PlayerController m_playerController;
    private Animator m_playerAnimator;
    private BoxCollider m_triggerCollider;
    private Transform m_anchorPoint;
    
    private void Awake()
    {
        m_anchorPoint = transform.Find("AnchorPoint");
        m_rigidbody = GetComponent<Rigidbody>();
        //m_playerTransform = GameObject.Find("Player").transform;
        m_playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        m_playerRigidbody = m_playerTransform.GetComponent<Rigidbody>();
        m_playerController = m_playerTransform.GetComponent<PlayerController>();
        m_playerAnimator = m_playerTransform.GetComponent<Animator>();
        m_triggerCollider = GetComponent<BoxCollider>();
    }

    private void FixedUpdate()
    {
        if(m_playerAttached)
        {
            //m_playerTransform.transform
            float x = Input.GetAxis("Horizontal");

            if(Input.GetButtonDown("Jump"))
            {
                m_playerAttached = false;
                StartCoroutine(m_playerController.StopSwinging());
                //m_playerController.StopSwinging();
                /*m_playerTransform.SetPositionAndRotation(m_playerTransform.position, Quaternion.identity);
                m_playerTransform.SetParent(null);
                print("Reactivating player animator...");
                m_playerAnimator.enabled = true;
                m_playerController.IgnoreInput = false;
                m_playerController.Swinging = false;*/
                StartCoroutine(DeactivateTriggerForSeconds(1f));
            }
            else if (x != 0)
            {
                m_rigidbody.AddForce(x * m_force * m_forceMultiplier);
            }
        }
        else
        {
            m_rigidbody.AddForce(-m_rigidbody.velocity);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            m_playerTransform.SetParent(transform);           
            m_playerAttached = true;
            StartCoroutine(m_playerController.DoSwing(m_anchorPoint.position));
        }
    }

    private IEnumerator DeactivateTriggerForSeconds(float seconds)
    {
        m_triggerCollider.enabled = false;
        yield return new WaitForSeconds(seconds);
        m_triggerCollider.enabled = true;
    }
}

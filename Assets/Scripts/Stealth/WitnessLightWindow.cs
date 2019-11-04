using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WitnessLightWindow : MonoBehaviour
{
    private Transform m_witnessBody;
    private Transform m_light;
    private Transform m_balloon;
    private BoxCollider m_collider;
    [SerializeField] private float m_asleepDuration = 10f;
    [SerializeField] private float m_awakenDuration = 5f;
    [SerializeField] private bool m_isSleepwalker = false;
    private bool m_gotcha = false;
    private bool m_isPeeking = false;
    private PlayerController m_playerController;

    public bool m_debugWakeup = false;
    private void Awake()
    {
        m_witnessBody = transform.GetChild(0);
        m_light = transform.GetChild(1);
        m_balloon = transform.GetChild(2);
        m_collider = GetComponent<BoxCollider>();
        m_balloon.gameObject.SetActive(false);
        m_playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();


    }
    // Start is called before the first frame update
    void Start()
    {
        SetPeeking(false);
        if(m_isSleepwalker)
        {
            StartCoroutine(SleepWalk());
        }
    }

    private void Update()
    {
        if (m_debugWakeup)
        {
            m_debugWakeup = false;
            StartCoroutine(WakeUpOnce());
        }
    }
    void SetPeeking(bool peeking)
    {
        m_witnessBody.gameObject.SetActive(peeking);
        m_light.gameObject.SetActive(peeking);
        //m_collider.enabled = peeking;
        m_isPeeking = peeking;
    }

    private IEnumerator SleepWalk()
    {
        while(true)
        {
            SetPeeking(false);
            m_balloon.gameObject.SetActive(false);
            yield return new WaitForSeconds(m_asleepDuration);
            SetPeeking(true);
            yield return new WaitForSeconds(m_awakenDuration);
        }
    }

    public IEnumerator WakeUpOnce()
    {
        SetPeeking(true);
        yield return new WaitForSeconds(m_awakenDuration);
        SetPeeking(false);
        m_balloon.gameObject.SetActive(false);
    }

    private void OnTriggerStay(Collider other)
    {
        if (m_isPeeking && other.CompareTag("Player"))
        {
            m_balloon.gameObject.SetActive(true);
            if (!m_gotcha)
            {
                print("GOTCHA!");
                m_gotcha = true;
                // increase witnesses count
                m_playerController.TakeDamage();
            }
        }
        else if(!m_isPeeking && other.CompareTag("Cat"))
        {
            StartCoroutine(WakeUpOnce());
        }

    }

    /*private void OnTriggerStay(Collider other)
    {
        if (!m_isPeeking && other.CompareTag("Cat"))
        {
            StartCoroutine(WakeUpOnce());
        }
    }*/
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedPlatform : Resettable
{
    public enum TimingStyle { STRICT_COUNTDOWN, CONTACT_COUNTDOWN };

    [SerializeField] private TimingStyle m_style = TimingStyle.CONTACT_COUNTDOWN;
    [SerializeField] private float m_maxTime = 5;
    [SerializeField] private float m_timeLeft = 5;
    
    private BoxCollider m_boxCollider;
    public float TimeLeft { get { return m_timeLeft; } set { m_timeLeft = value;  } }
    public TimingStyle Style { get { return m_style; } set { m_style = value; } }
    private bool m_aboutToCollapse = false;
    private bool m_contact = false;
    private bool m_alive = true;
    private TestCrumbling[] m_platformShatters;
    
    protected override void Awake()
    {
        base.Awake();
        m_platformShatters = GetComponentsInChildren<TestCrumbling>();
        m_boxCollider = GetComponent<BoxCollider>();
        m_timeLeft = m_maxTime;
    }

    void Update()
    {
        if (Style == TimingStyle.CONTACT_COUNTDOWN && m_contact && TimeLeft > 0)
        {
            TimeLeft -= Time.deltaTime;
            if(TimeLeft <= 0 && m_alive)
            {
                StartCoroutine(Collapse());      
            }
        }
    }

    void OnTriggerEnter(Collider collider)
    {        
        if(collider.CompareTag("Player"))
        {
            m_contact = true;

            if (Style == TimingStyle.STRICT_COUNTDOWN && !m_aboutToCollapse)
            {
                m_aboutToCollapse = true;
                StartCoroutine(CountdownToDestruction());
            }
        }
        
    }

    void OnTriggerExit(Collider collider)
    {
        if (collider.CompareTag("Player"))
        {
            m_contact = false;
        }            
    }

    private IEnumerator CountdownToDestruction()
    {
        yield return new WaitForSeconds(TimeLeft);

        yield return StartCoroutine(Collapse());
    }

    private IEnumerator Collapse()
    {
        m_alive = false;
        m_boxCollider.enabled = false;
        foreach (var shatter in m_platformShatters)
        {
            shatter.Collapse();
        }

        yield return new WaitForSeconds(5f);

        gameObject.SetActive(false);
    }

    public override void Reset()
    {
        foreach (var shatter in m_platformShatters)
        {
            shatter.Reset();
        }

        base.Reset();

        m_timeLeft = m_maxTime;
        m_contact = false;
        m_aboutToCollapse = false;
        m_alive = true;
    }
}

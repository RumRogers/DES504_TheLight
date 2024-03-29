﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedPlatform : MonoBehaviour
{
    public enum TimingStyle { STRICT_COUNTDOWN, CONTACT_COUNTDOWN };

    [SerializeField] private TimingStyle m_style = TimingStyle.CONTACT_COUNTDOWN;
    [SerializeField] private float m_timeLeft = 5;
    private BoxCollider m_boxCollider;
    public float TimeLeft { get { return m_timeLeft; } set { m_timeLeft = value;  } }
    public TimingStyle Style { get { return m_style; } set { m_style = value; } }
    private bool m_aboutToCollapse = false;
    private bool m_contact = false;
    private bool m_alive = true;
    private TestCrumbling[] m_platformShatters;
    
    private void Awake()
    {
        m_platformShatters = GetComponentsInChildren<TestCrumbling>();
        m_boxCollider = GetComponent<BoxCollider>();
    }

    void Update()
    {
        if (Style == TimingStyle.CONTACT_COUNTDOWN && m_contact && TimeLeft > 0)
        {
            TimeLeft -= Time.deltaTime;
            if(TimeLeft <= 0 && m_alive)
            {
                Collapse();      
            }
        }
    }

    void OnTriggerEnter(Collider collider)
    {
        m_contact = true;

        if (Style == TimingStyle.STRICT_COUNTDOWN && !m_aboutToCollapse)
        {
            m_aboutToCollapse = true;
            StartCoroutine(CountdownToDestruction());
        }      
    }

    void OnTriggerExit(Collider collider)
    {
        m_contact = false;    
    }

    private IEnumerator CountdownToDestruction()
    {
        while (TimeLeft > 0)
        {
            //print("Collapsing in " + TimeLeft + " seconds...");
            yield return new WaitForSeconds(1);
            TimeLeft -= 1;
        }

        //print("Platform collapsed. Coroutine end.");
        Collapse();

        yield return null;
    }

    private void Collapse()
    {
        m_alive = false;
        m_boxCollider.enabled = false;
        foreach (var shatter in m_platformShatters)
        {
            shatter.Collapse();
        }

        Destroy(gameObject, 3);
    }
}

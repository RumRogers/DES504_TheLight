using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedPlatform : MonoBehaviour
{
    public enum TimingStyle { STRICT_COUNTDOWN, CONTACT_COUNTDOWN };

    [SerializeField] private TimingStyle m_style = TimingStyle.CONTACT_COUNTDOWN;
    [SerializeField] private float m_timeLeft = 5;
    public float TimeLeft { get { return m_timeLeft; } set { m_timeLeft = value;  } }
    public TimingStyle Style { get { return m_style; } set { m_style = value; } }
    private bool m_aboutToCollapse = false;
    private bool m_contact = false;

    void Update()
    {
        if (Style == TimingStyle.CONTACT_COUNTDOWN && m_contact && TimeLeft > 0)
        {
            TimeLeft -= Time.deltaTime;
            if(TimeLeft <= 0)
            {
                Destroy(gameObject);
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
            print("Collapsing in " + TimeLeft + " seconds...");
            yield return new WaitForSeconds(1);
            TimeLeft -= 1;
        }

        print("Platform collapsed. Coroutine end.");
        Destroy(gameObject);

        yield return null;
    }
}

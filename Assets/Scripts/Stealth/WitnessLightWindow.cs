using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WitnessLightWindow : MonoBehaviour
{
    private Transform m_witnessBody;
    private Transform m_light;
    private BoxCollider m_collider;
    [SerializeField] private float m_asleepDuration = 60f;
    [SerializeField] private float m_awokenDuration = 5f;
    private bool m_gotcha = false;

    private void Awake()
    {
        m_witnessBody = transform.GetChild(0);
        m_light = transform.GetChild(1);
        m_collider = GetComponent<BoxCollider>();
    }
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Loop());
    }

    void SetPeeking(bool peeking)
    {
        m_witnessBody.gameObject.SetActive(peeking);
        m_light.gameObject.SetActive(peeking);
        m_collider.enabled = peeking;
    }

    public IEnumerator Loop()
    {
        while(true)
        {
            SetPeeking(false);
            yield return new WaitForSeconds(m_asleepDuration);
            SetPeeking(true);
            yield return new WaitForSeconds(m_awokenDuration);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if(!m_gotcha)
            {
                print("GOTCHA!");
                m_gotcha = true;
                // increase witnesses count
            }
            else
            {
                print("Spotted again...");
            }

        }
    }
}

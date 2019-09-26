using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LimitedJumpsPlatform : MonoBehaviour
{
    [SerializeField] private float m_jumpsLeft = 2;
    public float JumpsLeft
    {
        get { return m_jumpsLeft; }
        set { m_jumpsLeft = value; }
    }
    void OnTriggerEnter(Collider collider)
    {
        if (m_jumpsLeft == 0)
        {
            Destroy(gameObject);
        }
        else
        {
            print("Creeeeeak... " + m_jumpsLeft + " jumps remaining...");
            m_jumpsLeft--;
        }

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCBasicBehavior : CCTVCamera
{
    [SerializeField] private float m_speed = 1f;
    [SerializeField] private float m_breakDurationInSeconds = 3f;
    private Vector3 m_pathStart;
    [SerializeField] private Transform m_pathEnd;
    private Vector3 m_currentStart;
    private Vector3 m_currentGoal;

    protected override void Awake()
    {
        base.Awake();
        //m_fovDegrees = 160;
        m_alarm = false;
        m_pathStart = transform.position;
        m_currentStart = m_pathStart;
        m_currentGoal = m_pathEnd.position;
    }

    void Start()
    {
        StartCoroutine(Patrol());
    }

    IEnumerator Patrol()
    {
        float tLerp = 0;
        while (true)
        {      
            
            if (transform.position != m_currentGoal)
            {
                tLerp += m_speed * Time.deltaTime;
                transform.position = Vector3.Lerp(m_currentStart, m_currentGoal, tLerp);
                yield return new WaitForEndOfFrame();
            }
            else
            {
                ChangeGoal();
                tLerp = 0;
                yield return new WaitForSeconds(m_breakDurationInSeconds);
                transform.Rotate(0, 180, 0);
            }
        }
    }

    void ChangeGoal()
    {
        if(m_currentGoal == m_pathStart) // if the cop just came back to his original position...
        {
            m_currentGoal = m_pathEnd.position; // set the current goal as path end 
            m_currentStart = m_pathStart; // set the current start as path start
        }
        else // if the cop just reached path end, invert
        {
            m_currentGoal = m_pathStart; 
            m_currentStart = m_pathEnd.position;
        }
    }
}

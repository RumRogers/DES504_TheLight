﻿using System.Collections;
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
    [SerializeField] GameObject m_platform;
    private Vector3 m_leftBound;
    private Vector3 m_rightBound;
    private IEnumerator m_patrolCoroutine;
    [SerializeField] private float m_chasingSpeed = 10;
    private Animator m_copAnimator;
    [SerializeField] private bool m_chasing = false;
    private bool m_gotcha = false;
    private bool m_suspicious = false;
    private IEnumerator m_resetAfterChase = null;

    protected override void Awake()
    {
        base.Awake();
        //m_fovDegrees = 160;
        m_alarm = false;
        m_pathStart = transform.position;
        m_currentStart = m_pathStart;
        m_currentGoal = m_pathEnd.position;
        ComputePlatformBounds();
        m_alarmBalloon.transform.parent = null;
        Billboard billboard = m_alarmBalloon.GetComponent<Billboard>();
        billboard.SetTarget(transform);
        m_copAnimator = transform.GetChild(0).GetComponent<Animator>();
        //m_alarmBalloon.SetActive(true);
    }

    void Start()
    {
        m_patrolCoroutine = Patrol();
        StartCoroutine(m_patrolCoroutine);
    }

    protected override void Update()
    {
        DrawCone();

        if(m_playerController.IsHiding)
        {
            m_chasing = false;
        }
        if(!m_gotcha)
        {
            if (m_chasing)
            {
                Chase();
            }
            else if (m_patrolCoroutine == null)
            {
                ChangeGoal();
                m_currentStart = transform.position;
                m_patrolCoroutine = Patrol();
                StartCoroutine(m_patrolCoroutine);
            }
        }
    }

    IEnumerator Patrol()
    {
        float tLerp = 0;
        m_copAnimator.SetBool("isIdle", false);
        m_copAnimator.SetBool("isChasing", false);
        m_copAnimator.SetBool("isWalking", true);
        m_alarmBalloon.SetActive(false);

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
                m_copAnimator.SetBool("isIdle", true);
                m_copAnimator.SetBool("isChasing", false);
                m_copAnimator.SetBool("isWalking", false);
                yield return new WaitForSeconds(m_breakDurationInSeconds);

                m_copAnimator.SetBool("isIdle", false);
                m_copAnimator.SetBool("isChasing", false);
                m_copAnimator.SetBool("isWalking", true);
                //transform.Rotate(0, 180, 0);
            }

            //m_alarmBalloon.transform.parent = transform.parent;
            if(transform.position.x > m_currentGoal.x)
            {
                transform.rotation = Quaternion.Euler(0, 90, 0);
            }
            else if (transform.position.x < m_currentGoal.x)
            {
                transform.rotation = Quaternion.Euler(0, 270, 0);
            }
            //m_alarmBalloon.transform.parent = transform;
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

    void ComputePlatformBounds()
    {
        m_leftBound = new Vector3();
        m_rightBound = new Vector3();

        if (m_platform != null)
        {
            BoxCollider collider = m_platform.GetComponent<BoxCollider>();

            m_leftBound = collider.bounds.min;
            m_rightBound = collider.bounds.max;
        }

        //print("Left bound: " + m_leftBound);
        //print("Right bound: " + m_rightBound);
    }

    void Chase()
    {
        m_alarmBalloon.SetActive(true);

        if (m_suspicious)
        {
            m_copAnimator.SetBool("isIdle", true);
            m_copAnimator.SetBool("isChasing", false);
            m_copAnimator.SetBool("isWalking", false);
        }
        
        else
        {
            m_copAnimator.SetBool("isIdle", false);
            m_copAnimator.SetBool("isChasing", true);
            m_copAnimator.SetBool("isWalking", false);

            if (m_patrolCoroutine != null)
            {
                StopCoroutine(m_patrolCoroutine);
                m_patrolCoroutine = null;
            }

            int dir = 1;

            if (transform.position.x > m_target.position.x)
            {
                dir = -1;
            }

            Vector3 nextPos = transform.position + new Vector3(m_chasingSpeed * dir * Time.deltaTime, 0, 0);
            if (dir == 1 && nextPos.x > m_rightBound.x)
            {
                nextPos.x = m_rightBound.x;
                m_suspicious = true;
            }
            else if (dir == -1 && nextPos.x < m_leftBound.x)
            {
                nextPos.x = m_leftBound.x;
                m_suspicious = true;
            }

            transform.position = nextPos;
        }
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            if(m_resetAfterChase != null)
            {
                StopCoroutine(m_resetAfterChase);
                m_resetAfterChase = null;
            }
            
            // lose   
            m_chasing = true;
            if (!m_playerController.InvulnerableToCops)
            {                
                //m_playerController.Bust();                
            }            
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            if(!m_playerController.InvulnerableToCops)
            {
                if (Vector3.Distance(transform.position, other.transform.position) < 2f)
                {
                    m_gotcha = true;
                    //m_speed = 0;
                    //m_chasingSpeed = 0;
                    m_chasing = false;
                    m_alarmBalloon.SetActive(false);
                    m_copAnimator.SetBool("isIdle", true);
                    m_copAnimator.SetBool("isChasing", false);
                    m_copAnimator.SetBool("isWalking", false);                    
                    m_playerController.Bust();
                }
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (m_suspicious && m_resetAfterChase == null)
            {
                m_resetAfterChase = Utils.WaitAndExecute(2.5f, () =>
                {
                    m_chasing = false;
                    m_suspicious = false;
                    m_resetAfterChase = null;
                });
                StartCoroutine(m_resetAfterChase);
            }    
        }
    }
}

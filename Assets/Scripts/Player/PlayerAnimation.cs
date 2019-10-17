using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerAnimation : MonoBehaviour
{
    private Animator m_animator;
    private RuntimeAnimatorController m_playerAnimatorController;

    private void Awake()
    {
        m_animator = GetComponent<Animator>();
        if(m_animator != null)
        {
            m_playerAnimatorController = m_animator.runtimeAnimatorController;
        }
    }

    public void SetBool(string name, bool value)
    {
        if(m_animator.runtimeAnimatorController == null)
        {
            return;
        }

        bool b = m_animator.GetBool(name);
        if(b != value)
        {
            m_animator.SetBool(name, value);
        }
    }

    public void SetTrigger(string name)
    {
        m_animator.SetTrigger(name);
    }

    public void UseTimeline(bool b)
    {
        if(b)
        {
            m_animator.runtimeAnimatorController = null;
            m_animator.applyRootMotion = false;
        }
        else
        {            
            m_animator.runtimeAnimatorController = m_playerAnimatorController;
            m_animator.applyRootMotion = true;
        }
    }

    public Animator GetAnimator()
    {
        return m_animator;
    }
}

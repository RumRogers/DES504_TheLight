using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private Animator m_animator;

    private void Awake()
    {
        m_animator = GetComponent<Animator>();
    }

    public void SetBool(string name, bool value)
    {
        bool b = m_animator.GetBool(name);
        if(b != value)
        {
            m_animator.SetBool(name, value);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ChangeGlowing : MonoBehaviour
{
    [SerializeField] Material m_material;
    [SerializeField] float m_maxGlow = 1f;
    [SerializeField] float m_minGlow = .2f;
    [SerializeField] float m_speedMultiplier = 1;
    private float m_currGlow;
    private int sign = 1;


    // Update is called once per frame
    void Update()
    {
        m_currGlow += Time.deltaTime * sign * m_speedMultiplier;
        if(m_currGlow >= m_maxGlow)
        {
            sign = -1;
            m_currGlow = m_maxGlow;
        }
        else if(m_currGlow <= m_minGlow)
        {
            sign = 1;
            m_currGlow = m_minGlow;
        }


        m_material.SetFloat("_GlowPower", m_currGlow); 
    }
}

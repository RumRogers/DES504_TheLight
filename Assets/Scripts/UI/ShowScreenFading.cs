using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShowScreenFading : MonoBehaviour
{

    [SerializeField] int m_targetAlpha = 255; 
    private Image[] m_backgrounds;
    private TextMeshProUGUI m_text;
    private Color32[] m_originalBGColor;
    private Color32 m_originalTextColor;    
    private Color32 m_goalTextColor;
    private bool m_doFadeIn = false;
    [SerializeField] float m_fadingSpeed;
    private float m_currAlpha;
    private DateTime m_lastTime;
    [SerializeField] int m_howManyImages = 1;


    private void Awake()
    {
        m_backgrounds = new Image[m_howManyImages];
        m_originalBGColor = new Color32[m_howManyImages];
        m_doFadeIn = false;
        for(int i = 0; i < m_howManyImages; i++)
        {
            m_backgrounds[i] = transform.GetChild(i).GetComponent<Image>();
            m_originalBGColor[i] = m_backgrounds[i].color;
        }
        
        m_text = transform.GetChild(m_howManyImages).GetComponent<TextMeshProUGUI>();        
        m_originalTextColor = m_text.color;
        m_goalTextColor = new Color32(m_originalTextColor.r, m_originalTextColor.g, m_originalTextColor.b, 255);
    }

    // Update is called once per frame
    void Update()
    {
        if(m_doFadeIn)
        {            
            m_currAlpha = Mathf.Min(m_currAlpha + m_fadingSpeed * Time.unscaledDeltaTime, 1);            
            m_text.color = Color32.Lerp(m_originalTextColor, m_goalTextColor, m_currAlpha);

            for(int i = 0; i < m_howManyImages; i++)
            {
                Color c = m_originalBGColor[i];
                c.a = 255;
                m_backgrounds[i].color = Color32.Lerp(m_originalBGColor[i], c, m_currAlpha);
            }            
        }
    }

    public void Reset()
    {
        m_doFadeIn = false;
        for(int i = 0; i < m_howManyImages; i++)
        {
            m_backgrounds[i].color = m_originalBGColor[i];
        }        
        m_text.color = m_originalTextColor;
    }

    public void DoFadeIn(string message = "")
    {
        if(message.CompareTo("") != 0)
        {
            m_text.text = message;
        }
        
        m_doFadeIn = true;
        m_lastTime = DateTime.Now;
    }
}

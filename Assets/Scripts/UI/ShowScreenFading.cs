using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShowScreenFading : MonoBehaviour
{
    private Image m_background;
    private TextMeshProUGUI m_text;
    private Color32 m_originalBGColor;
    private Color32 m_originalTextColor;
    private Color32 m_goalBGColor;
    private Color32 m_goalTextColor;
    private bool m_doFadeIn = false;
    [SerializeField] float m_fadingSpeed;
    private float m_currAlpha;
    private DateTime m_lastTime;

    private void Awake()
    {
        m_doFadeIn = false;
        m_background = transform.GetChild(0).GetComponent<Image>();
        m_text = transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        m_originalBGColor = m_background.color;
        m_originalTextColor = m_text.color;
        print(m_originalBGColor);
        m_goalBGColor = new Color32(m_originalBGColor.r, m_originalBGColor.g, m_originalBGColor.b, 255);
        m_goalTextColor = new Color32(m_originalTextColor.r, m_originalTextColor.g, m_originalTextColor.b, 255);
    }

    // Update is called once per frame
    void Update()
    {
        if(m_doFadeIn)
        {            
            m_currAlpha = Mathf.Min(m_currAlpha + m_fadingSpeed * Time.unscaledDeltaTime, 1);
            m_background.color = Color32.Lerp(m_originalBGColor, m_goalBGColor, m_currAlpha);
            m_text.color = Color32.Lerp(m_originalTextColor, m_goalTextColor, m_currAlpha);
        }
    }

    public void Reset()
    {
        m_doFadeIn = false;
        m_background.color = m_originalBGColor;
        m_text.color = m_originalTextColor;
    }

    public void DoFadeIn(string message)
    {
        if(message.CompareTo("") != 0)
        {
            m_text.text = message;
        }
        
        m_doFadeIn = true;
        m_lastTime = DateTime.Now;
    }
}

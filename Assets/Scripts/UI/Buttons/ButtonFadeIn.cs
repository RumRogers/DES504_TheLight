using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ButtonFadeIn : MonoBehaviour
{
    private Button m_button;
    private Image m_image;
    private TextMeshProUGUI m_text;
    private Color m_color;
    [SerializeField] private float m_fadeInSpeed = 1f;
    private void Awake()
    {
        m_button = GetComponent<Button>();
        m_image = GetComponent<Image>();
        m_image.enabled = false;
        m_button.enabled = false;        
        m_text = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        m_color = m_text.color;
        m_color.a = 0;
        m_text.color = m_color;
    }

    // Update is called once per frame
    void Update()
    {
        if(m_color.a < 1)
        {
            m_color.a = Mathf.Min(m_color.a + Time.fixedDeltaTime * m_fadeInSpeed, 1);
            m_text.color = m_color;            
        }  
        else
        {
            m_image.enabled = true;
            m_button.enabled = true;            
        }
    }
}

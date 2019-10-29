using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Test : MonoBehaviour
{
    private Image m_image;

    private void Awake()
    {
        m_image = GetComponent<Image>();
        m_image.color = Color.red;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

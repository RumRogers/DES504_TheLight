using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonScript : MonoBehaviour
{
    protected Button m_button;

    private void Awake()
    {
        m_button = GetComponent<Button>();
    }    
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LowerHUDMessage : MonoBehaviour
{
    private TextMeshProUGUI m_text;

    private void Awake()
    {
        m_text = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        m_text.text = "";
    }
    
    public void SetText(string text, float seconds = 0)
    {
        m_text.text = text;

        if (seconds > 0)
        {
            StartCoroutine(ResetTextAfterSeconds(seconds));
        }
    }

    private IEnumerator ResetTextAfterSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        m_text.text = "";
    }
}

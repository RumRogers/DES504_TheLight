using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RetryButton : ButtonScript
{
    private void Start()
    {        
        m_button.onClick.AddListener(GameManager.Instance.Retry);
    }

}

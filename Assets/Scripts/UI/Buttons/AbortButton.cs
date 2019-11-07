using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AbortButton : ButtonScript
{   
    void Start()
    {
        try
        {
            m_button.onClick.AddListener(GameManager.Instance.AbortGame);
        }
        catch(ArgumentException ex)
        {

            StartCoroutine(Utils.WaitUntil(() => { return GameManager.Instance != null;  }));
        }
    }
}

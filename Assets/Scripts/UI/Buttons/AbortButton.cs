using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AbortButton : ButtonScript
{   
    void Start()
    {
        StartCoroutine(Utils.WaitUntilAndExecute(() => { return GameManager.Instance != null; }, () => { m_button.onClick.AddListener(GameManager.Instance.AbortGame); }));
    }
}

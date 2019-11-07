using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextLevelButton : ButtonScript
{
    // Start is called before the first frame update
    void Start()
    {
        m_button.onClick.AddListener(GameManager.Instance.NextLevel);
    }
}

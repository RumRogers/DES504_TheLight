using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCBasicBehavior : CCTVCamera
{
    private void Awake()
    {
        m_fovDegrees = 160;
        m_alarm = false;
    }

    // Update is called once per fram
}

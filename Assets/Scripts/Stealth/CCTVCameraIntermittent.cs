using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CCTVCameraIntermittent : CCTVCamera
{
    [SerializeField] private float m_idleTimeInSeconds = 2f;
    [SerializeField] private float m_recordingTimeInSeconds = 5f;

    private void Start()
    {
        StartCoroutine(WorkIntermittently());
    }

    private IEnumerator WorkIntermittently()
    {
        while(true)
        {
            m_isRecording = true;
            yield return new WaitForSeconds(m_recordingTimeInSeconds);
            m_isRecording = false;
            yield return new WaitForSeconds(m_idleTimeInSeconds);
        }
        
    }
}

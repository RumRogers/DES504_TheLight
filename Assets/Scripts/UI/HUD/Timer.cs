using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class Timer : MonoBehaviour
{
    [SerializeField] private int m_timeLeft;
    private TextMeshProUGUI m_textCaption;
    private TextMeshProUGUI m_textTimeLeft;
    private Material m_textMaterial;
    private IEnumerator m_timerCoroutine;
    public bool IsRunning { get; set; }


    private void Awake()
    {
        m_textCaption = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        m_textTimeLeft = transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        m_timerCoroutine = null;
        m_textCaption.color = Utils.SetAlpha(m_textCaption.color, 0);
        m_textTimeLeft.color = Utils.SetAlpha(m_textTimeLeft.color, 0);
        //m_textMaterial = transform.GetChild(1).GetComponent<Material>();
}

    private void Update()
    {
        if(IsRunning && m_timerCoroutine == null)
        {
            m_timerCoroutine = RunTimer();
            StartCoroutine(m_timerCoroutine);
            m_textCaption.color = Utils.SetAlpha(m_textCaption.color, 255);
            m_textTimeLeft.color = Utils.SetAlpha(m_textTimeLeft.color, 255);
        }
        else if(!IsRunning && m_timerCoroutine != null)
        {
            StopCoroutine(m_timerCoroutine);
        }
    }
    private IEnumerator RunTimer()
    {
        while(m_timeLeft > 0)
        {
            TimeSpan timeSpan = TimeSpan.FromSeconds(m_timeLeft);
            m_textTimeLeft.text = timeSpan.Minutes + ":" + timeSpan.Seconds;
            if(m_timeLeft == 10)
            {
                // change text material to red
            }
            yield return new WaitForSeconds(1);
            m_timeLeft--;
        }

        GameManager.Instance.ShowScreen(GameManager.UIScreen.MissionFailed, GameManager.Instance.m_loseByTimeUpMsg);
    }
    

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Timer : MonoBehaviour
{
    public Text timerText;
    [SerializeField] private float startTime;
    public Transform m_canvas_death;
    private float t_health;
    // Start is called before the first frame update
    void Start()
    {
        startTime = Time.time;
        t_health = GetComponent<Health>().health;
    }

    // Update is called once per frame
    void Update()
    {
        float t = 60 - (Time.time - startTime);
        if (t < 20)
        {
            timerText.color = Color.red;
            if (t <= 0)
            {
                m_canvas_death.gameObject.SetActive(true);
                Time.timeScale = 0;
            }
        }
        else
            timerText.color = Color.white;
        string minutes = ((int)t / 60).ToString();
        string seconds = (t % 60).ToString("f1");
        timerText.text = minutes + ":" + seconds;

        if (t_health > GetComponent<Health>().health)
        {
            startTime = Time.time;
            t_health = GetComponent<Health>().health;
        }
    }
}

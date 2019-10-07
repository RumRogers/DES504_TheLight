using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Timer : MonoBehaviour
{
    public Text timerText;
    private float startTime;

    // Start is called before the first frame update
    void Start()
    {
        startTime = Time.time;    
    }

    // Update is called once per frame
    void Update()
    {
        float t = 20 - (Time.time - startTime);
        if ( t < 10)
        {
            timerText.color = Color.red;
            if (t <= 0)
                return;
        }
       
        string minutes = ((int)t / 60).ToString();
        string seconds = (t % 60).ToString("f1");
        timerText.text = minutes + ":" + seconds;
    }
}

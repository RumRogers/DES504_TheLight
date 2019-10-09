using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DeathMenu : MonoBehaviour
{
    
    public Transform m_canvas_death;
    // Start is called before the first frame update
    void Start()
    {

        Time.timeScale = 1f;
        m_canvas_death.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void TryAgainButtonClick()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}

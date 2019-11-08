using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class EndingMenu : MonoBehaviour
{
    public void toMainMenu()
    {
        SceneManager.LoadScene(0);
    }
  public void QuitGame()
    {
        Application.Quit();
    }

    public void StartDemo()
    {
        SceneManager.LoadScene(2);
    }
}

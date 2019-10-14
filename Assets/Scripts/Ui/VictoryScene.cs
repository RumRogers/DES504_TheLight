using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class VictoryScene : MonoBehaviour
{
   public void Play_again()
    {
        SceneManager.LoadScene(3);
    }
   public void LoadTo_Main_menu()
    {
        SceneManager.LoadScene(2);
    }
}

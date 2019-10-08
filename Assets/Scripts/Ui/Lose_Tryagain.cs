using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Lose_Tryagain : MonoBehaviour
{
   public void TryAgainButtonClick()
    {
        SceneManager.LoadScene(1);
    }
}

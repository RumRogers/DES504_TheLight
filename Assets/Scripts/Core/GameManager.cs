using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public delegate void Callback();

    [Header("Game UI")]
    [SerializeField] private ShowScreenFading m_missionFailed;
    [SerializeField] private ShowScreenFading m_missionComplete;
    [SerializeField] private GameObject m_missionFailedButtonSet;
    [SerializeField] private GameObject m_missionCompleteButtonSet;
    [SerializeField] private List<Transform> m_witnessImages;


    public bool GamePaused { get; private set; }

    public enum UIScreen
    {
        MissionFailed, MissionComplete
    }

    public static GameManager Instance = null;

    // Start is called before the first frame update
    void Start()
    {
        if(Instance == null)
        {
            Instance = this;            
            GamePaused = false;            
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ShowScreen(UIScreen screen, string message = "")
    {
        SetPause(true);
        switch (screen)
        {
            case UIScreen.MissionFailed:
                m_missionComplete.gameObject.SetActive(false);
                m_missionFailed.DoFadeIn(message);
                StartCoroutine(WaitAndCall(1, () => { m_missionFailedButtonSet.SetActive(true); }));
                break;
            case UIScreen.MissionComplete:
                m_missionFailed.gameObject.SetActive(false);
                m_missionComplete.DoFadeIn(message);
                StartCoroutine(WaitAndCall(1, () => { m_missionCompleteButtonSet.SetActive(true); }));
                break;
        }
    }

    public void AbortGame()
    {
        SceneManager.LoadScene(0);
        SetPause(false);
    }

    public void Retry()
    {        
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);        
        SetPause(false);
    }

    void SetPause(bool pause)
    {
        if(pause)
        {
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
        }

        GamePaused = pause;
    }

    private IEnumerator WaitAndCall(float seconds, Callback callback)
    {
        yield return new WaitForSecondsRealtime(seconds);
        callback();
    }

    public void UpdateWitnessesUI(int playerHealth)
    {
        m_witnessImages[m_witnessImages.Count - playerHealth - 1].gameObject.SetActive(true);
    }
}

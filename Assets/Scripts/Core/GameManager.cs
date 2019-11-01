using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public delegate void Callback();

    [Header("Game UI")]
    [SerializeField] private ShowScreenFading m_missionFailed;
    [SerializeField] private ShowScreenFading m_missionComplete;
    [SerializeField] private GameObject m_missionFailedButtonSet;
    [SerializeField] private GameObject m_missionCompleteButtonSet;
    [SerializeField] private List<Transform> m_witnessImages;
    private GameObject m_pauseScreen;
    private LowerHUDMessage m_lowerHUDMessage;
    private Timer m_timerScript;

    public bool GamePaused { get; private set; }

    public enum UIScreen
    {
        MissionFailed, MissionComplete, Pause
    }

    public static GameManager Instance = null;

    // Start is called before the first frame update
    void Start()
    {
        if(Instance == null)
        {
            Instance = this;            
            GamePaused = false;
            m_timerScript = GameObject.FindGameObjectWithTag("Timer").GetComponent<Timer>();
            //m_pauseScreen = GameObject.FindGameObjectsWithTag("UIScreen")[2];
            m_pauseScreen = GameObject.Find("Pause");
            m_pauseScreen.SetActive(false);
            m_lowerHUDMessage = GameObject.Find("LowerHUD").GetComponent<LowerHUDMessage>();
            /*SceneManager.sceneLoaded += (Scene s, LoadSceneMode lsm) => 
            {
                Debug.Log("crap: " + s.name);
                Debug.Log(lsm);
            };*/
            
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

    public void SetPause(bool pause, bool showPauseScreen = false)
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
        m_pauseScreen.SetActive(pause & showPauseScreen);
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

    public void StartTimer()
    {
        m_timerScript.IsRunning = true;
    }

    public void SetLowerHUDText(string message, float seconds = 0)
    {
        m_lowerHUDMessage.SetText(message, seconds);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using TMPro;

public class GameManager : MonoBehaviour
{
    public delegate void Callback();
    public delegate bool Predicate();

    [Header("Game UI")]
    private ShowScreenFading m_missionFailed;
    private ShowScreenFading m_missionComplete;
    private GameObject m_missionFailedButtonSet;
    private GameObject m_missionCompleteButtonSet;
    //[SerializeField] private List<GameObject> m_witnessImages;
    private List<Image> m_witnessImages;
    private GameObject m_pauseScreen;
    private LowerHUDMessage m_lowerHUDMessage;
    private Timer m_timerScript;
    private Image m_nothingInventoryImage;
    private Image m_crowbarInventoryImage;
    private Image m_monkeyWrenchInventoryImage;
    private RectTransform m_panelInventory;
    private TextMeshProUGUI m_textInventory;
    private float m_globalTimeLeftInSeconds;
    private bool initialized = false;
    private Sprite m_inactiveWitness;
    private Sprite m_activeWitness;

    private PlayerController m_playerController;
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
            SceneManager.sceneLoaded += (Scene s, LoadSceneMode lsm) =>
            {
                Initialize();                
            };
            Initialize();  
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Initialize()
    {
        initialized = false;
        GamePaused = false;        

        int currSceneIdx = SceneManager.GetActiveScene().buildIndex;
        if (currSceneIdx != 0 && currSceneIdx != 1 && currSceneIdx != 4)
        {
            m_globalTimeLeftInSeconds = 300;
            GameObject missionFailedScreen = GameObject.Find("MissionFailed");
            GameObject missionCompleteScreen = GameObject.Find("MissionComplete");
            m_missionFailed = missionFailedScreen.GetComponent<ShowScreenFading>();
            m_missionComplete = missionCompleteScreen.GetComponent<ShowScreenFading>();
            m_missionFailedButtonSet = missionFailedScreen.transform.GetChild(2).gameObject;
            m_missionCompleteButtonSet = missionCompleteScreen.transform.GetChild(2).gameObject;

            Transform witnessesGameObject = GameObject.Find("WitnessesUI").transform;
            m_witnessImages = new List<Image>();

            m_activeWitness = Resources.Load<Sprite>(ResourceBindings.ActiveWitness);
            m_inactiveWitness = Resources.Load<Sprite>(ResourceBindings.InactiveWitness);

            foreach (Transform childTransform in witnessesGameObject)
            {
                Image im = childTransform.GetComponent<Image>();
                im.overrideSprite = m_inactiveWitness;
                m_witnessImages.Add(im);
            }

            m_timerScript = GameObject.FindGameObjectWithTag("Timer").GetComponent<Timer>();
            m_playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
            m_pauseScreen = GameObject.Find("Pause");
            m_pauseScreen.transform.localPosition = new Vector3(0, -10000, m_pauseScreen.transform.position.z);
            //m_pauseScreen.SetActive(false);
            m_lowerHUDMessage = GameObject.Find("LowerHUD").GetComponent<LowerHUDMessage>();
            m_crowbarInventoryImage = GameObject.Find("InventoryCrowbar").GetComponent<Image>();
            m_nothingInventoryImage = GameObject.Find("InventoryNothing").GetComponent<Image>();
            m_monkeyWrenchInventoryImage = GameObject.Find("InventoryMonkeyWrench").GetComponent<Image>();
            m_panelInventory = GameObject.Find("InventoryPanel").GetComponent<RectTransform>();
            m_textInventory = GameObject.Find("InventoryText").GetComponent<TextMeshProUGUI>();
            m_crowbarInventoryImage.color = new Color(255, 255, 255, 0);
            m_monkeyWrenchInventoryImage.color = new Color(255, 255, 255, 0);

            Inventory.Instance.Empty();            
            initialized = true;
        }
         
    }

    private void Update()
    {
        m_globalTimeLeftInSeconds -= Time.deltaTime;
        if(m_globalTimeLeftInSeconds <= 0)
        {
            SceneManager.LoadScene(4);
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

    public void NextLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
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
        m_pauseScreen.transform.localPosition = new Vector3(0, -10000 * (pause ? 0 : 1), m_pauseScreen.transform.position.z);
    }

    private IEnumerator WaitAndCall(float seconds, Callback callback)
    {
        yield return new WaitForSecondsRealtime(seconds);
        callback();
    }

    public void UpdateWitnessesUI(int playerHealth)
    {
        //m_witnessImages[m_witnessImages.Count - playerHealth - 1].gameObject.SetActive(true);
        m_witnessImages[m_witnessImages.Count - playerHealth - 1].overrideSprite = m_activeWitness;
    }

    public void StartTimer()
    {
        m_timerScript.IsRunning = true;
    }

    public void SetLowerHUDText(string message, float seconds = 0)
    {
        m_lowerHUDMessage.SetText(message, seconds);
    }

    public void UIAddToInventory(Inventory.InventoryItems item)
    {
        Color c = new Color(255, 255, 255, 255);
        switch(item)
        {
            case Inventory.InventoryItems.MonkeyWrench:
                m_monkeyWrenchInventoryImage.color = c;
                break;
            case Inventory.InventoryItems.Crowbar:
                m_crowbarInventoryImage.color = c;
                break;
            default:
                break;
        }
    }

    public void UISetActiveInventoryItem(Inventory.InventoryItems item)
    {
        Vector3 panelPos = m_panelInventory.localPosition;
        switch (item)
        {
            case Inventory.InventoryItems.MonkeyWrench:
                m_panelInventory.localPosition = new Vector3(-100, panelPos.y, panelPos.z);
                break;
            case Inventory.InventoryItems.None:
                m_panelInventory.localPosition = new Vector3(0, panelPos.y, panelPos.z);
                break;
            case Inventory.InventoryItems.Crowbar:
                m_panelInventory.localPosition = new Vector3(100, panelPos.y, panelPos.z);
                break;
            default:
                break;
        }

        m_textInventory.text = "Selected item: " + Inventory.Instance.GetItemName(item);
    }    

    public void UpdatePlayerRespawnPoint(Vector3 respawnPoint)
    {
        m_playerController.RespawnPoint = respawnPoint;
    }

    public IEnumerator StartGlobalTimer()
    {
        while(m_globalTimeLeftInSeconds > 0)
        {
            yield return new WaitForSeconds(1);
            m_globalTimeLeftInSeconds--;
        }
    }
}

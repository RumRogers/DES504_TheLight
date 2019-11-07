﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using TMPro;

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
    private Image m_nothingInventoryImage;
    private Image m_crowbarInventoryImage;
    private Image m_monkeyWrenchInventoryImage;
    private RectTransform m_panelInventory;
    private TextMeshProUGUI m_textInventory;

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
        GamePaused = false;
        m_timerScript = GameObject.FindGameObjectWithTag("Timer").GetComponent<Timer>();
        m_playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        m_pauseScreen = GameObject.Find("Pause");
        m_pauseScreen.SetActive(false);
        m_lowerHUDMessage = GameObject.Find("LowerHUD").GetComponent<LowerHUDMessage>();
        m_crowbarInventoryImage = GameObject.Find("InventoryCrowbar").GetComponent<Image>();
        m_nothingInventoryImage = GameObject.Find("InventoryNothing").GetComponent<Image>();
        m_monkeyWrenchInventoryImage = GameObject.Find("InventoryMonkeyWrench").GetComponent<Image>();
        m_panelInventory = GameObject.Find("InventoryPanel").GetComponent<RectTransform>();
        m_textInventory = GameObject.Find("InventoryText").GetComponent<TextMeshProUGUI>();

        if(SceneManager.GetActiveScene().buildIndex != 0) // not the main menu
        {
            m_crowbarInventoryImage.color = new Color(255, 255, 255, 0);
            m_monkeyWrenchInventoryImage.color = new Color(255, 255, 255, 0);
        }

        Inventory.Instance.Empty();      
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
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Loading_drainpipes : MonoBehaviour
{
    public Transform LoadingBar;
    public Transform TextIndicator;
    public Image image_LoadingBar;
    public Text text_TextIndicator;
    public Image image_RadialProgressBar;
    public Image image_Center;
    [SerializeField] private float currentAmount;
    [SerializeField] private float speed;
    // Start is called before the first frame update
    void Start()
    {
        image_RadialProgressBar.enabled = false;
        text_TextIndicator.enabled = false;
        image_LoadingBar.enabled = false;
        image_Center.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentAmount < 100)
        {
            currentAmount += speed * Time.deltaTime;
            TextIndicator.GetComponent<Text>().text = ((int)currentAmount).ToString() + "%";
        }
        else
            TextIndicator.GetComponent<Text>().text = "Done!";
        LoadingBar.GetComponent<Image>().fillAmount = currentAmount / 100;
    }
}

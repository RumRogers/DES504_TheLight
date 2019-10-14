using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Dialogue : MonoBehaviour
{
    public Image cloud_Dialogue_drainpipes;
    public Image cloud_Dialogue_items;
  
    public Text[] text_dialogue_drainpipes;
    public Text[] text_dialogue_items;

    private void Awake()
    {
    }
    // Start is called before the first frame update
    void Start()                                                                //hide all texts and images
    {
        cloud_Dialogue_drainpipes.enabled = false;
        cloud_Dialogue_items.enabled = false;
        for (int i = 0; i < text_dialogue_drainpipes.Length; i++)
        {
            text_dialogue_drainpipes[i].enabled = false;
        }
        for(int i = 0; i < text_dialogue_items.Length; i++)
        {
            text_dialogue_items[i].enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
       //  cloud_Dialogue_drainpipes.transform.rotation = Quaternion.identity;
       //  cloud_Dialogue_items.transform.rotation = Quaternion.identity;
         for (int i = 0; i < text_dialogue_drainpipes.Length; i++)
         {
             text_dialogue_drainpipes[i].transform.rotation = Quaternion.identity; // Quaternion.eulerAngles(0f; 180f, 0);
         }
         for (int i = 0; i < text_dialogue_items.Length; i++)
         {
             text_dialogue_items[i].transform.rotation = Quaternion.identity;
         }
      
        /*if (cloud_Dialogue_items.enabled == true)
        {
            StartCoroutine(waitSeconds_items());
            //cloud_Dialogue_items.enabled = false;
        }
        if (cloud_Dialogue_drainpipes.enabled == true)
        {
            StartCoroutine(waitSeconds_drainpipes());
            //cloud_Dialogue_drainpipes.enabled = false;
        }*/
    }

    IEnumerator waitSeconds_items()                             //wait for 1 seconds, and dialogue will be disappeared
    {
        yield return new WaitForSeconds(0);
        cloud_Dialogue_items.enabled = false;
        for (int i = 0; i < text_dialogue_items.Length; i++)
        {
            if (text_dialogue_items[i].enabled == true)
                text_dialogue_items[i].enabled = false;
        }
    } 
    IEnumerator waitSeconds_drainpipes()
    {
        yield return new WaitForSeconds(0);
        cloud_Dialogue_drainpipes.enabled = false;
        for (int i = 0; i < text_dialogue_drainpipes.Length; i++)
        {
            if (text_dialogue_drainpipes[i].enabled == true)
                text_dialogue_drainpipes[i].enabled = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Resettable" && (other.transform.parent.name.CompareTo("Pipes") == 0) && cloud_Dialogue_drainpipes.enabled == false)
        {
           
            cloud_Dialogue_drainpipes.enabled = true;
            int n = UnityEngine.Random.Range(0, 2);
            Debug.Log(n + "drainpipes");
            text_dialogue_drainpipes[n].enabled = true;
        }
        if (other.tag == "Resettable" && (other.transform.parent.name.CompareTo("Collectibles") == 0) && cloud_Dialogue_items.enabled == false)
        {
  
            cloud_Dialogue_items.enabled = true;
            int n = UnityEngine.Random.Range(0, 2);
            Debug.Log(n+"items");
            text_dialogue_items[n].enabled = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Resettable" && (other.transform.parent.name.CompareTo("Pipes") == 0) )
            StartCoroutine(waitSeconds_drainpipes());
        if (other.tag == "Resettable" && (other.transform.parent.name.CompareTo("Collectibles") == 0) )
            StartCoroutine(waitSeconds_items());
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Interaction : MonoBehaviour
{
    public Image[] interact;
    public Sprite interact_item;
    public bool identify_items;

    private void Start()
    {
        interact[0].enabled = false;
        identify_items = false;
    }
    private void Update()
    {
     
        if (interact[0].enabled == true && Input.GetKey(KeyCode.E))
        {
            interact[0].enabled = false;
            identify_items = false;
        }
        if (identify_items == true)
        {
            interact[0].enabled = true;
        }
    }
    private void OnTriggerEnter(Collider other) // loading
    {
        try
        {
            if (other.tag == "Resettable" && (other.transform.parent.name.CompareTo("Collectibles") == 0) || other.transform.parent.name.CompareTo("Pipes") == 0)
            {
                identify_items = true;
            }
        }
        catch(NullReferenceException ex)
        {
            print("Caught exception: " + ex.Message);
        }
        
    
        
    }
    private void OnTriggerExit(Collider other)
    {
        identify_items = false;
        interact[0].enabled = false;
    }
}

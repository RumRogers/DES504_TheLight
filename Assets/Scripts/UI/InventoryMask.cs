using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InventoryMask : MonoBehaviour
{
    private void Awake()
    {
        if(SceneManager.GetActiveScene().buildIndex != 3)
        {
            Destroy(gameObject);
        }
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}

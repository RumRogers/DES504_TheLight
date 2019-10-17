using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightedWindow : Resettable
{
    [SerializeField] private PlayerController m_playerController;
    private int i = 0;
    private bool judge = false;
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            judge = true;
            StartCoroutine(WaitPlayerSeconds());
        
        }
    }

    private void OnTriggerExit(Collider other)
    {
        judge = false;
    }

    IEnumerator WaitPlayerSeconds()                           //give player 0.5f to avoid lighted window
    {
            yield return new WaitForSeconds(0.1f);
            StartCoroutine(WaitForMove());
    }

    IEnumerator WaitForMove()                               //each 0.1f time to move 0.1f distance
    {
        while (i != 5)
        {
            yield return new WaitForSeconds(0.1f);
            i++;
            var pos = transform.position;
            pos.z -= 0.05f;
            transform.position = pos;
            if (judge == true)
                m_playerController.Die();
        }
        var posori = transform.position;
        posori.z = 0.8f;
        transform.position = posori;

    }
}

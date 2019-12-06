using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Readable : MonoBehaviour
{
    private SpriteRenderer m_spriteRenderer;
    private BoxCollider m_collider;
    private DisplayTips m_tips;
    private Transform m_associatedHotspot;
    private bool m_readable = true;

    private void Awake()
    {
        m_tips = Camera.main.transform.GetChild(0).GetComponent<DisplayTips>();
        m_spriteRenderer = GetComponent<SpriteRenderer>();
        m_collider = transform.gameObject.AddComponent<BoxCollider>();
        m_collider.center = m_spriteRenderer.sprite.bounds.center;
        m_collider.size = m_spriteRenderer.sprite.bounds.size;
        m_collider.size += new Vector3(0, 0, 3.5f);
        m_collider.isTrigger = true;
        if(transform.childCount > 0)
        {
            m_associatedHotspot = transform.GetChild(0);
        }
    }   

    private void OnTriggerStay(Collider other)
    {
       if(m_readable && other.CompareTag("Player"))
        {
            if(Input.GetButtonDown("Action"))
            {
                m_readable = false;
                StartCoroutine(Utils.WaitAndExecute(1f, () => { m_readable = true; }));
                m_tips.Display(m_spriteRenderer.sprite);
                m_collider.enabled = false;
                if (m_associatedHotspot != null)
                {
                    m_associatedHotspot.gameObject.SetActive(false);
                    GameManager.Instance.SetLowerHUDText("");
                }
                m_tips.Callback = () =>
                {
                    StartCoroutine(Utils.WaitAndExecute(.5f, () => { m_collider.enabled = true; }));      
                };
            }
        }
    }
}

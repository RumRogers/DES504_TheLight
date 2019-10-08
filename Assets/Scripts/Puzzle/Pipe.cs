using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pipe : MonoBehaviour
{
    public enum PipeDirection
    {
        Left,
        Right
    }

    private static float rotation_speed = 100;

    [SerializeField] private PipeDirection m_direction;
    [SerializeField] private Transform m_pipeJumpHotspot;
    [SerializeField] private Transform m_pipeEnd;
    [SerializeField] private bool m_isStuck = false;
    private PlayerController m_playerController;
    private bool m_rotating = false;

    private void Awake()
    {

        GameObject gameObject = GameObject.Find("Player");
        if(gameObject != null)
        {
            m_playerController = gameObject.GetComponent<PlayerController>();
        }      
    }

    private void Update()
    {
        if(m_direction == PipeDirection.Left && transform.rotation.eulerAngles.y != 0)
        {
            Rotate(-1);
        }
        else if(m_direction == PipeDirection.Right && transform.rotation.eulerAngles.y != 180)
        {
            Rotate(1);
        }
    }

    private void Rotate(int dir)
    {
        transform.Rotate(0, rotation_speed * dir * Time.deltaTime, 0, Space.Self);

        if(transform.rotation.eulerAngles.y > 270)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
            m_rotating = false;
        }
        else if(transform.rotation.eulerAngles.y > 180)
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
            m_rotating = false;
        }

    }

    private void OnTriggerStay(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            if(Input.GetButtonDown("Action") && !m_rotating)
            {
                switch(m_playerController.CurrentItem)
                {
                    case Inventory.InventoryItems.None:
                        StartCoroutine(m_playerController.GrabPipe(m_pipeJumpHotspot.position, m_pipeEnd.position, m_direction));
                        break;
                    case Inventory.InventoryItems.MonkeyWrench:
                        m_rotating = true;
                        m_direction = m_direction == PipeDirection.Left ? PipeDirection.Right : PipeDirection.Left;
                        break;
                }
                
            }
        }
    }
}

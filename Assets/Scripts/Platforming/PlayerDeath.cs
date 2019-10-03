using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeath : MonoBehaviour
{
    [SerializeField] private CameraFollowTarget m_cameraFollowTarget;
    [SerializeField] private Transform m_respawnPoint;
    [SerializeField] private PlayerController m_playerController;

    private void OnTriggerEnter(Collider other)
    {
        if(other.name.Equals("Player"))
        {
            StartCoroutine(DieWithDignity(other.transform));
        }
    }

    private IEnumerator DieWithDignity(Transform player)
    {
        //m_cameraFollowTarget.Smooth = false;
        yield return new WaitForSeconds(1);
        //m_cameraFollowTarget.Smooth = true;
        m_playerController.Respawn(m_respawnPoint.position);
    }
}

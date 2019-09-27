using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeath : MonoBehaviour
{
    [SerializeField] private CameraFollowTarget m_cameraFollowTarget;
    [SerializeField] private Transform m_respawnPoint;

    private void OnTriggerEnter(Collider other)
    {
        if(other.name.Equals("Player"))
        {
            StartCoroutine(DieWithDignity(other.transform));
        }
    }

    private IEnumerator DieWithDignity(Transform player)
    {
        m_cameraFollowTarget.Smooth = false;
        yield return new WaitForSeconds(1);
        m_cameraFollowTarget.Smooth = true;
        player.gameObject.SetActive(false);
        player.transform.position = m_respawnPoint.position;
        player.gameObject.SetActive(true);
    }
}

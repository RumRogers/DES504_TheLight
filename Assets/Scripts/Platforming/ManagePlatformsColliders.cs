using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagePlatformsColliders : MonoBehaviour
{
    private Collider[] m_colliders;   
    public static ManagePlatformsColliders Instance = null;

    private void Awake()
    {
        if(ManagePlatformsColliders.Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        ManagePlatformsColliders.Instance = this;
        m_colliders = GetComponentsInChildren<Collider>();
    }

    public void DetectCollisions(bool state)
    {
        foreach(Collider c in m_colliders)
        {
            c.enabled = state;
        }
    }
}

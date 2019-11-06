using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    protected Transform m_target = null;
    Vector3 m_offset;

    public void SetTarget(Transform target)
    {
        m_target = target;
        m_offset = transform.position - target.position;
    }
    // Update is called once per frame
    void Update()
    {
        //transform.forward = Vector3.down;
        if (m_target != null)
        {
            FollowTarget();
        }
    }

    private void FollowTarget()
    {
        transform.position = m_target.position + m_offset;
    }
}

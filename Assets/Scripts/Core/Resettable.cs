using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resettable : MonoBehaviour
{
    private Vector3 m_originalPosition;
    private Quaternion m_originalRotation;

    protected virtual void Awake()
    {
        m_originalPosition = transform.position;
        m_originalRotation = transform.rotation;
    }

    public virtual void Reset()
    {
        gameObject.SetActive(false);
        transform.position = m_originalPosition;
        transform.rotation = m_originalRotation;
        gameObject.SetActive(true);
    }
}

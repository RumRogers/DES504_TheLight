using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunningStars : MonoBehaviour
{
    [SerializeField] private float m_rotationSpeed = 180f;
    
    void Update()
    {
        transform.Rotate(0, m_rotationSpeed * Time.deltaTime, 0);
    }
}

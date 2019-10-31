using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowTarget : MonoBehaviour
{

    private Transform m_target;
    private Vector3 m_offset;
    [SerializeField] private bool m_smooth;
    [SerializeField] private float m_lerpSpeed = 1;
   
    public Transform Target { get { return m_target; } set { m_target = value; } }
    public bool Smooth { get { return m_smooth; } set { m_smooth = value; } }

    // Start is called before the first frame update
    void Awake()
    {
        m_target = GameObject.FindGameObjectWithTag("Player").transform;
        m_offset = transform.position - m_target.position;
    }

    // Update is called once per frame
    void Update()
    {
        if(!m_smooth)
        {
            transform.position = m_target.position + m_offset;
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, transform.position = m_target.position + m_offset, Time.deltaTime * m_lerpSpeed);
        }
    }
}

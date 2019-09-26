using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CCTVCamera : MonoBehaviour
{
    [SerializeField] private bool m_debugMode = false;
    private Transform m_lens;
    [SerializeField] private Transform m_target;
    [SerializeField] private float m_distance = 5f;
    [SerializeField] protected float m_fovDegrees = 90;
    [SerializeField] protected bool m_alarm = false;
   
    public Transform Target { get { return m_target; } set { m_target = value;  } }
    private Vector3 m_viewconeBaseCenter;


    private MeshRenderer[] m_meshRenderers = new MeshRenderer[2];
    private void Awake()
    {
        m_lens = transform.Find("Lens");

        //Component[] components = GetComponentsInChildren(typeof(MeshRenderer), false);
        //m_meshRenderers[0] = (MeshRenderer)components[0];
        //m_meshRenderers[1] = (MeshRenderer)components[1];
       
    }

    void Start()
    {
         
    }

    // Update is called once per frame
    protected void Update()
    {
        if(m_debugMode)
        {
            DrawDebuggingGraphics();
        }
        m_alarm = IsTargetVisible();
    }

    private bool IsTargetVisible()
    {
        Vector3 offset = m_target.position - m_lens.position;
        float angle = Vector3.Angle(m_lens.forward, offset);
        if (angle <= m_fovDegrees / 2 && Vector3.Magnitude(offset) <= m_distance) 
        {            
            return true;
        }
        return false;
    }

    

    private void DrawDebuggingGraphics()
    {
        Debug.DrawLine(m_lens.position, m_lens.position + transform.forward * m_distance, Color.red);
        Debug.DrawLine(m_lens.position, m_target.position, Color.green);


        /*if (m_alarm)
        {
            for(int i = 0; i < m_meshRenderers.Length; i++)
            {
                m_meshRenderers[i].material.color = Color.red;
            }
        }
        else
        {
            for (int i = 0; i < m_meshRenderers.Length; i++)
            {
                m_meshRenderers[i].material.color = Color.white;
            }
        }*/
    }
}

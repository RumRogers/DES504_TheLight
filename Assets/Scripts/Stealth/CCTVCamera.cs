using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CCTVCamera : MonoBehaviour
{
    [SerializeField] private bool m_debugMode = true;
    private Transform m_lens;
    [SerializeField] protected GameObject m_alarmBalloon;    
    [SerializeField] private float m_distance = 10f;
    [SerializeField] protected float m_fovDegrees = 20;
    [SerializeField] protected bool m_alarm = false;
    protected Transform m_target;
    private Vector3 axis;
    private Color m_nativeColor;
    private LineRenderer m_lineRenderer;
    private bool m_witnessed = false;
    protected PlayerController m_playerController;
    [SerializeField] protected bool m_isRecording = true;

    public Transform Target { get { return m_target; } set { m_target = value;  } }
    private Vector3 m_viewconeBaseCenter;


    private List<MeshRenderer> m_meshRenderers = new List<MeshRenderer>();

    protected virtual void Awake()
    {
        //m_target = GameObject.FindGameObjectWithTag("Player").transform;
        m_target = GameObject.Find("CameraTarget").transform;
        m_playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();

        m_lens = Utils.FindChildByNameRecursive(transform, "Lens");

        Component[] components = GetComponentsInChildren(typeof(MeshRenderer), false);
        for(int i = 0; i < components.Length; i++)
        {
            m_meshRenderers.Add((MeshRenderer)components[i]);
        }

        if(components.Length > 0)
        {
            m_nativeColor = m_meshRenderers[0].material.color;
        }

        m_lineRenderer = GetComponent<LineRenderer>();
        if(m_lineRenderer != null)
        {
            m_lineRenderer.positionCount = 720;
            //m_lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        }
        DrawCone();
    }

    // Update is called once per frame
    protected virtual void Update()
    {
               //m_lens = transform.Find("Lens");
        if (m_debugMode)
        {
            DrawDebuggingGraphics();
        }

        if(m_lineRenderer != null && m_isRecording)
        {
            m_lineRenderer.enabled = true;
            DrawCone();
        }
        else
        {
            m_lineRenderer.enabled = false;
        }

        m_alarm = IsTargetVisible();
        m_alarmBalloon.SetActive(m_alarm);
        if (m_alarm)
        {
            RaiseAlarm();
        }       
    }

    private bool IsTargetVisible()
    {
        if(!m_isRecording)
        {
            return false;
        }

        Vector3 offset = m_target.position - m_lens.position;
        float angle = Vector3.Angle(m_lens.forward, offset);
        if (angle <= m_fovDegrees / 2 && Vector3.Magnitude(offset) <= m_distance) 
        {            
            return true;
        }
        return false;
    }

    

    protected void DrawDebuggingGraphics()
    {
        //Vector3 a = Quaternion.Euler(0, 45, 0) * m_lens.forward;
        //Vector3 a = Quaternion.Euler(axis.x, axis.y, axis.z) * (m_lens.position + m_lens.forward);
        //Vector3 a = Quaternion.identity * (m_lens.position + m_lens.forward);
        Vector3 a = Quaternion.Euler(0, m_fovDegrees / 2, 0) * m_lens.forward;
        Vector3 b = Quaternion.Euler(0, -m_fovDegrees / 2, 0) * m_lens.forward;
        Vector3 c = Quaternion.Euler(0, 0, m_fovDegrees / 2) * m_lens.forward;
        Vector3 d = Quaternion.Euler(0, 0, -m_fovDegrees / 2) * m_lens.forward;
        

        /*Debug.DrawLine(m_lens.position, m_lens.position + a * m_distance, Color.red);
        Debug.DrawLine(m_lens.position, m_lens.position + b * m_distance, Color.red);
        Debug.DrawLine(m_lens.position, m_lens.position + m_lens.forward * m_distance, Color.blue);
        Debug.DrawLine(m_lens.position, m_lens.position + c * m_distance, Color.red);
        Debug.DrawLine(m_lens.position, m_lens.position + d * m_distance, Color.red);*/


        Vector3 axis1 = Vector3.Lerp(a, b, .5f);
        Vector3 axis2 = Vector3.Lerp(c, d, .5f);
        axis = Vector3.Lerp(axis1, axis2, 0.5f);

        Vector3 v;

        for (int i = 0; i < 360; i++)
        {

            //f = Quaternion.Euler(0, m_fovDegrees / 2 - i, m_fovDegrees / 2 - i) * b;
            //f = Quaternion.Euler(0, -m_fovDegrees / 2 + i, -m_fovDegrees / 2 + i) * m_lens.forward;
            v = Quaternion.AngleAxis(i, axis) * b;
            Debug.DrawLine(m_lens.position, m_lens.position + v * m_distance, Color.red);
        }

        //Debug.DrawLine(m_lens.position, m_lens.position + f * m_distance, Color.red);
        if (m_alarm)
        {
            Debug.DrawLine(m_lens.position, m_target.position, Color.red);
        }
        //


        if (m_alarm)
        {
            for(int i = 0; i < m_meshRenderers.Count; i++)
            {
                m_meshRenderers[i].material.color = Color.red;
            }
        }
        else
        {
            for (int i = 0; i < m_meshRenderers.Count; i++)
            {
                m_meshRenderers[i].material.color = m_nativeColor;
            }
        }
    }

    protected void DrawCone()
    {        
        Vector3 a = Quaternion.Euler(0, m_fovDegrees / 2, 0) * m_lens.forward;
        Vector3 b = Quaternion.Euler(0, -m_fovDegrees / 2, 0) * m_lens.forward;
        Vector3 c = Quaternion.Euler(0, 0, m_fovDegrees / 2) * m_lens.forward;
        Vector3 d = Quaternion.Euler(0, 0, -m_fovDegrees / 2) * m_lens.forward;

        Vector3 axis1 = Vector3.Lerp(a, b, .5f);
        Vector3 axis2 = Vector3.Lerp(c, d, .5f);
        axis = Vector3.Lerp(axis1, axis2, 0.5f);

        Vector3 v;

        for (int i = 0; i < 720; i += 2)
        {
            m_lineRenderer.SetPosition(i, m_lens.position);

            v = Quaternion.AngleAxis(i, axis) * b;
            m_lineRenderer.SetPosition(i + 1, m_lens.position + v * m_distance);
        }
    }

    private void RaiseAlarm()
    {
        if(!m_witnessed)
        {
            m_witnessed = true;
            m_playerController.TakeDamage();
            // increase witness count
        }
        if(m_alarmBalloon != null)
        {
            m_alarmBalloon.SetActive(true);
        }
    }
}

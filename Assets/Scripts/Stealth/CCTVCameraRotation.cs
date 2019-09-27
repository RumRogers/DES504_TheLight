using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CCTVCameraRotation : MonoBehaviour
{
    [SerializeField] private float m_rotationSpeed = 10f;
    [SerializeField] private bool m_shouldRotate = true;
    [SerializeField] private float m_rotationTimeInterval = 10f;
    [SerializeField] [Range(0, 360)] private float m_initialYRotationDegs;
    [SerializeField] [Range(0, 360)] private float m_finalYRotationDegs;
    [SerializeField] private bool m_rotateClockwise = false;
    private int m_rotationDir;
    private float m_targetDegrees;

    // Start is called before the first frame update
    void Awake()
    {
        Vector3 currentRotation = transform.rotation.eulerAngles;

        transform.rotation = Quaternion.Euler(currentRotation.x, m_initialYRotationDegs, currentRotation.x);
        m_targetDegrees = Mathf.Abs(m_initialYRotationDegs - m_finalYRotationDegs);

        if (!m_rotateClockwise)
        {
            m_rotationDir = -1;
        }
        else
        {
            m_rotationDir = 1;
        }
    }

    void Start()
    {
        if (m_shouldRotate)
        {
            StartCoroutine(RotateEternally());
        }
    }
    // Update is called once per frame
    IEnumerator RotateCamera()
    {
        float rotationSoFar = 0f; // Counter for degrees

        while(true)
        {
            float nextRotationAmount = m_rotationSpeed * Time.deltaTime;

            if(rotationSoFar + nextRotationAmount > m_targetDegrees)
            {                
                nextRotationAmount = m_targetDegrees - rotationSoFar; // Clamp amount if next rotation exceeds target rotation
            }

            rotationSoFar += nextRotationAmount;
            transform.Rotate(0, nextRotationAmount * m_rotationDir, 0, Space.World);

            if(rotationSoFar == m_targetDegrees)
            {
                m_rotationDir *= -1;
                yield break;
            }
            
            
            yield return new WaitForEndOfFrame();
        }       
    }

    IEnumerator RotateEternally()
    {
        while (true)
        {
            yield return new WaitForSeconds(m_rotationTimeInterval);
            yield return StartCoroutine(RotateCamera());
        }
    }
}

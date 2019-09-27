using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CCTVCameraRotation : MonoBehaviour
{
    [SerializeField] private float m_rotationSpeed = 10f;
    [SerializeField] private bool m_shouldRotate = true;
    [SerializeField] private float m_rotationTimeInterval = 10f;
    private int m_rotationDir;

    // Start is called before the first frame update
    void Awake()
    {
        if (transform.rotation.eulerAngles.y == 270f)
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
        while (true)
        {
            print("Rot: " + transform.rotation.eulerAngles);
            if (m_rotationDir == -1 && transform.rotation.eulerAngles.y <= 90)
            {
                m_rotationDir = 1;
            }
            else if (m_rotationDir == 1 && transform.rotation.eulerAngles.y >= 270)
            {
                m_rotationDir = -1;
            }

            transform.Rotate(0, m_rotationSpeed * m_rotationDir * Time.deltaTime, 0, Space.World);

            if (transform.rotation.eulerAngles.y < 90)
            {
                transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, 90, transform.rotation.eulerAngles.z);
                break;
            }
            else if (transform.rotation.eulerAngles.y > 270)
            {
                transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, 270, transform.rotation.eulerAngles.z);
                break;
            }

            yield return new WaitForEndOfFrame();
        }

        yield return null;
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

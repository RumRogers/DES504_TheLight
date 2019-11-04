using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatBehavior : MonoBehaviour
{
    // Where the cat can walk to
    private Vector3 m_leftBound;
    private Vector3 m_rightBound;
    private Vector3 m_currentDestination;

    private void Awake()
    {
        m_leftBound = transform.position;
        m_rightBound = transform.position;

        Transform closestPlatform = GetClosestPlatform();
        if(closestPlatform != null)
        {
            BoxCollider collider = closestPlatform.GetComponent<BoxCollider>();
            if(collider != null)
            {
                m_leftBound = collider.bounds.min;
                m_rightBound = collider.bounds.max;
            }
        }

    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private Transform GetClosestPlatform()
    {        
        GameObject[] platforms = GameObject.FindGameObjectsWithTag("Platform");

        if(platforms.Length == 0)
        {
            return null;
        }

        Transform closest = platforms[0].transform;
        float minDist = Vector3.Distance(transform.position, closest.position);

        for(int i = 1; i < platforms.Length; i++ )
        {
            float dist = Vector3.Distance(transform.position, platforms[i].transform.position);
            if(dist < minDist)
            {
                minDist = dist;
                closest = platforms[i].transform;
            }
        }

        return closest;
    }


}

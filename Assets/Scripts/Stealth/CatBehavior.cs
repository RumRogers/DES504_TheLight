using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class CatBehavior : MonoBehaviour
{
    [Header("Generic Behavior")]
    [SerializeField] private float m_speed;
    [SerializeField] private float m_meowingSeconds;
    [SerializeField] private float m_alarmDistanceThreshold;

    // Where the cat can walk to
    private Vector3 m_leftBound;
    private Vector3 m_rightBound;

    private Transform m_platform;
    private Vector3 m_currentDestination;

    private Collider m_catCollider;

    private bool m_isMeowing = false;

    private PlayerController m_playerController;

    private void Awake()
    {
        m_leftBound = transform.position;
        m_rightBound = transform.position;
        m_currentDestination = transform.position;    

        Transform closestPlatform = GetClosestPlatform();
        if(closestPlatform != null)
        {
            m_platform = closestPlatform;
            BoxCollider collider = closestPlatform.GetComponent<BoxCollider>();
            if(collider != null)
            {
                m_leftBound = collider.bounds.min;
                m_rightBound = collider.bounds.max;              
            }
        }

        m_playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        m_catCollider = GetComponent<Collider>();
        m_catCollider.enabled = false;
    }

    void Start()
    {       
        m_currentDestination = Vector3.Lerp(m_leftBound, m_rightBound, Random.Range(.1f, 1f));
        m_currentDestination.z = transform.position.z;
        StartCoroutine(RandomWalk());
    }

    private void Update()
    {        
        if(!m_isMeowing && Vector3.Distance(transform.position, m_playerController.transform.position) < m_alarmDistanceThreshold)
        {
            if(!m_playerController.Crouching && IsFacingPlayer())
            {
                StopAllCoroutines();
                StartCoroutine(Meow());
            }
        }
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

    void SetRandomDestinationPoint()
    {
        m_currentDestination = Vector3.Lerp(m_leftBound, m_rightBound, Random.Range(.1f, 1f));
        m_currentDestination.y = transform.position.y;
        m_currentDestination.z = transform.position.z;
    }

    IEnumerator RandomWalk()
    {
        while(true)
        {
            SetRandomDestinationPoint();
            yield return StartCoroutine(ReachDestination());
            yield return new WaitForSeconds(Random.Range(1f, 5f));
        }        
    }

    private IEnumerator ReachDestination()
    {
        if (m_currentDestination.x < transform.position.x)
        {
            transform.rotation = Quaternion.Euler(0, 270f, 0);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 90f, 0);
        }
        while (Vector3.Distance(transform.position, m_currentDestination) > .001f)
        {
            transform.position = Vector3.MoveTowards(transform.position, m_currentDestination, m_speed * Time.deltaTime);
            yield return new WaitForSeconds(Time.deltaTime);
        }
    }

    private IEnumerator Meow()
    {
        print("Meow!");
        m_isMeowing = true;
        //StopCoroutine(RandomWalk());
        //StopCoroutine(ReachDestination());
        
        m_catCollider.enabled = true;
        // make some noise!!!
        yield return new WaitForSeconds(m_meowingSeconds);
        m_isMeowing = false;
        m_catCollider.enabled = false;
        StartCoroutine(RandomWalk());
    }

    private bool IsFacingPlayer()
    {
        Vector3 dir = m_playerController.transform.position - transform.position;
           
        return Mathf.Sign(dir.x) == Mathf.Sign(transform.forward.x);
    }
}

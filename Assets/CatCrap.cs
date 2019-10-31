using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatCrap : MonoBehaviour
{
    [SerializeField] private float m_speed = 4;
    // Start is called before the first frame update
    

    // Update is called once per frame
    void Update()
    {
        transform.position += Vector3.right * Time.deltaTime * m_speed;    
    }
}

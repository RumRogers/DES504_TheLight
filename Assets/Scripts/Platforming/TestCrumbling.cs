using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCrumbling : MonoBehaviour
{
    private Rigidbody rb;
    private void Awake()
    {

        rb = GetComponent<Rigidbody>();
    }

    public void Collapse()
    {
        rb.useGravity = true;
        rb.angularVelocity = new Vector3(Random.Range(-1f, 1f), Random.Range(0f, -1f), Random.Range(-1f, 1f));
        rb.velocity = -Vector3.down * Physics.gravity.y * 2f;
    }
}

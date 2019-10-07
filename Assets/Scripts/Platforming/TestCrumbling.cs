using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCrumbling : Resettable
{
    private Rigidbody rb;
    protected override void Awake()
    {
        base.Awake();
        rb = GetComponent<Rigidbody>();
    }

    public void Collapse()
    {
        rb.useGravity = true;
        rb.angularVelocity = new Vector3(Random.Range(-1f, 1f), Random.Range(0f, -1f), Random.Range(-1f, 1f));
        rb.velocity = -Vector3.down * Physics.gravity.y * 2f;
    }

    public override void Reset()
    {
        base.Reset();
        rb.useGravity = false;
        rb.angularVelocity = Vector3.zero;
        rb.velocity = Vector3.zero;
    }
}

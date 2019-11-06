using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatParticleFX : Billboard
{
    private void Awake()
    {
        SetTarget(transform.parent);
        transform.parent = null;
        transform.localScale = new Vector3(2.5f, 2.5f, 0);
    }
}

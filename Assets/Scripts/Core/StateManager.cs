using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateManager : MonoBehaviour
{
    [SerializeField] private List<Resettable> m_resettables = new List<Resettable>();
    
    public void ResetAll()
    {
        foreach(Resettable resettable in m_resettables)
        {
            resettable.Reset();
        }
    }
}

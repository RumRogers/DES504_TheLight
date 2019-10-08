using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateManager : MonoBehaviour
{
    [SerializeField] private List<Resettable> m_resettables = new List<Resettable>();

    private void Awake()
    {
        GameObject[] resettableObjects = GameObject.FindGameObjectsWithTag("Resettable");
        for(int i = 0; i < resettableObjects.Length; i++)
        {
            m_resettables.Add(resettableObjects[i].GetComponent<Resettable>());
        }
    }
    public void ResetAll()
    {
        foreach(Resettable resettable in m_resettables)
        {
            resettable.Reset();
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomizeCredits : MonoBehaviour
{
    [SerializeField] private string[] m_members = new string[4];
    [SerializeField] private TMPro.TextMeshProUGUI[] m_labels = new TMPro.TextMeshProUGUI[4];
    private Dictionary<TMPro.TextMeshProUGUI, bool> m_assignedCouples;

    public void RandomizeCouples()
    {
        m_assignedCouples = new Dictionary<TMPro.TextMeshProUGUI, bool>();

        for (int i = 0; i < 4; i++)
        {
            string member = m_members[i];
            int labelIdx;

            do
            {
                labelIdx = Random.Range(0, 4);
            }
            while (m_assignedCouples.ContainsKey(m_labels[labelIdx]));

            m_assignedCouples[m_labels[labelIdx]] = true;
            m_labels[labelIdx].text = member;
        }
    }
}

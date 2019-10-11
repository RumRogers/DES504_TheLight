using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    /*private static SoundManager m_instance;
    public static SoundManager Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = new SoundManager();
            }
            return m_instance;
        }
    }*/

    private AudioSource m_audioSource;

    [SerializeField] private AudioClip m_walkFX;
    [SerializeField] private AudioClip m_jumpFX;
    [SerializeField] private AudioClip m_runFX;
    [SerializeField] private AudioClip m_turnPipeFX;


    public void PlaySound(int id)
    {
        AudioClip clip = null;

        switch(id)
        {
            case 0:
                clip = m_walkFX;
                break;
            case 1:
                clip = m_jumpFX;
                break;
            case 2:
                clip = m_runFX;
                break;
            case 3:
                clip = m_turnPipeFX;
                break;
        }

        m_audioSource.clip = clip;
        m_audioSource.Play();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

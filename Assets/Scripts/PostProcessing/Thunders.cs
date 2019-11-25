using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class Thunders : MonoBehaviour
{
    private PostProcessVolume m_postProcessVolume;
    private Bloom m_bloomLayer;
    private AudioSource m_audioSource;
    private bool m_throwing = false;
    [SerializeField] private float m_lightSpeedMultiplier = 1.5f;
    [SerializeField] private bool m_throwLightning = false;
    [SerializeField] private float m_minThreshold = .3f; 
    [SerializeField] private float m_goalThreshold = 1;
    
    private void Awake()
    {
        m_postProcessVolume = GetComponent<PostProcessVolume>();
        m_postProcessVolume.profile.TryGetSettings(out m_bloomLayer);
        m_audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!m_throwing && Random.Range(0f, 1f) >= .95f)        
        {
            m_throwLightning = true;
        }

        if(m_throwLightning)
        {
            m_throwLightning = false;
            StartCoroutine(DoThrow());
        }
    }

    private IEnumerator ThrowLightning()
    {
        m_bloomLayer.threshold.value = m_minThreshold;
        if (!m_audioSource.isPlaying)
        {
            m_audioSource.PlayOneShot(m_audioSource.clip);
        }


        while (m_bloomLayer.threshold.value < m_goalThreshold)
        {
            yield return new WaitForSeconds(0.01f);
            m_bloomLayer.threshold.value += Time.deltaTime * m_lightSpeedMultiplier;
        }

        m_bloomLayer.threshold.value = m_goalThreshold;    
    }

    private IEnumerator DoThrow()
    {
        m_throwing = true;
        StartCoroutine(ThrowLightning());
        yield return new WaitForSeconds(.1f);
        StartCoroutine(ThrowLightning());
        yield return new WaitForSeconds(Random.Range(10, 20));
        m_throwing = false;
    }
}

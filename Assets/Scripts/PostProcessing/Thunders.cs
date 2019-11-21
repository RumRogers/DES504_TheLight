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
    
    private void Awake()
    {
        m_postProcessVolume = GetComponent<PostProcessVolume>();
        m_postProcessVolume.profile.TryGetSettings(out m_bloomLayer);
        m_audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        //if(!m_throwing && Random.Range(0f, 1f) >= .85f)
        if (Random.Range(0f, 1f) >= .98f)
        {
            m_throwLightning = true;
        }

        if(m_throwLightning)
        {
            m_throwLightning = false;
            StartCoroutine(ThrowLightning());
        }
    }

    private IEnumerator ThrowLightning()
    {
        m_throwing = true;
        m_bloomLayer.threshold.value = 0f;
        if(!m_audioSource.isPlaying)
        {
            m_audioSource.PlayOneShot(m_audioSource.clip);
        }
        

        while(m_bloomLayer.threshold.value < 1)
        {
            yield return new WaitForSeconds(0.01f);
            m_bloomLayer.threshold.value += Time.deltaTime * m_lightSpeedMultiplier;
        }

        m_bloomLayer.threshold.value = 1;

        yield return new WaitForSeconds(Random.Range(1, 6));
        m_throwing = false;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public enum SoundID
    {
        BackgroundFX,
        PlayerWalk,
        PlayerRun,
        PlayerJump,
        PlayerLand,
        ItemPickUp,
        Pipe_Rotate
    }

    public static SoundManager Instance { get; private set; }    
    
    private AudioSource m_audioSource;
    private Dictionary<SoundID, AudioClip> m_audioBindings = new Dictionary<SoundID, AudioClip>();

    [SerializeField] private AudioClip m_backgroundFX;
    [SerializeField] private AudioClip m_playerWalkFX;
    [SerializeField] private AudioClip m_playerJumpFX;
    [SerializeField] private AudioClip m_playerRunFX;
    [SerializeField] private AudioClip m_playerLandFX;
    [SerializeField] private AudioClip m_itemPickUpFX;
    [SerializeField] private AudioClip m_turnPipeFX;

    private void Awake()
    {
        if(Instance == null)
        {
           Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
        m_audioSource = GetComponent<AudioSource>();
        m_audioBindings[SoundID.BackgroundFX] = m_backgroundFX;
        m_audioBindings[SoundID.PlayerWalk] = m_playerWalkFX;
        m_audioBindings[SoundID.PlayerJump] = m_playerJumpFX;
        m_audioBindings[SoundID.PlayerRun] = m_playerRunFX;
        m_audioBindings[SoundID.PlayerLand] = m_playerLandFX;
        m_audioBindings[SoundID.ItemPickUp] = m_itemPickUpFX;
        m_audioBindings[SoundID.Pipe_Rotate] = m_turnPipeFX;
    }

    // Start is called before the first frame update
    void Start()
    {       
        //PlaySound(SoundID.BackgroundFX, true, 1f, true);
    }


    public void PlaySound(SoundID soundID, AudioSource audioSource = null, bool oneshot = false, float volume = 1f, bool loop = false)
    {
        return;
        if(audioSource == null)
        {
            audioSource = m_audioSource;
        }
        
        audioSource.clip = m_audioBindings[soundID];
        audioSource.volume = volume;
        audioSource.loop = loop;

        if (oneshot)
        {            
            if(!audioSource.isPlaying)
            {
                audioSource.PlayOneShot(m_audioBindings[soundID], volume);
            }
            
        }
        else
        {
            audioSource.clip = m_audioBindings[soundID];
            audioSource.Play();
        }
    }

    public void Stop(AudioSource audioSource)
    {
        audioSource.Stop();
    }
}

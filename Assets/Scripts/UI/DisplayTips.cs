using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayTips : MonoBehaviour
{
    private SpriteRenderer m_spriteRenderer;
    private Animator m_animator;
    private AnimatorClipInfo[] m_animatorClipInfo;
    private const string m_finalAnimationClipName = "TipPosterRotate";
    private PlayerController m_playerController;
    public GameManager.Callback Callback { get; set; }
    private float m_delay = 0f;

    private void Awake()
    {
        m_spriteRenderer = GetComponent<SpriteRenderer>();
        m_animator = GetComponent<Animator>();
        m_playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    public void Display(Sprite sprite)
     {
        m_playerController.IgnoreInput = true;
        m_spriteRenderer.sprite = sprite;
        m_animator.SetTrigger("showTip");
    }

    // Update is called once per frame
    void Update()
    {
        m_delay = Mathf.Max(m_delay - Time.deltaTime, 0f);
        if(m_delay == 0f && Input.anyKeyDown && !Input.GetButtonDown("Cancel"))
        {            
            m_animatorClipInfo = m_animator.GetCurrentAnimatorClipInfo(0);
            if(m_animatorClipInfo[0].clip.name.Equals(m_finalAnimationClipName))
            {
                m_delay = .5f;
                m_animator.SetTrigger("hideTip");
                m_playerController.IgnoreInput = false;                
                if (Callback != null)
                {
                    Callback();
                    Callback = null;
                }
            }
        }
    }
}

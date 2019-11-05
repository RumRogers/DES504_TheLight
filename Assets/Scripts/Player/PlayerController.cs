using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/* TODO: the player shouldn't know anything about specific pipes and ladders...
 * the pipes and ladders scripts should manage inputs in a particular trigger collider 
 * and from there communicate with the player controller... fix asap
*/

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(AudioSource))]

public class PlayerController : MonoBehaviour
{
    struct InputRetrieved
    {
        /* Since we're using GetAxisRaw, x and y integers are enough
         * but just in case we decide to change behavior...
        */
        public float x;
        public float y;
        public bool jump;
        public bool dash;
    };

    struct CurrentLadder
    {
        public Transform transform;
        public Transform top;
        public Transform bottom;
    };
    private CharacterController m_characterController;

    [Header("Cheats")]
    [SerializeField] private bool m_invulnerableToHeight = false;
    [SerializeField] private bool m_noGravity = false;

    // State vars
    [Header("Player state")]
    [SerializeField] private bool m_isGrounded;
    [SerializeField] private int m_lives = 3; // witnesses left
    [SerializeField] private bool m_ignoreInput = false;
    [SerializeField] private bool m_moving = false;
    [SerializeField] private bool m_walking = false;
    [SerializeField] private bool m_jumping = false;
    [SerializeField] private bool m_falling = false;
    [SerializeField] private bool m_crouching = false;
    [SerializeField] private bool m_running = false;
    [SerializeField] private bool m_canJump = true;
    [SerializeField] private bool m_onLadder = false;
    [SerializeField] private bool m_climbing = false;
    [SerializeField] private bool m_sliding = false;
    [SerializeField] private bool m_crawling = false;
    [SerializeField] private bool m_swinging = false;
    [SerializeField] private bool m_stunned = false;
    [SerializeField] private bool m_dead = false;
    [SerializeField] private bool m_hiding = false;    
    [SerializeField] private bool m_ignoreSounds = false;

    private bool m_hasJustJumped = false;
    [SerializeField] private bool m_hasJustLanded = false;
    private Vector3 m_timelineOffset;
    private float m_fallingStart;

    // Actual movement vars
    private Vector3 m_velocity = Vector3.zero; // needed for keeping track of gravity & custom physics
    private Vector3 m_movement = Vector3.zero; // needed for input
    private Quaternion m_rotation; // where is the character facing?

    // Tweak these for fine tuning
    [Header("Movement")]
    [SerializeField] private float m_speed = 7f;
    [SerializeField] [Range(1.1f, 5f)] float m_runningSpeedMultiplier = 1.5f;
    [SerializeField] [Range(0, 1)] private float m_crouchSpeedPercentage = .35f;
    [SerializeField] private float m_jumpForce = 3f;
    [SerializeField] private float m_climbSpeed = 7f;
    [SerializeField] private float m_stunnedTime = 3f;

    [Header("Gravity")]
    [SerializeField] [Range(.1f, 5f)] private float m_gravityOnJumping = .25f;
    [SerializeField] [Range(.1f, 5f)] private float m_gravityOnFalling = 1f;
    [SerializeField] [Range(0f, 1f)] private float m_additionalGravity = 0.05f;
    [SerializeField] [Range(-5f, -.1f)] private float m_gravityLimiter = -.8f;
    [SerializeField] private float m_stunningFallThreshold = 5;
    [SerializeField] private float m_deathFallThreshold = 10;

    [Header("Animations")]
    [SerializeField] private PlayerTimelineController m_timelineController;

    [Header("Misc")]
    [SerializeField] private Transform m_starsSpawningPoint;
    [SerializeField] private GameObject m_starsCirclePrefab;
    private Vector3 m_respawnPoint;

    [Header("Items Carried")]
    [SerializeField] private Inventory.InventoryItems m_currentItem = Inventory.InventoryItems.None;
    [SerializeField] private Transform m_armCrowbar;
    [SerializeField] private Transform m_armWrench;
    private Dictionary<Inventory.InventoryItems, Transform> m_inventoryBindings = new Dictionary<Inventory.InventoryItems, Transform>();


    private InputRetrieved input;
    private CurrentLadder m_currentLadder;
    private GameObject m_currentPipe;
    private Transform m_top;
    private Transform m_bottom;
    private PlayerAnimation m_playerAnimation;
    private AudioSource m_audioSource;

    public bool IgnoreInput { get { return m_ignoreInput;  } set { m_ignoreInput = value; } }
    public bool Dead { get { return m_dead; } set { m_dead = value; } }
    public bool ChangingFloor { get { return m_climbing || m_sliding; } }
    public bool Swinging { get { return m_swinging; } set
        {
            m_swinging = value;
            m_fallingStart = transform.position.y;
        } }
    public bool Crouching { get { return m_crouching; } }
    public bool Climbing { get { return m_climbing; } }
        
    public Vector3 RespawnPoint { get { return m_respawnPoint; } set { m_respawnPoint.x = value.x; m_respawnPoint.y = value.y + 1f; } }

    public Inventory.InventoryItems CurrentItem { get { return m_currentItem; } }

    private void Awake()
    {
        transform.SetParent(null);
        m_respawnPoint = transform.position;
        m_characterController = GetComponent<CharacterController>();
        m_playerAnimation = GetComponent<PlayerAnimation>();
        m_rotation = Quaternion.Euler(0, -90, 0);
        m_top = transform.Find("Head");
        m_bottom = transform.Find("Feet");
        m_inventoryBindings[Inventory.InventoryItems.Crowbar] = m_armCrowbar;
        m_inventoryBindings[Inventory.InventoryItems.MonkeyWrench] = m_armWrench;
        m_audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        m_isGrounded = m_characterController.isGrounded;
        GetInput();

        if (GameManager.Instance.GamePaused)
        {
            return;
        }

        if (!m_ignoreInput && !m_stunned && !m_hasJustLanded)
        {
            ManageInput(ref input);    
        }

        if(!m_stunned && !m_swinging)
        {
            ApplyRotation();
            ApplyMovement();
        }        
        
        NotifyAnimator();

        if(!m_ignoreSounds)
        {
            ManageSound();
        }    
    }

    private void GetInput()
    {
        if(Input.GetButtonDown("Cancel"))
        {
            GameManager.Instance.SetPause(!GameManager.Instance.GamePaused, true);
        }

        input.x = Input.GetAxisRaw("Horizontal");
        input.y = Input.GetAxisRaw("Vertical");
        input.jump = Input.GetButton("Jump");
        input.dash = Input.GetButton("Dash");

        if(!m_canJump)
        {
            m_canJump = Input.GetButtonUp("Jump");
        }
    }

    // TODO: player should be able to start dashing only when grounded
    private void ManageInput(ref InputRetrieved input)
    {        
        m_movement = Vector3.zero; // Reset movement each frame
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if(Inventory.Instance.ContainsItem(Inventory.InventoryItems.Crowbar))
            {
                SetCurrentItem(Inventory.InventoryItems.Crowbar);
            }
        }
        else if(Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (Inventory.Instance.ContainsItem(Inventory.InventoryItems.MonkeyWrench))
            {
                SetCurrentItem(Inventory.InventoryItems.MonkeyWrench);
            }
        }
        else if(Input.GetKeyDown(KeyCode.Alpha0))
        {
            SetCurrentItem(Inventory.InventoryItems.None);
        }

        if (m_onLadder && !m_climbing)
        {           
            if(((input.y < 0 && m_bottom.position.y >= m_currentLadder.bottom.position.y) ||
                (input.y > 0 && m_top.position.y <= m_currentLadder.top.position.y)))
            {             
                StartCoroutine(AttachToLadder()); 
            }
        }
        if(m_climbing)
        {
            if (input.y != 0 && !IsRegularClimbing())
            {
                DetachFromLadder();
            }
            else
            {
                m_movement.y = input.y * m_climbSpeed;
            }       
        }
        else
        {
            if (input.jump && m_characterController.isGrounded && m_canJump)
            {
                m_canJump = false;
                m_jumping = true;
                m_crouching = false;                  
                m_velocity.y += m_jumpForce;
                m_hasJustJumped = true;
            }

            if (input.y != 0 && m_characterController.isGrounded)
            {
                if (input.y < 0 && !m_jumping)
                {
                    if (m_onLadder)
                    {
                        m_climbing = true;
                    }
                    else
                    {
                        m_crouching = true;                        
                    }
                }
            }
            else if (input.y == 0)
            {
                m_crouching = false;
            }

            if (input.x != 0)
            {
                m_moving = true;
                float speed = input.x * m_speed;

                if (m_crouching)
                {
                    //speed *= m_crouchSpeedPercentage;
                    //m_crawling = true;
                    speed = 0;
                }
                else if (!input.dash)                
                {                    
                    m_walking = false;
                    m_running = true;
                    
                    speed *= m_runningSpeedMultiplier;
                }
                else
                {
                    m_walking = true;
                    m_running = false;                    
                    m_crawling = false;
                }

                m_movement.x = speed;

                float xRot = transform.rotation.eulerAngles.x;
                float zRot = transform.rotation.eulerAngles.z;

                if (input.x < 0)
                {
                    m_rotation = Quaternion.Euler(xRot, 90, zRot);
                }
                else
                {
                    m_rotation = Quaternion.Euler(xRot, -90, zRot);
                }
            }
            else
            {
                m_walking = false;
                m_running = false;
                m_crawling = false;
                m_moving = false;
            }
        }
    }

    private void ApplyMovement()
    {
        if (!m_hasJustLanded)
        {
            m_characterController.Move(m_movement * Time.deltaTime);
        }
        
        if(!m_climbing && !m_swinging)
        {
            if (m_velocity.y > 0) // Going up!
            {
                m_velocity.y += (Physics.gravity.y * m_gravityOnJumping * Time.deltaTime);
            }
            else if(m_velocity.y < -0.3f) // DO NOT TOUCH THIS!!!!!! No-falling threshold            
            //else if(m_velocity.y < 0)
            {
                if(m_fallingStart == 0)
                {
                    m_fallingStart = transform.position.y;
                }
                m_velocity.y += (Physics.gravity.y * m_gravityOnFalling * Time.deltaTime);
                m_jumping = false;
                m_falling = true;
            }

            if (!input.jump || m_velocity.y <= 0) // not pressing space or simply falling
            {

                m_velocity.y += (Physics.gravity.y * m_additionalGravity * Time.deltaTime);
            }

            // avoid realistic gravity, just use a Mario-ish limiter 
            if (m_velocity.y < m_gravityLimiter)
            {
                m_velocity.y = m_gravityLimiter;
            }

            m_characterController.Move(m_velocity);            

            if (m_characterController.isGrounded)
            {
                if(m_falling)
                {
                    m_hasJustLanded = true;
                    //print("just landed!");
                    float fellFor = m_fallingStart - transform.position.y;
                    if(!m_invulnerableToHeight)
                    {
                        if (fellFor >= m_deathFallThreshold)
                        {
                            GameManager.Instance.ShowScreen(GameManager.UIScreen.MissionFailed, "You fell to your death. Whoops.");
                        }
                        else if (fellFor >= m_stunningFallThreshold)
                        {
                            StartCoroutine(GetStunned());
                        }
                    }
                    
                    //print("You fell for " + fellFor + " meters.");
                    m_fallingStart = 0;
                }
                m_jumping = false;
                m_falling = false;
                m_velocity.y = 0;
            }

            if(m_sliding)
            {
                m_fallingStart = transform.position.y;
            }
        }
        
    }

    private void ApplyRotation()
    {
        if (m_dead)
        {
            transform.Rotate(9, 9, 9);
        }
        /*else if (!m_moving && !m_falling && !m_crouching)
        {
            transform.rotation = Quaternion.identity;
        }*/
        else
        {
            transform.rotation = m_rotation;            
        }
    }

    private void OnTriggerEnter(Collider other)
    {        
        if (other.CompareTag("Ladder"))
        {            
            m_onLadder = true;
            m_currentLadder.transform = other.gameObject.transform;
            m_currentLadder.top = other.transform.GetChild(3).transform;
            m_currentLadder.bottom = other.transform.GetChild(4).transform;       
        }
    }

    private void OnTriggerExit(Collider other)
    {        
        if (other.CompareTag("Ladder"))
        {            
            if (m_currentLadder.transform != null)
            {
                m_onLadder = false;
                m_currentLadder.transform = null;
            }
        }
    }

    private IEnumerator AttachToLadder()
    {        
        m_crouching = false;
        yield return new WaitForEndOfFrame();
        //m_playerAnimation.GetAnimator().runtimeAnimatorController = null;
        m_playerAnimation.GetAnimator().enabled = false;
        m_climbing = true;
        Vector3 newPos = m_currentLadder.top.position;
        //newPos.z = m_currentLadder.top.position.z;
        gameObject.SetActive(false);
        transform.position = new Vector3(newPos.x, transform.position.y, newPos.z);
        m_rotation = Quaternion.Euler(0, 180f, 0);
        gameObject.SetActive(true);
    }

    private void DetachFromLadder()
    {     
        gameObject.SetActive(false);
        m_climbing = false;
        m_onLadder = false;        
        Vector3 newPos = m_currentLadder.transform.position;
        newPos.z = m_currentLadder.bottom.position.z;
        transform.position = new Vector3(newPos.x, transform.position.y - .1f, newPos.z);
        m_fallingStart = transform.position.y;
        m_rotation = Quaternion.Euler(0, 90f, 0);
        gameObject.SetActive(true);
        m_playerAnimation.GetAnimator().enabled = true;
    }

  
    private bool IsRegularClimbing()
    {
        try
        {
            return ((input.y < 0 && m_top.position.y >= m_currentLadder.bottom.position.y) ||
                (input.y > 0 && m_bottom.position.y <= m_currentLadder.top.position.y));
        }
        catch(NullReferenceException ex)
        {            
            return false;
        }
    }

    public IEnumerator GrabPipe(Vector3 pipeHotspot, Transform pipeEnd, Pipe.PipeDirection pipeDir)
    {       
        m_velocity = Vector3.zero;
        m_movement = Vector3.zero;
        m_sliding = true;
        gameObject.SetActive(false);
        transform.position = new Vector3(pipeHotspot.x, transform.position.y, transform.position.z);
        gameObject.SetActive(true);
        IgnoreInput = true;
        m_playerAnimation.UseTimeline(true);
        GameManager.Callback callback = () =>
        {            
            m_rotation = Quaternion.Euler(0, 90f, 0);
            IgnoreInput = false;
            m_sliding = false;
            m_playerAnimation.UseTimeline(false);
        };
        yield return StartCoroutine(m_timelineController.GrabPipe(transform, pipeEnd, pipeDir, callback));
    }

    public void Die()
    {
        m_dead = true;
        m_ignoreInput = true;
        ManagePlatformsColliders.Instance.DetectCollisions(false);
        m_movement = Vector3.zero;
        GameManager.Instance.ShowScreen(GameManager.UIScreen.MissionFailed, "Your brains are all over the floor. Clean that mess.");
    }

    public void Respawn()
    {
        gameObject.SetActive(false);
        SetCurrentItem(Inventory.InventoryItems.None);
        ManagePlatformsColliders.Instance.DetectCollisions(true);
        gameObject.SetActive(false);
        transform.position = m_respawnPoint;
        transform.rotation = Quaternion.identity;
        gameObject.SetActive(true);        
        ResetState();        
        gameObject.SetActive(true);
        
    }

    private void SetCurrentItem(Inventory.InventoryItems item)
    {
        foreach(var x in m_inventoryBindings)
        {
            if(x.Key == item)
            {
                x.Value.gameObject.SetActive(true);
            }
            else
            {
                x.Value.gameObject.SetActive(false);
            }
        }

        m_currentItem = item;
    }

    private void NotifyAnimator()
    {
        m_playerAnimation.SetBool("isMoving", m_moving);
        m_playerAnimation.SetBool("isWalking", m_walking);
        m_playerAnimation.SetBool("isRunning", m_running);
        m_playerAnimation.SetBool("isJumping", m_jumping);
        m_playerAnimation.SetBool("isFalling", m_falling);
        m_playerAnimation.SetBool("isCrouching", m_crouching);
        m_playerAnimation.SetBool("isCrawling", m_crawling);
        m_playerAnimation.SetBool("isSwinging", m_swinging);
        if (m_hasJustLanded)
        {
            m_playerAnimation.SetBool("hasJustLanded", m_hasJustLanded);
        }
        else
        {
            m_playerAnimation.SetBool("isGrounded", m_characterController.isGrounded);
        }        
    }

    private void ManageSound()
    {
        if(m_falling)
        {
            SoundManager.Instance.Stop(m_audioSource);
        }
        else if(m_hasJustJumped)
        {
            m_hasJustJumped = false;
            SoundManager.Instance.PlaySound(SoundManager.SoundID.PlayerJump, m_audioSource, false, .5f);
        }
        else if(m_hasJustLanded)
        {
            //m_hasJustLanded = false;
            //SoundManager.Instance.PlaySound(SoundManager.SoundID.PlayerLand, m_audioSource, false, .5f);
            //StartCoroutine(IgnoreSoundForSeconds(.2f));
        }
        else if(!m_jumping && m_walking)
        {
            SoundManager.Instance.PlaySound(SoundManager.SoundID.PlayerWalk, m_audioSource, true, 1f);
        }
        else if(!m_jumping && m_running)
        {
            SoundManager.Instance.PlaySound(SoundManager.SoundID.PlayerRun, m_audioSource, true, .5f);
        }
        
    }

    private void ResetState() // TODO: use Resettable superclass instead
    {
        m_ignoreInput = false;
        m_moving = false;
        m_walking = false;
        m_jumping = false;
        m_falling = false;
        m_crouching = false;
        m_running = false;
        m_canJump = true;
        m_onLadder = false;
        m_climbing = false;
        m_sliding = false;
        m_crawling = false;
        m_dead = false;
        ResetHasJustLanded();
    }

    private IEnumerator IgnoreSoundForSeconds(float seconds)
    {
        m_ignoreSounds = true;
        yield return new WaitForSeconds(seconds);
        m_ignoreSounds = false;
    }

    public IEnumerator HideTo(Vector3 position)
    {
        if(m_hiding || m_moving || m_crouching || m_falling || m_crawling)
        {
            yield break;
        }

        m_hiding = true;
        IgnoreInput = true;

        yield return new WaitForEndOfFrame();
        gameObject.SetActive(false);
        transform.position = position;
        gameObject.SetActive(true);

        
    }

    public IEnumerator LeaveHidingSpot(Vector3 position)
    {
        if (!m_hiding)
        {
            yield break;
        }

        m_hiding = false;

        //IgnoreInput = true;

        yield return new WaitForEndOfFrame();

        gameObject.SetActive(false);
        transform.position = position;
        gameObject.SetActive(true);
        IgnoreInput = false;
    }

    private IEnumerator GetStunned()
    {
        m_moving = false;
        m_walking = false;
        m_running = false;
        m_stunned = true;
        GameObject stars = Instantiate(m_starsCirclePrefab, m_starsSpawningPoint);        
        yield return new WaitForSeconds(m_stunnedTime);
        Destroy(stars);
        m_stunned = false;
    }
    public void TakeDamage(int damage = 1)
    {
        m_lives -= damage;

        if (m_lives == 0)
        {
            GameManager.Instance.ShowScreen(GameManager.UIScreen.MissionFailed, "Too many witnesses! You have been identified.");
        }

        GameManager.Instance.UpdateWitnessesUI(m_lives);
    }

    public IEnumerator DoSwing(Vector3 anchorPoint)
    {
        m_ignoreInput = true;
        m_swinging = true;
        gameObject.SetActive(false);
        transform.position = anchorPoint;
        gameObject.SetActive(true);
        m_playerAnimation.SetBool("isSwinging", true);
        yield return new WaitForSeconds(Time.deltaTime);
        m_playerAnimation.GetAnimator().enabled = false;        
    }

    public IEnumerator StopSwinging()
    {        
        transform.SetParent(null);
        transform.rotation = Quaternion.identity;        
        m_ignoreInput = false;
        m_swinging = false;
        yield return new WaitForSeconds(Time.deltaTime);
        m_playerAnimation.GetAnimator().enabled = true;
    }

    public void ResetHasJustLanded()
    {
        //print("resetHasJustLanded");
        m_hasJustLanded = false;
        m_playerAnimation.GetAnimator().SetBool("hasJustLanded", false);
    }
}
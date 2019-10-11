using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* TODO: the player shouldn't know anything about specific pipes and ladders...
 * the pipes and ladders scripts should manage inputs in a particular trigger collider 
 * and from there communicate with the player controller... fix asap
*/

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    public delegate void Callback();

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
        public bool lowerThanCharacter;
    };
    private CharacterController m_characterController;

    // State vars
    [Header("Player state")]
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
    [SerializeField] private bool m_dead = false;

    private Vector3 m_timelineOffset;

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

    [Header("Gravity")]
    [SerializeField] [Range(.1f, 5f)] private float m_gravityOnJumping = .25f;
    [SerializeField] [Range(.1f, 5f)] private float m_gravityOnFalling = 1f;
    [SerializeField] [Range(0f, 1f)] private float m_additionalGravity = 0.05f;
    [SerializeField] [Range(-5f, -.1f)] private float m_gravityLimiter = -.8f;

    [Header("Animations")]
    [SerializeField] private PlayerTimelineController m_timelineController;

    [Header("Misc")]
    [SerializeField] private Transform m_respawnPoint;

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

    public bool IgnoreInput { get { return m_ignoreInput;  } set { m_ignoreInput = value; } }
    public bool Dead { get { return m_dead; } set { m_dead = value; } }
    public bool ChangingFloor { get { return m_climbing || m_sliding; } }
    public Inventory.InventoryItems CurrentItem { get { return m_currentItem; } }

    private void Awake()
    {
        transform.SetParent(null);
        m_characterController = GetComponent<CharacterController>();
        m_playerAnimation = GetComponent<PlayerAnimation>();
        m_rotation = Quaternion.Euler(0, -90, 0);
        m_top = transform.Find("Head");
        m_bottom = transform.Find("Feet");
        m_inventoryBindings[Inventory.InventoryItems.Crowbar] = m_armCrowbar;
        m_inventoryBindings[Inventory.InventoryItems.MonkeyWrench] = m_armWrench;
    }

    // Update is called once per frame
    void Update()
    {
        GetInput();

        if (!m_ignoreInput)
        {
            ManageInput(ref input);
            
        }

        ApplyRotation();
        ApplyMovement();
        NotifyAnimator();
    }

    private void GetInput()
    {
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
                AttachToLadder(); 
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
                    speed *= m_crouchSpeedPercentage;
                    m_crawling = true;
                }
                else if (input.dash)
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

        //m_crawling = m_crouching && m_walking;
    }

    private void ApplyMovement()
    {
        m_characterController.Move(m_movement * Time.deltaTime);

        if(!m_climbing)
        {
            if (m_velocity.y > 0)
            {
                m_velocity.y += (Physics.gravity.y * m_gravityOnJumping * Time.deltaTime);
            }
            else
            {
                m_velocity.y += (Physics.gravity.y * m_gravityOnFalling * Time.deltaTime);
                m_jumping = false;
                m_falling = true;
            }

            if (!input.jump)
            {

                m_velocity.y += (Physics.gravity.y * m_additionalGravity * Time.deltaTime);
            }

            if (m_velocity.y < m_gravityLimiter)
            {
                m_velocity.y = m_gravityLimiter;
            }
            m_characterController.Move(m_velocity);

            if (m_characterController.isGrounded)
            {
                m_jumping = false;
                m_falling = false;
                m_velocity.y = 0;
            }
        }
        
    }

    private void ApplyRotation()
    {
        if (m_dead)
        {
            transform.Rotate(9, 9, 9);
        }
        else if (!m_moving && !m_falling && !m_crouching)
        {
            transform.rotation = Quaternion.identity;
        }
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
            m_onLadder = false;
            m_currentLadder.transform = null;
            m_currentLadder.top = null;
            m_currentLadder.bottom = null;
        }
    }

    private void AttachToLadder()
    {
        m_climbing = true;
        Vector3 newPos = m_currentLadder.transform.position;
        gameObject.SetActive(false);
        transform.position = new Vector3(newPos.x, transform.position.y, newPos.z - .5f);
        m_rotation = Quaternion.Euler(0, 180f, 0);
        gameObject.SetActive(true);
    }

    private void DetachFromLadder()
    {       
        gameObject.SetActive(false);
        m_climbing = false;
        m_onLadder = false;
        transform.position += new Vector3(0f, -.1f, 1f);
        m_rotation = Quaternion.Euler(0, 90f, 0);
        gameObject.SetActive(true);
    }

  
    private bool IsRegularClimbing()
    {
        return ((input.y < 0 && m_top.position.y >= m_currentLadder.bottom.position.y) ||
                (input.y > 0 && m_bottom.position.y <= m_currentLadder.top.position.y));
    }

    public void SetCurrentPipe(GameObject pipe)
    {
        m_currentPipe = pipe;
        print("Current pipe: " + pipe);
    }

    public IEnumerator GrabPipe(Vector3 pipeHotspot, Transform pipeEnd, Pipe.PipeDirection pipeDir)
    {
        //Vector3 initialPos = transform.position;
        //float tLerp = 0;

        //transform.LookAt(pipeHotspot);

        /*while (transform.position != pipeHotspot)
        {
            print((transform.position - pipeHotspot).magnitude);
            tLerp += Time.deltaTime;
            transform.position = Vector3.Lerp(initialPos, pipeHotspot, tLerp);
            yield return new WaitForEndOfFrame();            
        }*/

        m_velocity = Vector3.zero;
        m_movement = Vector3.zero;
        m_sliding = true;
        gameObject.SetActive(false);
        transform.position = new Vector3(pipeHotspot.x, transform.position.y, transform.position.z);
        gameObject.SetActive(true);
        IgnoreInput = true;
        m_playerAnimation.UseTimeline(true);
        Callback callback = () =>
        {
            m_rotation = Quaternion.Euler(0, 90f, 0);
            IgnoreInput = false;
            m_sliding = false;
            m_playerAnimation.UseTimeline(false);
        };
        yield return StartCoroutine(m_timelineController.GrabPipe(transform, pipeEnd, pipeDir, callback));

        //m_playerAnimation.SetTrigger("GrabPipe");
    }

    public void Die()
    {
        m_dead = true;
        m_ignoreInput = true;
        ManagePlatformsColliders.Instance.DetectCollisions(false);
        m_movement = Vector3.zero;
        //transform.rotation = Quaternion.Euler(180, 180, 45);
        /*StartCoroutine(m_timelineController.Die(transform, () => 
        {
            //transform.rotation = Quaternion.Euler(0, 180, 180);            
        }));*/
    }

    public void Respawn(Vector3 respawnPoint)
    {
        gameObject.SetActive(false);
        SetCurrentItem(Inventory.InventoryItems.None);
        ManagePlatformsColliders.Instance.DetectCollisions(true);
        gameObject.SetActive(false);
        transform.position = respawnPoint;
        transform.rotation = Quaternion.identity;
        gameObject.SetActive(true);
        //m_ignoreInput = false;
        //m_dead = false;
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
        m_playerAnimation.SetBool("isGrounded", m_characterController.isGrounded);
        //m_playerAnimation.SetBool("isStill", !m_moving && !m_falling && !m_crouching && m_characterController.isGrounded);
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
    }

}
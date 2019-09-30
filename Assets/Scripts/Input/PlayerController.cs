﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(CharacterController))]
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

    private CharacterController m_characterController;

    // State vars
    [Header("Player state")]
    [SerializeField] private bool m_walking = false;
    [SerializeField] private bool m_jumping = false;
    [SerializeField] private bool m_falling = false;
    [SerializeField] private bool m_crouching = false;
    [SerializeField] private bool m_running = false;
    [SerializeField] private bool m_canJump = true;
    [SerializeField] private bool m_onLadder = false;
    [SerializeField] private bool m_climbing = false;

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

    [Header("Gravity")]
    [SerializeField] [Range(.1f, 5f)] private float m_gravityOnJumping = .25f;
    [SerializeField] [Range(.1f, 5f)] private float m_gravityOnFalling = 1f;
    [SerializeField] [Range(0f, 1f)] private float m_additionalGravity = 0.05f;
    [SerializeField] [Range(-5f, -.1f)] private float m_gravityLimiter = -.8f;
 
    private InputRetrieved input;
    private GameObject m_currentLadder;

    private void Awake()
    {
        m_characterController = GetComponent<CharacterController>();
        m_rotation = Quaternion.Euler(0, -90, 0);
    }

    // Update is called once per frame
    void Update()
    {
        GetInput();
        ManageInput(ref input);
        ApplyMovement();
        ApplyRotation();
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

        if (m_onLadder)
        {
            if (input.y != 0)
            {
                AnchorToLadder(); 
            }
        }
        if(m_climbing)
        {
            m_movement.y = input.y;
            
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
                float speed = input.x * m_speed;

                if (m_crouching)
                {
                    speed *= m_crouchSpeedPercentage;
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
            }
        }
        
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
        transform.rotation = m_rotation;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ladder"))
        {
            m_onLadder = true;
            m_currentLadder = other.gameObject;
      
            /*Vector3 newPos = other.transform.position;        
            gameObject.SetActive(false);
            transform.position = new Vector3(newPos.x, transform.position.y, transform.position.z);
            gameObject.SetActive(true);*/
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Ladder"))
        {
            //print(transform.position);
            m_onLadder = false;
        }
    }

    private void AnchorToLadder()
    {
        m_climbing = true;
        Vector3 newPos = m_currentLadder.transform.position;
        gameObject.SetActive(false);
        transform.position = new Vector3(newPos.x, transform.position.y, transform.position.z);
        gameObject.SetActive(true);
    }
}
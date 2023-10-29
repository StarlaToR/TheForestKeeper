using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterMouvements : MonoBehaviour
{
    public enum MovementState
    {
        IDLE,
        RUN,
        BOUNCE,
    }

    [Header("Mouvement Parameters")]
    [SerializeField] private MovementState m_moveState;
    [SerializeField] private float m_runSpeed;
    [SerializeField] private float m_speedBoostByWater;
    [SerializeField] private float m_speedBoostCost = 1.0f;
    [SerializeField] private float m_speedBoostActivationCost = 10.0f;

    private float m_currentLimitSpeed = 0.0f;
    [SerializeField] private float m_acceleration = 2.0f;
    [SerializeField] private float m_decceleration = 3.0f;

    [SerializeField] private float m_currentSpeed;
    private Rigidbody m_rigidbody;
    private Vector3 m_inputDirection;

    // Rebound Variable 
    [Header("ReboundVariable")]
    public Vector3 m_direction;
    public float m_minPower = 5;
    public float m_maxPower = 10;
    public float m_currentPower = 5;
    public float m_deccelerationPower = 2.5f;
    public float m_stopSpeedToBounce = 1.5f;


    [Header("Sounds Parameters")]
    [SerializeField] private float m_walkTime = 0.2f;
    private float m_walkTimer = 0;
    public SoundEventClass walkForestEvent;
    public SoundEventClass walkStoneEvent;
    public SoundEventClass bounceEvent;
    [SerializeField] private SoundEventClass m_boostSoundEvent;

    private bool m_waterBoostInput;
    private bool m_isBoosting;
    private CharacterWater m_characterWater;

    [SerializeField] private Animator animator;

    public void Start()
    {
        InitComponents();
        walkForestEvent.InitSound();
        walkStoneEvent.InitSound();
        m_boostSoundEvent.InitSound();
        bounceEvent.InitSound();
    }

    private void InitComponents()
    {
        m_rigidbody = GetComponent<Rigidbody>();
        m_characterWater = GetComponent<CharacterWater>();
    }

    public void MouvementInput(InputAction.CallbackContext ctx)
    {
        if (ctx.performed || TimeManager.currentPhase != TimeManager.TimePhase.SLEEP)
        {
            Vector2 direction = ctx.ReadValue<Vector2>();
            m_inputDirection.z = direction.y;
            m_inputDirection.x = direction.x;
            m_currentLimitSpeed = m_runSpeed;

            m_moveState = MovementState.RUN;
            animator.SetBool("is walking", true);

        }
        if (ctx.canceled ||TimeManager.currentPhase == TimeManager.TimePhase.SLEEP)
        {
            m_inputDirection = Vector3.zero;
            walkForestEvent.StopSoundFadeout();

            if (m_moveState == MovementState.RUN)
            {
                m_moveState = MovementState.IDLE;
                animator.SetBool("is walking", false);
            }
        }
    }

    public void WaterBoostInput(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            if (m_characterWater.currentCarriedWater > 0)
            {
                m_waterBoostInput = true;
                m_characterWater.LoseWater(m_speedBoostActivationCost);
                m_boostSoundEvent.PlaySound();
                animator.SetBool("is running", true);
            }
        }

        if (ctx.canceled)
        {
            m_waterBoostInput = false;
            animator.SetBool("is running", false);
        }
    }

    public void FixedUpdate()
    {
        if (GameState.GetState() == GameState.State.PAUSE || TimeManager.currentPhase == TimeManager.TimePhase.SLEEP)
        {
            m_rigidbody.rotation = transform.rotation;
            m_rigidbody.angularVelocity = Vector3.zero;
            m_rigidbody.velocity = Vector3.zero;
            animator.SetBool("is walking", false);
            animator.SetBool("is running", false);
            return;
        }
        if (m_inputDirection != Vector3.zero)
        {
            float angle = Vector3.SignedAngle(transform.forward, m_inputDirection, Vector3.up);
            transform.rotation *= Quaternion.AngleAxis(angle, Vector3.up);
        }

        if (m_moveState == MovementState.RUN)
        {
            RaycastHit hit = new RaycastHit();
            if (Physics.Raycast(transform.position, Vector3.down, out hit, 3.0f))
            {

                if (m_walkTimer > m_walkTime)
                {
                    m_walkTimer = 0;
                    if (hit.collider.tag == "GroundForest") walkForestEvent.PlaySound();
                    if (hit.collider.tag == "GroundStone") walkStoneEvent.PlaySound();
                }
                else
                {
                    m_walkTimer += Time.deltaTime;
                }
            }

            if (m_waterBoostInput && m_inputDirection != Vector3.zero && m_characterWater.currentCarriedWater > 0f)
            {
                m_characterWater.LoseWater(m_speedBoostCost * TimeManager.GetDeltaTime());
                m_currentLimitSpeed = m_runSpeed + m_speedBoostByWater;
                m_isBoosting = true;
            }
            else
            {
                m_boostSoundEvent.StopSoundFadeout();
                m_isBoosting = false;
            }
            m_rigidbody.rotation = transform.rotation;
            m_rigidbody.angularVelocity = Vector3.zero;

            m_currentSpeed += m_acceleration * Time.deltaTime;

            m_rigidbody.AddForce(m_inputDirection.normalized * m_currentSpeed, ForceMode.Impulse);
            m_rigidbody.AddForce(m_inputDirection.normalized * m_currentSpeed, ForceMode.Impulse);

            m_currentSpeed = Mathf.Clamp(m_currentSpeed, 0, m_currentLimitSpeed);
            m_rigidbody.velocity = Vector3.ClampMagnitude(m_rigidbody.velocity, m_currentLimitSpeed);
        }
        else
        {
            m_rigidbody.rotation = transform.rotation;
            m_rigidbody.angularVelocity = Vector3.zero;
            m_currentLimitSpeed -= m_decceleration * Time.deltaTime;
            if (m_currentLimitSpeed <= 0) m_currentLimitSpeed = 0.0f;
            m_rigidbody.AddForce(m_rigidbody.velocity.normalized * m_currentSpeed, ForceMode.Impulse);

            m_currentSpeed = Mathf.Clamp(m_currentSpeed, 0, m_currentLimitSpeed);
            m_rigidbody.velocity = Vector3.ClampMagnitude(m_rigidbody.velocity, m_currentLimitSpeed);
        }


        if (m_moveState == MovementState.BOUNCE)
        {
            m_currentSpeed = m_currentPower;

            m_rigidbody.AddForce(m_direction.normalized * m_currentPower, ForceMode.Impulse);


            m_currentSpeed = Mathf.Clamp(m_currentSpeed, 0, m_currentLimitSpeed);
            m_rigidbody.velocity = Vector3.ClampMagnitude(m_rigidbody.velocity, m_currentLimitSpeed);

            m_currentPower -= m_deccelerationPower * Time.deltaTime;
            if (m_currentPower < m_stopSpeedToBounce)
            {
                m_moveState = MovementState.RUN;
                m_currentLimitSpeed = m_runSpeed;

                animator.SetBool("is walking", true);
            }
        }
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Tree")
        {
            TreeBehavior treeBehavior = collision.gameObject.GetComponent<TreeBehavior>();
            if (m_isBoosting)
            {
                if (treeBehavior.currentTreeState == TreeBehavior.TreeState.Die)
                {
                    treeBehavior.DestroyTree();
                }


                if (treeBehavior.currentTreeState != TreeBehavior.TreeState.Die)
                {
                    m_moveState = MovementState.BOUNCE;
                    float maxSpeed = m_runSpeed + m_speedBoostByWater;
                    float ratio = m_currentSpeed / maxSpeed;
                    m_currentPower = Mathf.Lerp(m_minPower, m_maxPower, ratio);
                    m_currentLimitSpeed = m_minPower;
                    bounceEvent.PlaySound();
                    Vector3 directionToTree = treeBehavior.transform.position - transform.position;
                    float signAngle = Vector3.SignedAngle(transform.forward, directionToTree.normalized, Vector3.up);
                    signAngle = -Mathf.Sign(signAngle);

                    m_direction = Quaternion.Euler(0, signAngle * 10, 0) * m_inputDirection;
                    m_direction.y = 0.0f;
                    treeBehavior.GetComponent<BoxCollider>().isTrigger = true;
                    treeBehavior.CollisionTree();
                    m_rigidbody.AddForce(m_direction.normalized * m_currentPower, ForceMode.Impulse);


                    m_currentSpeed = Mathf.Clamp(m_currentSpeed, 0, m_currentLimitSpeed);
                    m_rigidbody.velocity = Vector3.ClampMagnitude(m_rigidbody.velocity, m_currentLimitSpeed);
                }
            }
        }
    }
}

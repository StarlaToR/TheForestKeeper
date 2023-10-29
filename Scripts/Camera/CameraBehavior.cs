using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehavior : MonoBehaviour
{
    [Header("Camera Parameters")]
    [SerializeField] private GameObject m_target;
    [SerializeField] private Transform m_cameraPos;
    [SerializeField] private float m_targetDistance = 15;
    private Vector3 m_targetDirection;
    private Vector3 m_initRotation;

    [Header("Transition Parameters")]
    [SerializeField] private Transform m_transitionTarget;
    [SerializeField] private bool m_activeTransition;
    [SerializeField] private bool m_onTransition;
    [SerializeField] private float m_transitionPlayerToTimelaspeDuration = 3;
    [SerializeField] public float m_transitionTimelaspeToPlayerDuration = 3;

    private Vector3 m_camStartPositionTransition;
    private Quaternion m_camStartRotationTransition;
    private float m_transitionTimer = 0.0f;


    private CameraShake m_cameraShake;

    // Start is called before the first frame update
    void Start()
    {
        m_cameraShake = GetComponent<CameraShake>();
        transform.rotation = m_cameraPos.rotation;
        transform.position = m_cameraPos.position;
        m_initRotation = transform.eulerAngles;
        m_targetDirection = transform.position - m_target.transform.position;
        m_targetDistance = m_targetDirection.magnitude;
        m_onTransition = false;

    }


    // Update is called once per frame
    void Update()
    {
        if (!m_activeTransition && !m_onTransition)
        {

            Vector3 initPosition = m_cameraShake.GetEffectPos();
            Vector3 initRotation = m_cameraShake.GetEffectRot();

            initPosition += m_target.transform.position + m_targetDirection.normalized * m_targetDistance;
            initRotation += m_initRotation;
            transform.rotation = Quaternion.Euler(initRotation);
            transform.position = initPosition;
        }

        TransitioMovement();
    }

    public void ActiveTransitionCamera()
    {

        if (m_activeTransition == false)
        {
            m_camStartPositionTransition = transform.position;
            m_camStartRotationTransition = transform.rotation;
        }

        m_onTransition = !m_onTransition;
        m_activeTransition = !  m_activeTransition;


    }

    private void TransitioMovement()
    {
        if ( !m_onTransition) return;

        Vector3 startPosition = Vector3.zero;
        Vector3 endPosition = Vector3.zero;
        Quaternion startRotation = Quaternion.identity;
        Quaternion endRotaiton = Quaternion.identity;
        float ratio = 0.0f;

        m_transitionTimer += Time.deltaTime;
        if (m_activeTransition)
        {
            startPosition = m_camStartPositionTransition;
            endPosition = m_transitionTarget.position;
            startRotation = m_camStartRotationTransition;
            endRotaiton = m_transitionTarget.rotation; 
            
            ratio = m_transitionTimer / m_transitionPlayerToTimelaspeDuration;
            if (m_transitionTimer > m_transitionPlayerToTimelaspeDuration)
            {
                m_transitionTimer = 0.0f;
                m_onTransition = false;
            }
        }
        else
        {
            endPosition = m_camStartPositionTransition;
            startPosition = m_transitionTarget.position;

            startRotation = m_transitionTarget.rotation;
            endRotaiton = m_camStartRotationTransition;

            ratio = m_transitionTimer / m_transitionTimelaspeToPlayerDuration;
            if (m_transitionTimer >m_transitionTimelaspeToPlayerDuration)
            {
                m_transitionTimer = 0.0f;
                m_onTransition = false;
            }    
        }

        transform.position = Vector3.Lerp(startPosition, endPosition, ratio);
        transform.rotation = Quaternion.Slerp(startRotation, endRotaiton, ratio);
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD;
using FMODUnity;
public class WaterSource : MonoBehaviour
{
    [Header("Water Source Parameters")]
    public GameObject playerObject;
    public float currentWater = 0f;
    public float waterMax = 100f;
    public float waterGiven = 5f;
    public float regenLimit = 20f;
    public float regenSpeed = 2f;
    public bool regenOn = false;
    public bool infinite = false;
    public float radiusToDrain = 4.0f;

    private bool m_isDrain = false;
    private bool m_isRegenStart = false;


    [Header("Sounds Parameters")]

    private FMOD.Studio.EventInstance m_waterDrainEvent;
    [SerializeField] private EventReference m_waterDrainRef;
    [SerializeField] SoundEventClass m_waterSourceEvent;

    [Header("Debugs Parameter")]
    [SerializeField] private bool m_activeDebug = false;
    [SerializeField] private Color m_debugColor = Color.green;


    private void Start()
    {
        currentWater = waterMax;
        InitSound();

        
    }

    private void InitSound()
    {
        m_waterSourceEvent.InitSound(transform);
        m_waterSourceEvent.PlaySound();
        m_waterDrainEvent = FMODUnity.RuntimeManager.CreateInstance(m_waterDrainRef);
    }

    private void Update()
    {
        if (GameState.GetState() == GameState.State.PAUSE) return;

        if (currentWater < regenLimit)
        {
            if (!regenOn)
            {
                m_waterSourceEvent.StopSoundDirect();
                regenOn = true;
            }
            
        }
        else if (currentWater >= waterMax)
        {
            if (regenOn)
            {
                regenOn = false;
                m_waterSourceEvent.PlaySound();
            }
          
        }
        if (regenOn)
            currentWater += regenSpeed * TimeManager.GetDeltaTime();

        DetectionPlayerDistance();
    }

    public void OnDrawGizmos()
    {
        if (m_activeDebug)
        {
            Gizmos.color = m_debugColor;
            Gizmos.DrawWireSphere(transform.position, radiusToDrain);
        }
    }


    private void DetectionPlayerDistance()
    {
        if (Vector3.Distance(playerObject.transform.position, transform.position) < radiusToDrain)  
        {
            if (currentWater < waterGiven * TimeManager.GetDeltaTime())
            {
                m_waterDrainEvent.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                return;
            }

            CharacterWater water = playerObject.GetComponent<CharacterWater>();
            water.GainWater(waterGiven * TimeManager.GetDeltaTime());
            if (!m_isDrain)
            {
                m_isDrain = true;
                m_waterDrainEvent.start();
            }
            if (!infinite)
                currentWater -= waterGiven * TimeManager.GetDeltaTime();
        }
        else
        {
            if (m_isDrain)
            {
                m_isDrain = false;
                m_waterDrainEvent.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            }
        }
    }


}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using FMOD;

public class CharacterDrain : MonoBehaviour
{
    [Header("Heal Parameters")]
    public float healPower = 20f;
    public float healCost = 20f;
    public float healCooldown = 0.5f;
    private float healTimer = 0f;
    public LayerMask treeLayerMask;

    [Header("Sounds Parameters")]
    public FMODUnity.EventReference inputErrorWater;
    public FMOD.Studio.EventInstance errorWaterEvent;
    public FMODUnity.EventReference inputWaterRef;
    public FMOD.Studio.EventInstance inputWaterEvent;

    private CharacterWater m_characterWater;
    private TreeBehavior treeTarget;


    private bool m_isWaterInput = false;

    [SerializeField] private Animator animator;

    public void Start()
    {
        //m_waterCount.text = "Water :" + currentWaterCarry.ToString("F0");
        m_characterWater = GetComponent<CharacterWater>();
        healTimer = healCooldown;
        InitSound();
    }

    public void InitSound()
    {
        errorWaterEvent = FMODUnity.RuntimeManager.CreateInstance(inputErrorWater);
        inputWaterEvent = FMODUnity.RuntimeManager.CreateInstance(inputWaterRef);
    }

    public void HealthInput(InputAction.CallbackContext ctx)
    {
        if (ctx.started || TimeManager.currentPhase != TimeManager.TimePhase.SLEEP)
        {
            TreeBehavior script = InteractionTrigger.m_treeTarget;
            if (script != null)
            {
                treeTarget = script;
                HealTree();
                inputWaterEvent.start();
                animator.SetBool("is watering", true);
        
            }
            else
            {
                errorWaterEvent.start();
            }
        }
        if (ctx.canceled)
        {
            if (treeTarget != null)
            {
                inputWaterEvent.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
                treeTarget = null;
                animator.SetBool("is watering", false);
                m_isWaterInput = false;
            }
        }
    }

    public void Update()
    {
        if (GameState.GetState() == GameState.State.PAUSE || TimeManager.currentPhase == TimeManager.TimePhase.SLEEP) return;

        if (healTimer > 0f)
            healTimer -= TimeManager.GetDeltaTime();
    }

    public void HealTree()
    {
        if (m_isWaterInput) return;
        m_isWaterInput =true;
        if (m_characterWater.currentCarriedWater < healCost)
        {
            inputWaterEvent.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            return;

        }

        treeTarget.currentWater += healPower;
        m_characterWater.LoseWater(healCost);
        if (treeTarget.isBurning)
        {
            treeTarget.isBurning = false;
            treeTarget.immuneTimer = 10f;
            treeTarget.StopBurningSound();
        }

        healTimer = healCooldown;

        if (treeTarget.currentWater <= 0.0f || InteractionTrigger.m_treeTarget == null)
        {
            treeTarget = null;
        }
    }

}

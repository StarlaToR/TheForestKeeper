using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public enum TimePhase
    {
        START,
        FIRE,
        RAIN,
        SLEEP,
    }

    static private float s_timeMultiplier = 1f;

    private float m_globalTimer = 0f;
    [SerializeField] private float[] m_phaseTimes = { 60f, 60f, 60f, 30f };
    private static float[] s_phaseTimes;
    public static TimePhase currentPhase = TimePhase.START;
    public int m_phaseIndex = 0;
    public CameraBehavior cameraBeheviors;
    public float lapseSpeed = 3f;
    public float normalSpeed = 1f;
    public ForestManager m_forestManager;

    [Header("Sounds Parameters")]
    [SerializeField] private SoundEventClass m_rainEvent;
    [SerializeField] private SoundEventClass m_peaceEvent;
    [SerializeField] private SoundEventClass m_music;
    [SerializeField] private SoundEventClass m_timelapseEvent;
    [SerializeField] private SoundEventClass m_timelapseClock;
    [SerializeField] private SoundEventClass m_timelapseJingle;

    public bool m_transition;

    public void StopMusic()
    {
        m_rainEvent.StopSoundDirect();
        m_peaceEvent.StopSoundDirect();
        m_music.StopSoundDirect();
        m_timelapseEvent.StopSoundDirect();
        m_timelapseJingle.StopSoundDirect();
        m_timelapseClock.StopSoundDirect();
    }

    public static float GetDeltaTime()
    {
        return s_timeMultiplier * Time.deltaTime;

    }

    public static float GetPhaseTime(TimePhase phase)
    {
        return s_phaseTimes[(int)phase];
    }

    public static void ChangeGameSpeed (float multiplier)
    {
        s_timeMultiplier = multiplier;
    }

    private void Start()
    {
        m_phaseIndex =0;
        currentPhase = (TimePhase)m_phaseIndex;
        m_globalTimer = 0.0f;
        s_timeMultiplier = normalSpeed;
        m_rainEvent.InitSound();
        m_peaceEvent.InitSound();
        m_music.InitSound();
        m_timelapseEvent.InitSound();
        m_timelapseClock.InitSound();
        m_timelapseJingle.InitSound();

        m_music.PlaySound();

        ChangeAmbiance(true);
        ChangeMusic(1);

        s_phaseTimes = m_phaseTimes;
    }


    public void ChangeMusic(int index)
    {
        float value = 0.0f;
        FMOD.RESULT rest = m_music.soundEvent.getParameterByName("Zone", out value);
        FMOD.RESULT res = m_music.soundEvent.setParameterByName("Zone", index);

    }

    public void ChangeAmbiance(bool isPeace)
    {
        if (isPeace)
        {
            m_peaceEvent.soundEvent.setParameterByName("Audio", 0);
            m_peaceEvent.PlaySound();
        }
        else
        {
            m_peaceEvent.soundEvent.setParameterByName("Audio", 1);
            m_peaceEvent.PlaySound();
        }
    }



    private void Update()
    {

        if (m_globalTimer < m_phaseTimes[m_phaseIndex])
        {
            m_globalTimer += Time.deltaTime;
            float tempsValue = m_phaseTimes[m_phaseIndex] - m_globalTimer;
            if (m_phaseIndex == 3 && tempsValue< cameraBeheviors.m_transitionTimelaspeToPlayerDuration && m_transition)
            {
                m_transition = false;
                cameraBeheviors.ActiveTransitionCamera();
            }
        }
        else
        {
            m_globalTimer = 0f;

            m_phaseIndex++;



            if (m_phaseIndex == 1)
            {
                ChangeMusic(2);
            }

            if (m_phaseIndex == 2)
            {
                m_rainEvent.PlaySound();
                ChangeMusic(3);
                ChangeAmbiance(true);
            }

            if (m_phaseIndex == 3)
            {
                cameraBeheviors.ActiveTransitionCamera();
                m_timelapseEvent.PlaySound();
                m_timelapseClock.PlaySound();
                m_timelapseJingle.PlaySound();
                m_transition = true;
            }

            if (m_phaseIndex > 3)
            {
                m_phaseIndex = 0;
              
                m_timelapseEvent.StopSoundFadeout();
                m_timelapseClock.StopSoundFadeout();
                m_timelapseJingle.StopSoundFadeout();
                ChangeMusic(1);
            }

            currentPhase = (TimePhase)m_phaseIndex;
        }

        if (currentPhase == TimePhase.SLEEP)
            s_timeMultiplier = lapseSpeed;
        else
            s_timeMultiplier = normalSpeed;
    }
}

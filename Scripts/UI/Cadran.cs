using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cadran : MonoBehaviour
{
    [SerializeField] private MovingImage sun;
    [SerializeField] private MovingImage moon;
    [SerializeField] private MovingImage cloud1;
    [SerializeField] private MovingImage cloud2;
    [SerializeField] private MovingImage rain;

    private TimeManager.TimePhase phase = TimeManager.TimePhase.SLEEP;
    //private bool cloud1On = false;
    //private bool cloud2On = false;

    // Update is called once per frame
    void Update()
    {
        if (GameState.GetState() == GameState.State.PAUSE) return;

        if (phase != TimeManager.currentPhase)
        {
            phase = TimeManager.currentPhase;

            if (phase == TimeManager.TimePhase.START)
            {
                sun.gameObject.SetActive(true);
                sun.travelTime = TimeManager.GetPhaseTime(phase) + TimeManager.GetPhaseTime(TimeManager.TimePhase.FIRE);
            }
            else if (phase == TimeManager.TimePhase.RAIN)
            {
                rain.gameObject.SetActive(true);
                rain.travelTime = TimeManager.GetPhaseTime(phase);
            }
            else if (phase == TimeManager.TimePhase.SLEEP)
            {
                moon.gameObject.SetActive(true);
                moon.travelTime = TimeManager.GetPhaseTime(phase);
            }
        }

        //if (!cloud1.gameObject.activeSelf && !cloud1On)
        //{
        //    cloud1.gameObject.SetActive(true);
        //    cloud1.travelTime = Random.Range(5f, 20f);
        //    cloud1On = true;
        //}

        //if (!cloud2.gameObject.activeSelf)
        //{
        //    cloud2.gameObject.SetActive(true);
        //    cloud2.travelTime = Random.Range(5f, 20f);
        //}
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rain : MonoBehaviour
{
    [SerializeField] private GameObject rainFX;
    private bool raining = false;

    private void Start()
    {
        rainFX.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (TimeManager.currentPhase == TimeManager.TimePhase.RAIN && !raining ) 
        {
            raining = true;
            rainFX.SetActive(true);
        }
        else if (TimeManager.currentPhase != TimeManager.TimePhase.RAIN && raining)
        {
            raining = false;
            rainFX.SetActive(false);
        }
    }
}

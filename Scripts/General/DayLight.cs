using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayLight : MonoBehaviour
{
    private Light dirlight;
    private TimeManager.TimePhase phase = TimeManager.TimePhase.SLEEP;
    private float rotationSpeed;
    private float totalRotation = 0f;
    [SerializeField] private Color[] colors;

    // Start is called before the first frame update
    void Start()
    {
        dirlight = GetComponent<Light>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameState.GetState() == GameState.State.PAUSE) return;

        if (phase != TimeManager.currentPhase)
        {
            phase = TimeManager.currentPhase;
            rotationSpeed = 90f / TimeManager.GetPhaseTime(phase);
        }

        if (phase != TimeManager.TimePhase.SLEEP)
            dirlight.color = Color.Lerp(colors[(int)phase], colors[(int)phase + 1], (totalRotation % 90f) / 90f);
        else
            dirlight.color = Color.Lerp(colors[3], colors[0], (totalRotation % 90f) / 90f);

        totalRotation += rotationSpeed * Time.deltaTime;
        transform.SetPositionAndRotation(transform.position, Quaternion.Euler(new Vector3(50f, totalRotation, 0f)));
    }
}

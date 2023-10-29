using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterSeeds : MonoBehaviour
{
    public int seedAmount = 0;
    public int maxSeeds = 10;
    [SerializeField] private PlantableChecker checker;

    // Start is called before the first frame update
    void Start()
    {
        seedAmount = 0;
        checker.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (TimeManager.currentPhase == TimeManager.TimePhase.SLEEP) seedAmount = 0;
    }

    public void SeedInput(InputAction.CallbackContext ctx)
    {
        if (seedAmount <= 0) return;

        if (ctx.started)
        {
            Debug.Log("Seed");
            checker.gameObject.SetActive(true);
        }
        if (ctx.canceled)
        {
            Debug.Log("TryPlant");
            if (checker.TryPlantTree())
                seedAmount--;
            
            checker.gameObject.SetActive(false);
        }
    }
}

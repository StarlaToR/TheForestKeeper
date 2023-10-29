using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterWater : MonoBehaviour
{
    public float currentCarriedWater = 0.0f;
    public float maxCarriedWater = 100.0f;

    public GameObject waterFX;
    private GameObject prefab;
    private float timer = 0.1f;

    private void Start()
    {
        currentCarriedWater = 0.0f;
        prefab = Instantiate(waterFX);
        prefab.transform.parent = transform;
        prefab.transform.localPosition = Vector3.zero;
        prefab.SetActive(false);
    }

    private void Update()
    {
        if (timer > 0)
        {
            if (!prefab.activeSelf) prefab.SetActive(true);

            timer -= TimeManager.GetDeltaTime();
        }
        else if (prefab.activeSelf) prefab.SetActive(false);
    }

    public void LoseWater(float amount)
    {
        currentCarriedWater -= amount;

        if (currentCarriedWater < 0.0f)
            currentCarriedWater = 0.0f;
    }

    public void GainWater(float amount)
    {
        currentCarriedWater += amount;
        timer = 0.1f;

        if (currentCarriedWater > maxCarriedWater)
            currentCarriedWater = maxCarriedWater;
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seed : MonoBehaviour
{
    private float currentRotation = 0;
    public float rotationSpeed = 1;
    public float lifeTime = 30f;

    private void Update()
    {
        if (lifeTime > 0f)
        {
            lifeTime -= TimeManager.GetDeltaTime();
            currentRotation += rotationSpeed * TimeManager.GetDeltaTime();
            gameObject.transform.SetPositionAndRotation(new Vector3(transform.position.x, 1 + Mathf.Cos(currentRotation) / 2f, transform.position.z), Quaternion.Euler(new Vector3(0, currentRotation * 60, 0)));
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            CharacterSeeds playerSeeds = other.gameObject.GetComponent<CharacterSeeds>();

            if (playerSeeds.seedAmount < playerSeeds.maxSeeds)
            {
                playerSeeds.seedAmount++;
                Destroy(gameObject);
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingImage : MonoBehaviour
{
    public float startingX = -50;
    public float endingX = 250;
    public float travelTime = 0;
    private float timer = 0;

    // Start is called before the first frame update
    void Start()
    {
        timer = travelTime;
    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;
        transform.position = new Vector3(Mathf.Lerp(startingX, endingX, 1 - timer / travelTime), transform.position.y, transform.position.z);

        if (timer < 0f)
        {
            transform.position = new Vector3(startingX, transform.position.y, transform.position.z);
            gameObject.SetActive(false);
        }
    }
}

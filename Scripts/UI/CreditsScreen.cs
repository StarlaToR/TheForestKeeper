using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreditsScreen : MonoBehaviour
{
    [SerializeField] private GameObject upImg1;
    [SerializeField] private GameObject upImg2;
    [SerializeField] private GameObject botImg1;
    [SerializeField] private GameObject botImg2;

    [SerializeField] private float speed = 1f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        upImg1.transform.position = upImg1.transform.position + new Vector3(speed * Time.deltaTime, 0, 0);
        upImg2.transform.position = upImg2.transform.position + new Vector3(speed * Time.deltaTime, 0, 0);
        botImg1.transform.position = botImg1.transform.position - new Vector3(speed * Time.deltaTime, 0, 0);
        botImg2.transform.position = botImg2.transform.position - new Vector3(speed * Time.deltaTime, 0, 0);

        if (upImg1.transform.position.x > 4000) upImg1.transform.position = new Vector3(-2500, upImg1.transform.position.y, upImg1.transform.position.z);
        if (upImg2.transform.position.x > 4000) upImg2.transform.position = new Vector3(-2500, upImg2.transform.position.y, upImg2.transform.position.z);
        if (botImg1.transform.position.x < -2000) botImg1.transform.position = new Vector3(4500, botImg1.transform.position.y, botImg1.transform.position.z);
        if (botImg2.transform.position.x < -2000) botImg2.transform.position = new Vector3(4500, botImg2.transform.position.y, botImg2.transform.position.z);
    }
}

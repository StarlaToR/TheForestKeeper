using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire : MonoBehaviour
{
    [SerializeField] private List<GameObject> fireFX;
    private TreeBehavior tree;
    private bool burning = false;

    // Start is called before the first frame update
    void Start()
    {
        foreach (GameObject gO in fireFX) gO.SetActive(false);

        tree = GetComponent<TreeBehavior>();
    }

    // Update is called once per frame
    void Update()
    {
        if (tree.isBurning && !burning)
        {
            burning = true;
            foreach (GameObject gO in fireFX) gO.SetActive(true);
        }
        else if (!tree.isBurning && burning)
        {
            burning = false;
            foreach (GameObject gO in fireFX) gO.SetActive(false);
        }
    }
}

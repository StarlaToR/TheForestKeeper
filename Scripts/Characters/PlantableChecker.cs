using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantableChecker : MonoBehaviour
{
    [SerializeField] private GameObject green;
    [SerializeField] private GameObject red;
    [SerializeField] private ForestManager manager;
    public LayerMask treeLayerMask;
    public bool isPlantable = false;

    private int currentCaseIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        green.SetActive(false);
        red.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        isPlantable = IsPlantable();

        if (isPlantable)
        {
            green.SetActive(true);
            red.SetActive(false);
        }
        else
        {
            green.SetActive(false);
            red.SetActive(true);
        }
    }

    bool IsPlantable()
    {
        currentCaseIndex = manager.GetIndexZone(transform.position);
        int indexCase = 0;
        if (!manager.CanCreateTree(transform.position, out indexCase, currentCaseIndex)) return false;

        Collider[] colliders = Physics.OverlapSphere(transform.position, 4, treeLayerMask);

        if (colliders.Length != 0)
            return false;

        return true;
    }

    public bool TryPlantTree()
    {
        if (isPlantable)
        {
            GameObject tree = Instantiate(manager.GetTreePrefab());
            tree.transform.SetPositionAndRotation(transform.position, Quaternion.Euler(new Vector3(0, Random.Range(-180f, 180f), 0)));
            manager.AddTree(tree.GetComponent<TreeBehavior>(), currentCaseIndex);
            return true;
        }
        return false;
    }
}

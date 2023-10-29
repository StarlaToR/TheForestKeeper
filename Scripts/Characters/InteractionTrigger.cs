using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionTrigger : MonoBehaviour
{
    public bool activeDebug;
    static public TreeBehavior m_treeTarget;

    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Tree")
        {
            m_treeTarget = other.GetComponent<TreeBehavior>();
           if(activeDebug) Debug.Log(other.name + "is at range");
        }
    }


    public void OnTriggerExit(Collider other)
    {
        if (other.tag == "Tree")
        {
            m_treeTarget = null;
        }
    }
}

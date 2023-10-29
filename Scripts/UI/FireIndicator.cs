using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireIndicator : MonoBehaviour
{
    [SerializeField] private ForestManager manager;
    [SerializeField] private GameObject player;

    [SerializeField] private RectTransform fireIcon;

    [SerializeField] private float iconRadius = 300f;
    [SerializeField] private float indicRadius = 50f;

    // Update is called once per frame
    void Update()
    {
        if (manager.treeBurningArray.Count == 0 || TimeManager.currentPhase != TimeManager.TimePhase.FIRE)
        {
            if (fireIcon.gameObject.activeSelf)
                fireIcon.gameObject.SetActive(false);

            return;
        }
        else
        {
            if (!fireIcon.gameObject.activeSelf)
                fireIcon.gameObject.SetActive(true);
        }

        if (manager.treeBurningArray[0] == null) return;

        Vector2 treePos = new Vector2(manager.treeBurningArray[0].gameObject.transform.position.x, manager.treeBurningArray[0].gameObject.transform.position.z);
        Vector2 playerPos = new Vector2(player.transform.position.x, player.transform.position.z);
        Vector2 dir = (treePos - playerPos).normalized;
        Vector2 forwardVec = Vector2.up;
        float angle = Mathf.Atan2(dir.y * forwardVec.x - dir.x * forwardVec.y, dir.x * forwardVec.x + dir.y * forwardVec.y) + 90f;

        fireIcon.anchoredPosition = new Vector2(iconRadius * Mathf.Cos(angle), iconRadius * Mathf.Sin(angle));
    }
}

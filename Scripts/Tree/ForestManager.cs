using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public struct ZoneData
{
    public int currentIndex;
    public Transform[] treeBehaviors;
    public Vector3 center;

    public void SetNewCurrentIndex()
    {
        currentIndex = 0;
        for (int i = 0; i < treeBehaviors.Length; i++)
        {
            if (treeBehaviors[i] == null)
            {
                break;
            }
            currentIndex++;
        }
    }
};

public class ForestManager : MonoBehaviour
{
    [Header("Forest Parameters")]
    public List<TreeBehavior> treeArray = new List<TreeBehavior>();
    public List<TreeBehavior> treeBurningArray = new List<TreeBehavior>();
    public GameObject treePrefab;
    public GameObject[] treePrefabs = new GameObject[0];
    public List<Vector2> fireParameters1;
    public List<Vector2> fireParameters2;
    public List<Vector2> fireParameters3;
    public List<Vector2> fireParameters4;
    private int currFireIndex = 0;
    private int currCycle = 0;
    private float fireTimer = 0;

    [Header("Terain Parameters")]
    [SerializeField] private Vector2 m_size;
    [SerializeField] private Vector3 m_center;
    [SerializeField] private int m_maxTreeByZone = 5;
    [SerializeField] private Vector2 m_zoneSize = new Vector2(10, 10);
    [SerializeField] private bool m_isTerrainDebugActive;

    [SerializeField] private ZoneData[] zoneDatas = new ZoneData[0];


    //[Header("Sounds Parameters")]
    //[SerializeField] private SoundEventClass m_fireEvent;
    private bool m_isRaining;
    private float m_timerBetweenInfection = 0.0f;
    private TimeManager m_time;

    public void Start()
    {
        m_time = GetComponent<TimeManager>();
        CreateZone();
        SetupBaseTree();
    }

    public void CreateZone()
    {
        int width = (int)(m_size.x / m_zoneSize.x);
        int height = (int)(m_size.y / m_zoneSize.y);
        Vector2 countCase = new Vector2(m_size.x / m_zoneSize.x, m_size.y / m_zoneSize.y);
        zoneDatas = new ZoneData[width * height];

        Vector3 startPoint = m_center + new Vector3(-m_size.x, 0, m_size.y) / 2.0f;

        for (int i = 0; i < width; i++)
        {

            for (int j = 0; j < height; j++)
            {
                ZoneData data = new ZoneData();
                data.center = startPoint + new Vector3(m_zoneSize.x, 0, m_zoneSize.y) / 2.0f;
                data.center.x += m_zoneSize.x * i;
                data.center.z -= m_zoneSize.y * j;
                data.treeBehaviors = new Transform[m_maxTreeByZone];
                zoneDatas[j + i * height] = data;
            }
        }
    }

    public void SetupBaseTree()
    {
        Vector3 startPoint = m_center + new Vector3(-m_size.x, 0, m_size.y) / 2.0f;
        int casePerLine = (int)(m_size.y / m_zoneSize.y);
        for (int i = 0; i < treeArray.Count; i++)
        {
            float xCoord = treeArray[i].transform.position.x - startPoint.x;
            int widthIndex = (int)(xCoord / m_zoneSize.x);

            float yCoord = startPoint.z - treeArray[i].transform.position.z;
            int heightIndex = (int)(yCoord / m_zoneSize.y) +1;
            TreeBehavior treeBehavior = treeArray[i].GetComponent<TreeBehavior>();

            int indexCase = heightIndex + widthIndex * casePerLine;
            
            treeBehavior.currentTransformIndex = zoneDatas[indexCase].currentIndex;
            zoneDatas[indexCase].treeBehaviors[zoneDatas[indexCase].currentIndex] = treeArray[i].transform;
            zoneDatas[indexCase].currentIndex++;


            treeBehavior.currentIndexZone = indexCase;
            treeBehavior.treeGameObject = treePrefab;
            treeBehavior.m_manager = this;
        }
    }

    public int GetIndexZone(Vector3 position)
    {
        Vector3 startPoint = m_center + new Vector3(-m_size.x, 0, m_size.y) / 2.0f;
        int casePerLine = (int)(m_size.x / m_zoneSize.x);
        float xCoord = position.x - startPoint.x;
        int widthIndex = (int)(xCoord / m_zoneSize.x);

        float yCoord = startPoint.y - position.x;
        int heightIndex = (int)(yCoord / m_zoneSize.y);
        int indexCase = heightIndex + widthIndex * casePerLine;

        return indexCase;
    }

    // Check if tree can be create and give the index of zone of spawn;
    public bool CanCreateTree(Vector3 position, out int indexCase, int indexOfSpawn)
    {
        Vector3 size = new Vector3(m_size.x, 0, m_size.y) / 2.0f;
        int casePerLine = (int)(m_size.x / m_zoneSize.x);

        indexCase = 0;

        Vector3 centerCase = zoneDatas[indexOfSpawn].center;
        Vector3 direction = (position - centerCase);
        bool isWidthDistanceShort = Mathf.Abs(direction.x) < m_zoneSize.x;
        bool isHeightDistanceShort = Mathf.Abs(direction.z) < m_zoneSize.y;
        if (isWidthDistanceShort && isHeightDistanceShort)
        {
            if (zoneDatas[indexOfSpawn].currentIndex == m_maxTreeByZone)
            {

                indexCase = 0;
                return false;
            }

            indexCase = indexOfSpawn;
            return true;
        }

        int indexToAdd = 0;
        if (!isWidthDistanceShort)
        {
            indexToAdd += (int)Mathf.Sign(direction.normalized.x) * casePerLine;
        }

        if (!isHeightDistanceShort)
        {
            indexToAdd += (int)Mathf.Sign(-direction.normalized.z) * 1;
        }

        if(indexOfSpawn + indexToAdd >= zoneDatas.Length || indexOfSpawn +  indexToAdd <0)
        {
            indexCase = 0;
            return false;
        }

        if (zoneDatas[indexOfSpawn + indexToAdd].currentIndex == m_maxTreeByZone) 
        {
            indexCase = 0;
            return false;
        }

        indexCase = indexOfSpawn + indexToAdd;
        return true;
    }

    public void Update()
    {
        if (GameState.GetState() == GameState.State.PAUSE) return;

        if (TimeManager.currentPhase == TimeManager.TimePhase.FIRE)
        {
            if (currCycle == 0 && fireParameters1.Count > currFireIndex && fireTimer >= fireParameters1[currFireIndex].x)
            {
                for (int i = 0; i < fireParameters1[currFireIndex].y; i++)
                    SpawnInfection();

                currFireIndex++;
            }
            else if (currCycle == 1 && fireTimer >= fireParameters2[currFireIndex].x)
            {
                for (int i = 0; i < fireParameters2[currFireIndex].y; i++)
                    SpawnInfection();

                currFireIndex++;
            }
            else if (currCycle == 2 && fireTimer >= fireParameters3[currFireIndex].x)
            {
                for (int i = 0; i < fireParameters3[currFireIndex].y; i++)
                    SpawnInfection();

                currFireIndex++;
            }
            else if (currCycle == 3 && fireTimer >= fireParameters4[currFireIndex].x)
            {
                for (int i = 0; i < fireParameters4[currFireIndex].y; i++)
                    SpawnInfection();

                currFireIndex++;
            }

            fireTimer += TimeManager.GetDeltaTime();
        }

        if (TimeManager.currentPhase == TimeManager.TimePhase.RAIN)
        {
            ClearFire();
            fireTimer = 0f;
            currFireIndex = 0;
            currCycle ++;
        }

    }

    public void SpawnInfection()
    {
        int treeIndex = UnityEngine.Random.Range(0, treeArray.Count);
        treeArray[treeIndex].SetBurning();
    }

    public GameObject GetTreePrefab()
    {
        return treePrefab;
    }

    public void AddTree(TreeBehavior treeBehavior, int indexCase)
    {
        treeArray.Add(treeBehavior);
        treeBehavior.treeGameObject = GetTreePrefab();
        treeBehavior.m_manager = this;
        treeBehavior.currentIndexZone = indexCase;
        treeBehavior.currentTransformIndex = zoneDatas[indexCase].currentIndex;
        zoneDatas[indexCase].treeBehaviors[zoneDatas[indexCase].currentIndex] = treeBehavior.transform;
        zoneDatas[indexCase].SetNewCurrentIndex();
    }
    
    public void RemoveTree(TreeBehavior treeBehavior)
    {
        if (!treeArray.Contains(treeBehavior)) return;
        treeArray.Remove(treeBehavior);
        zoneDatas[treeBehavior.currentIndexZone].treeBehaviors[treeBehavior.currentTransformIndex] = null;
        zoneDatas[treeBehavior.currentIndexZone].SetNewCurrentIndex();
    }

    public void AddBurningTree(TreeBehavior treeBehavior)
    {
        if (treeBurningArray.Contains(treeBehavior)) return;

        treeBurningArray.Add(treeBehavior);
        if(treeBurningArray.Count == 4)
        {
            for (int i = 0; i < treeBurningArray.Count; i++)
            {
                treeBurningArray[i].StopBurningSound();
            }
            m_time.ChangeAmbiance(false);
        }
        if (treeBurningArray.Count >= 4)
        {
            treeBurningArray[treeBurningArray.Count-1].StopBurningSound();
        }
    }

    public void RemoveBurning(TreeBehavior treeBehavior)
    {
        treeBurningArray.Remove(treeBehavior);
        if (treeBurningArray.Count == 3)
        {
            for (int i = 0; i < treeBurningArray.Count; i++)
            {
                treeBurningArray[i].StartBurningSound();
            }
            m_time.ChangeAmbiance(true);
        }
    }

    void ClearFire()
    {
        if (treeBurningArray.Count == 0) return;
                
        for (int i = treeBurningArray.Count-1; i >= 0; i--)
        {           
            treeBurningArray[i].isBurning = false;
        }

        treeBurningArray.Clear();
        }

    public void RainOnTree()
    {
        for (int i = 0; i < treeBurningArray.Count; i++)
        {
            treeBurningArray[i].isBurning = false;
        }
    }

    public GameObject GetTreePrefabs()
    {
        int index = UnityEngine.Random.Range(0,treePrefabs.Length);
        return treePrefabs[index];
    }


    private void OnDrawGizmos()
    {
        for (int i = 0; i < zoneDatas.Length; i++)
        {
            Gizmos.color = Color.white;
            if (zoneDatas[i].currentIndex == 5) Gizmos.color = Color.red;
            Gizmos.DrawSphere(zoneDatas[i].center,3);
        }
    }

}

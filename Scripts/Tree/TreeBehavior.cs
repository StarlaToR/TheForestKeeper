using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeBehavior : MonoBehaviour
{
    public enum TreeState
    {
        Child,
        Teen,
        Adult,
        Old,
        Die,
    }

    [Header("Tree Parameters")]
    public TreeState currentTreeState;
    public float maxSize = 10;
    private float treeLifeTime = 60;
    private float treeDeathTime = 3f;
    public float m_lifeTimer;
    public float m_sizeScaleRatio = 0.02f;
    public float immuneTimer = 0f;
    [HideInInspector] public int currentIndexZone;
    [HideInInspector] public int currentTransformIndex;


    [SerializeField] public float[] m_timeByPhase = new float[5];

    [Header("Water Parameters")]
    public float baseWater = 10;
    public float[] limitWater = { 10f, 15f, 30f, 80f };
    public float gainWater = 0.5f;
    public float currentWater = 0;
    public float waterRatio = 0;

    [Header("Infection Parameters")]
    public bool isBurning = false;
    public float infectionRange = 6;
    public float infectionDamage = 2;
    public float timeBeforeInfection = 2.0f;

    public LayerMask treeLayerMask;
    public LayerMask environnementMask;

    [Header("Reproduction Parameters")]
    public float reproductionRange = 6;
    public float rangeNeeded = 2; //Radius of the zone needed for a tree to be created
    public float minRangeEnvironnement = 2; //Radius of the zone needed for a tree to be created
    public float reproductionTimer = 0.0f;
    public float timeBeforeReproduction = 2.0f;
    public GameObject treeGameObject;


    private float m_timerBeforeInfection = 0.0f;

    [SerializeField] private bool m_isDebugActive;
    public int m_phaseIndex;
    private float m_timeAlreadyPast;

    // Temps
    private float m_baseY;
    private GameObject m_model;

    [SerializeField] private GameObject seed;
    private bool seedDroped = false;
    public float seedDropingPercent = 50;

    [SerializeField] private MeshRenderer m_meshRender;

    public ForestManager m_manager;


    private bool m_hasCollide;
    private float m_timeBeforeColliderReset = 0.5f;
    private float m_timerResetCollider;
    private BoxCollider m_collider;

    private TreeRender m_treeRender;
    private bool m_stopGrowth = false;

    [Header("Sounds Parameters")]
    public SoundEventClass treePopEvent;
    public SoundEventClass seedPopEvent;
    public SoundEventClass treeBurningEvent;
    public SoundEventClass treeStartBurningEvent;
    public SoundEventClass treeDieEvent;

    // Start is called before the first frame update
    void Awake()
    {
        m_lifeTimer = 0.0f;
        //Sound 
        for (int i = 0; i < m_phaseIndex; i++)
        {
            m_timeAlreadyPast += m_timeByPhase[i];
            m_lifeTimer += m_timeByPhase[i];
        }

        currentTreeState = (TreeState)m_phaseIndex;
        treePopEvent.InitSound(transform);
        seedPopEvent.InitSound(transform);
        treeDieEvent.InitSound(transform);
        treeBurningEvent.InitSound(transform);
        treeStartBurningEvent.InitSound(transform);

        treePopEvent.PlaySound();

        m_treeRender = GetComponentInChildren<TreeRender>();
        m_collider = GetComponent<BoxCollider>();
        currentWater = baseWater;
        m_baseY = transform.position.y;
        treeLifeTime = 0.0f;

        for (int i = 0; i < m_timeByPhase.Length - 1; i++)
            treeLifeTime += m_timeByPhase[i];

        treeDeathTime = m_timeByPhase[m_timeByPhase.Length - 1];
        m_model = transform.GetChild(0).gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameState.GetState() == GameState.State.PAUSE) return;

        if (immuneTimer > 0.0f)
            immuneTimer -= Time.deltaTime;

        if (m_phaseIndex >= 4)
        {
            DeathUpdate();
            return;
        }

        waterRatio = GetWaterRatio();

        TreeLifeTime();
        if (currentTreeState != TreeState.Die)
        {
            float ratio = m_lifeTimer / treeLifeTime;
            float height = m_baseY + (maxSize * ratio) / 2.0f;

            transform.position = new Vector3(transform.position.x, 0, transform.position.z);
            m_model.transform.localScale = Vector3.one * (1 + maxSize * ratio) * 10 * m_sizeScaleRatio;
        }

        WaterSystem();
        ResetCollisionTree();
    }

    private void TreeLifeTime()
    {
        if (!m_stopGrowth) m_lifeTimer += TimeManager.GetDeltaTime();
        float phaseTimer = m_timeAlreadyPast + m_timeByPhase[m_phaseIndex];

        if (isBurning)
        {
            m_timerBeforeInfection += TimeManager.GetDeltaTime();
            currentWater -= infectionDamage * TimeManager.GetDeltaTime();

            if (m_timerBeforeInfection > timeBeforeInfection)
            {
                m_timerBeforeInfection = 0.0f;
                TestInfection();
            }
        }

        if (m_lifeTimer > phaseTimer * waterRatio)
        {
            m_timeAlreadyPast = m_lifeTimer;
            if (m_phaseIndex == 3)
            {
                m_stopGrowth = true;
            }
            else
            {
                m_phaseIndex++;
            }
            gainWater += 0.5f;


            currentTreeState = (TreeState)m_phaseIndex;
        }

        if (m_phaseIndex == 2 || m_phaseIndex == 3)
        {
            if (!seedDroped && TimeManager.currentPhase == TimeManager.TimePhase.RAIN)
            {
                seedDroped = true;
                if (Random.Range(0, 100) <= seedDropingPercent)
                {
                    GameObject instSeed = Instantiate(seed);
                    float radius = Random.Range(1, rangeNeeded);
                    float angle = Random.Range(0, 360);
                    Vector3 pos = new Vector3(radius * Mathf.Cos(angle), 0.1f, radius * Mathf.Sin(angle));
                    instSeed.transform.SetPositionAndRotation(transform.position + pos, Quaternion.identity);
                }
            }

            reproductionTimer += TimeManager.GetDeltaTime();
            if (reproductionTimer > timeBeforeReproduction)
            {
                reproductionTimer = 0.0f;
                TestReproduction();
            }
        }
    }

    private void WaterSystem()
    {
        if (!m_stopGrowth) currentWater += TimeManager.GetDeltaTime() * gainWater;

        if (currentWater < 0.0f)
        {
            currentTreeState = TreeState.Die;
            StopBurningSound();
            m_phaseIndex = 4;
        }
    }

    public void CollisionTree()
    {
        m_hasCollide = true;
        m_timerResetCollider = 0.0f;
    }

    private void ResetCollisionTree()
    {
        if (!m_hasCollide) return;

        if (m_timerResetCollider > m_timeBeforeColliderReset)
        {
            m_collider.isTrigger = false;
            m_hasCollide = false;
        }
        else
        {
            m_timerResetCollider += Time.deltaTime;
        }
    }

    private bool CanStartFire(Vector3 direction, float distance)
    {
        RaycastHit hit = new RaycastHit();
        if (Physics.Raycast(transform.position, direction, out hit, distance, treeLayerMask))
        {
            return false;
        }
        return true;
    }
    private float GetWaterRatio()
    {
        switch (currentTreeState)
        {
            case TreeState.Child:
                return currentWater / (10 + 0.5f * m_lifeTimer);
            case TreeState.Teen:
                return currentWater / (10 + 0.5f * m_timeByPhase[0] + m_lifeTimer - m_timeByPhase[0]);
            case TreeState.Adult:
                return currentWater / (10 + 0.5f * m_timeByPhase[0] + m_timeByPhase[1] + 1.5f * (m_lifeTimer - m_timeByPhase[0] - m_timeByPhase[1]));
            case TreeState.Old:
                return currentWater / (10 + 0.5f * m_timeByPhase[0] + m_timeByPhase[1] + 1.5f * m_timeByPhase[2] + 2 * (m_lifeTimer - m_timeByPhase[0] - m_timeByPhase[1] - m_timeByPhase[2]));

            default: return 1;
        }
    }

    private void TestInfection()
    {
        // To upgrade the system but valid for simple prototypage

        Collider[] colliders = Physics.OverlapSphere(transform.position, infectionRange, treeLayerMask);
        if (colliders.Length == 0) return;

        for (int i = 0; i < 10; i++)
        {
            int indexTree = Random.Range(0, colliders.Length);
            TreeBehavior targetTree = colliders[indexTree].GetComponent<TreeBehavior>();
            if(targetTree == null)
            {
                continue;
            }
            if (targetTree == this)
            {
                continue;
            }
            if (m_isDebugActive) Debug.Log(this.name + " has infect " + targetTree.name);

            Vector3 direction = transform.position - targetTree.transform.position;

            if (targetTree.immuneTimer > 0f) break;

            if (CanStartFire(direction.normalized, direction.magnitude))
                targetTree.SetBurning();
            break;
        }
    }

    public void SetBurning()
    {
        if (currentTreeState == TreeState.Die) return;
        isBurning = true;
        m_manager.AddBurningTree(GetComponent<TreeBehavior>());
        treeStartBurningEvent.PlaySound();
        StartBurningSound();
    }

    public void StartBurningSound()
    {
        treeBurningEvent.PlaySound();

    }
    public void StopBurningSound()
    {
        treeBurningEvent.StopSoundFadeout();

    }    

    public void OnDrawGizmos()
    {
        if (m_isDebugActive) Gizmos.DrawWireSphere(transform.position, infectionRange);
    }

    public void DestroyTree()
    {
        m_manager.RemoveTree(this);
        if (isBurning) m_manager.RemoveBurning(this);
        Destroy(gameObject);
    }

    public void TestReproduction()
    {
        float spawnRadius = Random.Range(rangeNeeded, reproductionRange);
        float spawnAngle = Random.Range(-180f, 180f);
        Vector3 spawnPoint = transform.position + new Vector3(spawnRadius * Mathf.Cos(spawnAngle), 0, spawnRadius * Mathf.Sin(spawnAngle));
        spawnPoint.y = -2;
        int indexZone = 0;
        if (!m_manager.CanCreateTree(spawnPoint, out indexZone, currentIndexZone)) return;

        Collider[] colliders = Physics.OverlapSphere(spawnPoint, rangeNeeded, treeLayerMask);
        Collider[] colliders2 = Physics.OverlapSphere(spawnPoint, minRangeEnvironnement, environnementMask);


        if (colliders.Length == 0 && colliders2.Length == 0)
        {
            GameObject tree = Instantiate(m_manager.GetTreePrefabs());
            tree.transform.SetPositionAndRotation(spawnPoint, Quaternion.Euler(new Vector3(0, Random.Range(-180f, 180f), 0)));
            m_manager.AddTree(tree.GetComponent<TreeBehavior>(), indexZone);
        }
    }

    void DeathUpdate()
    {
        treeDeathTime -= TimeManager.GetDeltaTime();
        if (treeDeathTime < 3)
        {
            m_treeRender.ActiveWoodFall(Vector3.zero);
        }
        if (treeDeathTime < 0)
        {
            DestroyTree();
        }
    }
}

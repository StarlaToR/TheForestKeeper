using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class CharacterOxygen : MonoBehaviour
{
    [Header("Oxygene Parameters")]
    [SerializeField] private float m_oxygenTimeBeforeDeath = 5.0f;
    [SerializeField] private Transform m_treeLinkTranform;
    [SerializeField] private bool m_isLink;
    [SerializeField] private LayerMask m_treeLayerMask;
    [SerializeField] private float m_rangeOfOxygen;
    [SerializeField] private Color playerNormalColor;
    [SerializeField] private float m_deathDuration = 1.0f;

    [SerializeField] private Animator animator;

    public AnimationCurve animPostProces;
    public Volume m_postProcess;

    [Header("Sounds Parameters")]
    [SerializeField] private SoundEventClass m_playerSuffocateEvent;
    [SerializeField] private SoundEventClass m_playerOxygenTimer;
    [SerializeField] private SoundEventClass m_loseJingle;

    private bool m_deathIsLaunch = false;
    private float m_oxygenTimer;

    public TimeManager timeManager;

    // Start is called before the first frame update
    void Start()
    {
        m_playerOxygenTimer.InitSound();
        m_playerSuffocateEvent.InitSound();
        m_loseJingle.InitSound();

        //m_meshRender = GetComponent<MeshRenderer>();
        //m_treeMaterial = m_meshRender.material;

        //Color color = Color.yellow;
        //m_treeMaterial.SetColor("_BaseColor", color);
        //m_meshRender.material = m_treeMaterial;

        SearchTree();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameState.GetState() == GameState.State.PAUSE || TimeManager.currentPhase == TimeManager.TimePhase.SLEEP) return;

        if (m_isLink)
        {
            if (IsInRange())
            {
                if (m_oxygenTimer > 0.0f)
                {
                    float ratio = m_oxygenTimer / m_oxygenTimeBeforeDeath;
                    m_postProcess.weight = animPostProces.Evaluate(ratio);
                    m_oxygenTimer -= Time.deltaTime;
                }

                }
                else
            {
                m_playerOxygenTimer.PlaySound();
                SearchTree();
            }
        }


        if (!m_isLink)
        {
            if (m_oxygenTimer > m_oxygenTimeBeforeDeath && !m_deathIsLaunch)
            {
                m_deathIsLaunch = true;
                m_playerSuffocateEvent.PlaySound();
                m_playerOxygenTimer.StopSoundDirect();
                GetComponent<CharacterMouvements>().enabled = false;
                StartCoroutine(DeathEvent());
            }
            else
            {
                float ratio = m_oxygenTimer / m_oxygenTimeBeforeDeath;
                m_postProcess.weight = animPostProces.Evaluate(ratio);
                SearchTree();
                m_oxygenTimer += Time.deltaTime;
            }
        }

        //Color color = Color.Lerp(Color.blue, Color.yellow, m_oxygenTimer / m_oxygenTimeBeforeDeath);
        //m_treeMaterial.SetColor("_BaseColor", color);
        //m_meshRender.material = m_treeMaterial;
    }

    private bool IsInRange()
    {
        if (m_treeLinkTranform == null)
            return false;

        TreeBehavior tree = m_treeLinkTranform.GetComponent<TreeBehavior>();
        if (tree.currentTreeState == TreeBehavior.TreeState.Die)
        {
            return false;
        }

        if (tree.isBurning) return false;

        if (Vector3.Distance(transform.position, m_treeLinkTranform.position) > m_rangeOfOxygen)
        {
            return false;
        }

        return true;
    }

    private void SearchTree()
    {
        m_isLink = false;
        Collider[] collidersTrees = Physics.OverlapSphere(transform.position, m_rangeOfOxygen, m_treeLayerMask);
        if (collidersTrees.Length == 0) return;

        float range = m_rangeOfOxygen;
        int indexTree = 0;
        for (int i = 0; i < collidersTrees.Length; i++)
        {
            float distance = Vector3.Distance(transform.position, collidersTrees[i].transform.position);
            TreeBehavior tree = collidersTrees[i].GetComponent<TreeBehavior>();
            if (tree.currentTreeState == TreeBehavior.TreeState.Die)
            {
                continue;
            }

            if (tree.isBurning) continue;
            if (distance < range)
            {
                range = distance;
                indexTree = i;
            }
        }

        m_treeLinkTranform = collidersTrees[indexTree].transform;
        m_isLink = true; ;
        //m_treeMaterial.SetColor("_BaseColor", playerNormalColor);
        //m_meshRender.material = m_treeMaterial;
        m_playerOxygenTimer.StopSoundFadeout();
    }

    IEnumerator DeathEvent()
    {
        m_loseJingle.PlaySound();
        animator.SetBool("die", true);
        yield return new WaitForSeconds(m_deathDuration);
        m_loseJingle.StopSoundFadeout();
        m_playerSuffocateEvent.StopSoundDirect();
        timeManager.StopMusic();
        TimeManager.currentPhase = TimeManager.TimePhase.START;
        timeManager.m_phaseIndex = 0;
        GameState.ChangeScene(SceneManager.GetActiveScene().buildIndex);

    }

    public float GetPercentOxygen()
    {
        return (m_oxygenTimeBeforeDeath - m_oxygenTimer) / m_oxygenTimeBeforeDeath;
    }
}

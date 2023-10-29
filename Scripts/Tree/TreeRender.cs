using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeRender : MonoBehaviour
{
    [Header("Tree Parameters")]
    [SerializeField] private GameObject[] m_leafArray = new GameObject[0];
    [SerializeField] private GameObject[] m_woodArray = new GameObject[0];
    private MeshRenderer[] meshRenderers = new MeshRenderer[0];
    [SerializeField] private bool m_activeDestruction;

    private TreeBehavior m_treeBehaviorComponent;

    private float m_leafGrowTime;
    private float m_leafGrowTimer;

    private float m_leafReduceTime;
    private float m_leafReduceTimer;

    private float m_gradientTime;
    private float m_gradientTimer;
    public Gradient m_autumnGradient;

    private bool m_isGradientTransitionOccuring;
    private int m_indexGradient;
    public Gradient[] m_gradientPhase = new Gradient[0];



    private void InitComponent()
    {
        m_treeBehaviorComponent = GetComponentInParent<TreeBehavior>();
        m_leafGrowTime = m_treeBehaviorComponent.m_timeByPhase[1] + m_treeBehaviorComponent.m_timeByPhase[2];
        m_gradientTime = m_treeBehaviorComponent.m_timeByPhase[2];
        m_leafReduceTime = m_treeBehaviorComponent.m_timeByPhase[4] -3;
        m_leafReduceTimer = m_leafReduceTime;
        m_leafGrowTimer=  m_treeBehaviorComponent.m_lifeTimer - m_treeBehaviorComponent.m_timeByPhase[0];
        if (m_leafGrowTimer < 0) m_leafGrowTimer = 0;
        meshRenderers = new MeshRenderer[m_leafArray.Length];
        float ratio = m_leafGrowTimer / m_leafGrowTime;
        for (int i = 0; i < m_leafArray.Length; i++)
        {
            m_leafArray[i].transform.localScale = Vector3.one * ratio;
            meshRenderers[i] = m_leafArray[i].GetComponent<MeshRenderer>();
       
        }
    }

    public void Start()
    {
        InitComponent();
    }
    // TODO : Scale down leaf when old to die
    public void Update()
    {
        if (m_treeBehaviorComponent.currentTreeState == TreeBehavior.TreeState.Teen || m_treeBehaviorComponent.currentTreeState == TreeBehavior.TreeState.Adult)
        {
            GrowLeaf();

        }

        if (m_treeBehaviorComponent.currentTreeState == TreeBehavior.TreeState.Die)
        {
            ReduceLeaf();
        }
        
        if (m_isGradientTransitionOccuring)
        {
            TransitionGradient();
        }
    }

    private void GrowLeaf()
    {
        m_leafGrowTimer += TimeManager.GetDeltaTime();
        float ratio = m_leafGrowTimer / m_leafGrowTime;
        for (int i = 0; i < m_leafArray.Length; i++)
        {
            m_leafArray[i].transform.localScale = Vector3.one * ratio;
        }
    }

    private void ReduceLeaf()
    {
        m_leafReduceTimer -= TimeManager.GetDeltaTime();
        float ratio = m_leafReduceTimer / m_leafReduceTime;
        if (ratio < 0) ratio = 0;
        for (int i = 0; i < m_leafArray.Length; i++)
        {
            m_leafArray[i].transform.localScale = Vector3.one * ratio;
        }
    }

    private void ChangeColor()
    {
        m_gradientTimer += TimeManager.GetDeltaTime();
        float ratio = m_gradientTimer / m_gradientTime;
        for (int i = 0; i < meshRenderers.Length; i++)
        {
            meshRenderers[i].material.SetColor("_Leaves_Color", m_autumnGradient.Evaluate(ratio));
            meshRenderers[i].material.SetColor("_Fresnel_Color", m_autumnGradient.Evaluate(ratio));
        }
    }

    private void TransitionGradient()
    {
        m_gradientTimer += TimeManager.GetDeltaTime();
        float ratio = m_gradientTimer / m_gradientTime;
        for (int i = 0; i < meshRenderers.Length; i++)
        {
            meshRenderers[i].material.SetColor("_Leaves_Color", m_gradientPhase[m_indexGradient].Evaluate(ratio));
            meshRenderers[i].material.SetColor("_Fresnel_Color", m_gradientPhase[m_indexGradient].Evaluate(ratio));
        }
        if (m_gradientTimer > m_gradientTime)
        {
            m_isGradientTransitionOccuring = false;
        }

    }

    public void ActiveGradient(float time, int index)
    {
        m_isGradientTransitionOccuring = true;
        m_gradientTime = time;
        m_gradientTimer = 0.0f;
    }


    public void ActiveWoodFall(Vector3 forceDirection)
    {
        if (!m_activeDestruction) return;

        for (int i = 0; i < m_woodArray.Length; i++)
        {
            m_woodArray[i].GetComponent<MeshCollider>().enabled = true;
            Rigidbody rigid = m_woodArray[i].GetComponent<Rigidbody>();
            rigid.isKinematic = false;
            rigid.AddForce(forceDirection, ForceMode.Impulse);
        }
    }

}

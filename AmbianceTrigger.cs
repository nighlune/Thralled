using UnityEngine;
using System.Collections;

public class AmbianceTrigger : MonoBehaviour
{
    public Globals.Ambiance m_TransitionToAmbiance;
    public float m_FadeOutAmbianceTime;
    public float m_FadeInAmbianceTime;
    public bool m_Delay;
    public float m_DelayTime;

    private bool m_TriggerOnce;
    private bool m_IsPlaying;
    private float m_DelayTimer;

    // Use this for initialization
    void Start()
    {
        m_TriggerOnce = false;
        m_IsPlaying = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_TriggerOnce && !m_IsPlaying)
        {
            

            if (m_Delay)
            {
                m_DelayTimer += Time.deltaTime;

                if (m_DelayTimer >= m_DelayTime)
                {
                    m_IsPlaying = true;
                    m_DelayTimer = 0.0f;
                    AudioManager.Instance.StartAmbianceTransition(m_TransitionToAmbiance, m_FadeOutAmbianceTime, m_FadeInAmbianceTime);
                }
            }

            else
            {
                m_IsPlaying = true;
                AudioManager.Instance.StartAmbianceTransition(m_TransitionToAmbiance, m_FadeOutAmbianceTime, m_FadeInAmbianceTime);
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" && !m_TriggerOnce)
        {
            m_TriggerOnce = true;            
        }
    }
}

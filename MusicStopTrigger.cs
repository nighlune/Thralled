using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MusicStopTrigger : MonoBehaviour
{
    public List<GameObject> m_AudioFilePrefabs;
    public bool m_FadeOut;
    public float m_FadeOutTime;

    private bool m_TriggerOnce;
    private bool m_Fading;
    private bool m_IsStopping;
    private float m_Timer;

	// Use this for initialization
	void Start()
    {
        m_TriggerOnce = false;
        m_Fading = false;
        m_IsStopping = false;
        m_Timer = 0.0f;
    }
	
	// Update is called once per frame
    void Update()
    {
        if (m_TriggerOnce && !m_IsStopping)
        {
            m_IsStopping = true;

            for (int i = 0; i < m_AudioFilePrefabs.Count; ++i)
            {
                if (m_AudioFilePrefabs[i] != null && AudioManager.Instance.IsPlaying(m_AudioFilePrefabs[i].name))
                {
                    if (m_FadeOut)
                    {
                        AudioManager.Instance.FadeOutToStop(m_AudioFilePrefabs[i].name, m_FadeOutTime);
                    }

                    else
                    {
                        AudioManager.Instance.Stop(m_AudioFilePrefabs[i].name);
                    }
                }
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

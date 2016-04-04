using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MusicFadeTrigger : MonoBehaviour
{
    public List<GameObject> m_AudioFilePrefabs;
    public float m_FadeToVolume;
    public float m_FadeInterval;

    private bool m_TriggerOnce;

	// Use this for initialization
	void Start()
    {
        m_TriggerOnce = false;
	}

    // Update is called once per frame
    void Update()
    {
	
	}

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" && !m_TriggerOnce)
        {
            m_TriggerOnce = true;

            for (int i = 0; i < m_AudioFilePrefabs.Count; ++i)
            {
                if (m_AudioFilePrefabs[i] != null)
                {
                    if (!AudioManager.Instance.IsPlaying(m_AudioFilePrefabs[i].name))
                    {
                        //print(m_AudioFilePrefabs[i].name + " isn't playing!");
                    }

                    else
                    {
                        AudioManager.Instance.FadeOut(m_AudioFilePrefabs[i].name, m_FadeInterval, m_FadeToVolume);
                    }
                }
            }
        }
    }
}

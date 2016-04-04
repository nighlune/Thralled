using UnityEngine;
using System.Collections;

public class SFXStartTrigger : MonoBehaviour
{
    public GameObject m_AudioFilePrefab;
    public bool m_Loop;
    public bool m_OverrideVolume;
    public float m_VolumeToOverride;
    public bool m_FadeInToVolume;
    public float m_VolumeToFadeTo;
    public bool m_ResetVolumeOnFadeIn;
    public bool m_FadeIn;
    public float m_FadeInInterval;

    private bool m_TriggerOnce;
    private bool m_IsPlaying;

	// Use this for initialization
	void Start()
    {
        m_TriggerOnce = false;
        m_IsPlaying = false;
    }
	
	// Update is called once per frame
	void Update()
    {
        if (m_TriggerOnce && !m_IsPlaying && m_AudioFilePrefab != null)
        {
            m_IsPlaying = true;
            
            AudioManager.Instance.Load(m_AudioFilePrefab.name);

            if (m_ResetVolumeOnFadeIn && !m_OverrideVolume)
            {
                AudioManager.Instance.ResetVolume(m_AudioFilePrefab.name);
            }
            if (m_OverrideVolume && !m_ResetVolumeOnFadeIn)
            {
                AudioManager.Instance.SetVolume(m_AudioFilePrefab.name, m_VolumeToOverride);
            }
            if (m_Loop)
            {
                AudioManager.Instance.LoopPlay(m_AudioFilePrefab.name);
            }
            else
            {
                AudioManager.Instance.Play(m_AudioFilePrefab.name);
            }
            if(m_OverrideVolume && m_FadeIn)
            {
                AudioManager.Instance.FadeIn(m_AudioFilePrefab.name, m_FadeInInterval);
            }
            else if(m_OverrideVolume && m_FadeInToVolume)
            {
				AudioManager.Instance.FadeInToVolume(m_AudioFilePrefab.name, m_FadeInInterval, m_VolumeToFadeTo);
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

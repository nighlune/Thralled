using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MusicStartTrigger : MonoBehaviour
{
    // Public variables
    public List<GameObject> m_AudioFilePrefabs;
    public bool m_Loop;
    public bool m_OverrideVolume;
    public float m_Volume;
    public bool m_FadeIn;
    public bool m_ResetVolumeOnFadeIn;
    public float m_FadeInInterval;

    // Private variables
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
        if (m_TriggerOnce && !m_IsPlaying)
        {
            m_IsPlaying = true;

            // Load the AudioFile.
            for (int i = 0; i < m_AudioFilePrefabs.Count; ++i)
            {
                if (m_AudioFilePrefabs[i] != null)
                {
                    AudioManager.Instance.Load(m_AudioFilePrefabs[i].name);

                    if (m_OverrideVolume)
                    {
                        AudioManager.Instance.SetVolume(m_AudioFilePrefabs[i].name, m_Volume);
                    }
                }
            }

            // Reset the volume, so the fade in is from zero volume.
            if (m_ResetVolumeOnFadeIn)
            {
                for (int i = 0; i < m_AudioFilePrefabs.Count; ++i)
                {
                    if (m_AudioFilePrefabs[i] != null)
                    {
                        AudioManager.Instance.SetVolume(m_AudioFilePrefabs[i].name, 0.0f);
                    }
                }
            }

            // If the AudioManager's current music file hasn't been initialized or isn't playing.
            if (AudioManager.Instance.currentMusicPrefab == null || !AudioManager.Instance.IsPlaying(AudioManager.Instance.currentMusicPrefab.name))
            {
                // Initialize the current music file.
                AudioManager.Instance.currentMusicPrefab = m_AudioFilePrefabs[0];

                for (int i = 0; i < m_AudioFilePrefabs.Count; ++i)
                {
                    if (m_AudioFilePrefabs[i] != null)
                    {
                        if (m_Loop)
                        {
                            AudioManager.Instance.Loop(m_AudioFilePrefabs[i].name);
                        }
                        if (m_FadeIn)
                        {
                            AudioManager.Instance.Play(m_AudioFilePrefabs[i].name);
                            AudioManager.Instance.FadeIn(m_AudioFilePrefabs[i].name, m_FadeInInterval);
                        }
                        else
                        {
                            AudioManager.Instance.Play(m_AudioFilePrefabs[i].name);
                        }
                    }
                }
            }

            // If the AudioManager's current music file has been initialized and is playing.
            else
            {
                for (int i = 0; i < m_AudioFilePrefabs.Count; ++i)
                {
                    if (m_AudioFilePrefabs[i] != null)
                    {
                        if (m_Loop)
                        {
                            AudioManager.Instance.Loop(m_AudioFilePrefabs[i].name);
                        }
                        if (m_FadeIn)
                        {
                            AudioManager.Instance.PlayFromTime(m_AudioFilePrefabs[i].name,
                                AudioManager.Instance.GetTime(AudioManager.Instance.currentMusicPrefab.name));
                            AudioManager.Instance.FadeIn(m_AudioFilePrefabs[i].name, m_FadeInInterval);
                        }
                        else
                        {
                            AudioManager.Instance.PlayFromTime(m_AudioFilePrefabs[i].name,
                                AudioManager.Instance.GetTime(AudioManager.Instance.currentMusicPrefab.name));
                        }                        
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

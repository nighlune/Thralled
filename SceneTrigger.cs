using UnityEngine;
using System.Collections;

public class SceneTrigger : MonoBehaviour
{
    public bool m_UnloadSounds;
    public bool m_ResetAllVolumes;
    public bool m_StopMusic;
    public bool m_StopSounds;
    public bool m_FadeOutMusic;
    public bool m_FadeOutSounds;
    public float m_FadeInterval;

    private bool m_TriggerOnce;
    private bool m_TriggerMusicFade;
    private bool m_TriggerSoundFade;
    private float m_MusicFadeTimer;
    private float m_SoundFadeTimer;
    
	// Use this for initialization
    void Start()
    {
        m_TriggerOnce = false;
        m_TriggerMusicFade = false;
        m_TriggerSoundFade = false;
        m_MusicFadeTimer = 0.0f;
        m_SoundFadeTimer = 0.0f;
    }
	
	// Update is called once per frame
	void Update()
    {
        if (m_TriggerMusicFade)
        {
            m_MusicFadeTimer += Time.deltaTime;

            if (m_MusicFadeTimer >= m_FadeInterval)
            {
                m_TriggerMusicFade = false;
                AudioManager.Instance.StopMusic();
            }
        }

        if (m_TriggerSoundFade)
        {
            m_SoundFadeTimer += Time.deltaTime;

            if (m_SoundFadeTimer >= m_FadeInterval)
            {
                m_TriggerSoundFade = false;
                AudioManager.Instance.StopSounds();
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (AudioManager.Instance != null && other.gameObject.tag == "Player" && !m_TriggerOnce)
        {
            m_TriggerOnce = true;

            if (m_ResetAllVolumes)
            {
                AudioManager.Instance.ResetAllVolumes();
            }

            if (m_StopMusic)
            {
                AudioManager.Instance.StopMusic();
            }

            if (m_StopSounds)
            {
                AudioManager.Instance.StopSounds();
            }

            if (m_FadeOutMusic)
            {
                m_TriggerMusicFade = true;
                AudioManager.Instance.FadeOutMusic(m_FadeInterval, 0.0f);
            }

            if (m_FadeOutSounds)
            {
                m_TriggerSoundFade = true;
                AudioManager.Instance.FadeOutSounds(m_FadeInterval, 0.0f);
            }

            if (m_UnloadSounds)
            {
                Globals.CURRENT_AMBIANCE = Globals.Ambiance.NONE;
                AudioManager.Instance.ChangeAmbiance();
                AudioManager.Instance.UnloadSounds();
            }
        }
    }
}

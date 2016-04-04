using UnityEngine;
using System.Collections;

public class SFXStopTrigger : MonoBehaviour
{
    public GameObject m_AudioFilePrefab;
    public bool m_FadeOut;
    public float m_FadeOutInterval;

    private bool m_TriggerOnce;

	// Use this for initialization
	void Start() { m_TriggerOnce = false; }
	
	// Update is called once per frame
	void Update() {}

    void OnTriggerEnter(Collider other)
    {
        //print(AudioManager.Instance.IsPlaying(m_AudioFilePrefab.name));

        if (other.gameObject.tag == "Player" && !m_TriggerOnce)
        {
            m_TriggerOnce = true;

            if (AudioManager.Instance.IsPlaying(m_AudioFilePrefab.name))
            {
                if (m_FadeOut)
                {
                    AudioManager.Instance.FadeOutToStop(m_AudioFilePrefab.name, m_FadeOutInterval);
                }

                else
                {
                    AudioManager.Instance.Stop(m_AudioFilePrefab.name);
                }
            }            
        }
    }
}

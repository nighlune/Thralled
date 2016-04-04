using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AudioFile : MonoBehaviour
{
    private List<AudioSource> m_AudioSources;
    private List<AudioClip> m_AudioClips;

    private float m_MinFadeOutVolume;
    private float m_FadeToVolume;
    private float m_TimeInterval;
	private float m_currentVolume;

    private bool m_FadeIn;
    private bool m_FadeInLayer;
    private bool m_FadeOut;
    private bool m_FadeOutLayer;
    private bool m_FadeEnabled;
    private bool m_FadeToStop;

    // Public variables
    public AudioType m_Type;

    public List<string> m_FilePaths;    
    
    public float m_InitialVolume;
    public float m_DefaultVolume;

	// Use this for initialization
	void Awake()
    {
        m_AudioSources = new List<AudioSource>();
        m_AudioClips = new List<AudioClip>();

        m_MinFadeOutVolume = 0.0f;
        m_FadeToVolume = m_DefaultVolume;
        m_TimeInterval = 0.0f;

        m_FadeIn = false;
        m_FadeOut = false;
        m_FadeEnabled = true;
        m_FadeToStop = false;

        for (int i = 0; i < m_FilePaths.Count; i++)
        {
            AudioClip tempClip = (AudioClip)Resources.Load(m_FilePaths[i]);
            AudioSource tempSource = (AudioSource)gameObject.AddComponent("AudioSource");
            //tempSource.minDistance = 500;
            tempSource.clip = tempClip;
            tempSource.volume = m_InitialVolume;

            GameObject tempAudioManager = GameObject.Find("AudioManager");
            if (tempAudioManager != null)
            {
                tempSource.transform.parent = tempAudioManager.transform;
            }

            m_AudioClips.Add(tempClip);
            m_AudioSources.Add(tempSource);
        }
	}

    // Update is called once per frame
    void Update()
    {
        // Update the AudioManager's position
        if (Camera.mainCamera != null)
        {
            transform.position = Camera.mainCamera.transform.position;
        }

        // If the sound is set to fade in
        if (m_FadeIn)
        {
            for (int i = 0; i < m_AudioSources.Count; ++i)
            {
                if (m_AudioSources[i].volume < m_FadeToVolume)
                {
                    float currentVolume = m_AudioSources[i].volume + (Time.deltaTime / (m_TimeInterval + 1)) * (m_FadeToVolume - m_currentVolume);
                    m_AudioSources[i].volume = currentVolume;
                }

                else
                {
                    m_FadeIn = false;

                    for (int j = 0; j < m_AudioSources.Count; ++j)
                    {
                        m_AudioSources[j].volume = m_FadeToVolume;
                    }

                    break;
                }
            }
        }

        // If the sound is set to fade out
        if (m_FadeOut)
        {
            for (int i = 0; i < m_AudioSources.Count; ++i)
            {
                if (m_AudioSources[i].volume > m_MinFadeOutVolume)
                {
                    float currentVolume = m_AudioSources[i].volume - (Time.deltaTime / (m_TimeInterval + 1)) * (-m_MinFadeOutVolume + m_currentVolume);
                    m_AudioSources[i].volume = currentVolume;
                }

                else
                {
                    m_FadeOut = false;

                    for (int j = 0; j < m_AudioSources.Count; ++j)
                    {
                        m_AudioSources[j].volume = m_MinFadeOutVolume;

                        if (m_FadeToStop)
                        {
                            m_AudioSources[j].Stop();
                        }
                    }

                    m_FadeToStop = false;

                    break;
                }
            }
        }
	}

    // Returns the AudioType.
    public AudioType GetType()
    {
        return m_Type;
    }

    // Returns the number of AudioSources
    public int GetNumOfSources()
    {
        return m_AudioSources.Count;
    }

    // Returns the list of AudioSources
    public List<AudioSource> GetSources()
    {
        return m_AudioSources;
    }

    // Functions related to Volume.
    #region Volume

    // Returns the current volume of the AudioSource.
    public float GetVolume()
    {
        return m_AudioSources[0].volume;
    }    

    // Sets the AudioSource's current volume.
    public void SetVolume(float volume)
    {
        for (int i = 0; i < m_AudioSources.Count; ++i)
        {
            if (volume <= m_DefaultVolume)
            {
                m_AudioSources[i].volume = volume;
            }
        }
    }

    // Returns a specific layer's volume.
    public float GetLayerVolume(int layer)
    {
        layer = layer - 1;

        if (layer > m_AudioSources.Count)
        {
            return 2.0f;
        }

        else
        {
            return m_AudioSources[layer].volume;
        }
    }

    // Sets the AudioSource's specific layer's current volume.
    public void SetLayerVolume(int layer, float volume)
    {
        layer = layer - 1;

        if (layer > m_AudioSources.Count)
        {
            // ERROR!
        }

        else
        {
            if (volume <= m_DefaultVolume)
            {
                m_AudioSources[layer].volume = volume;
            }
        }
    }

    // Returns the default volume that was passed in by
    // the Initialize function.
    public float GetDefaultVolume()
    {
        return m_DefaultVolume;
    }

    // Sets the default volume for the AudioSource.
    public void SetDefaultVolume(float volume)
    {
        m_DefaultVolume = volume;
        m_FadeToVolume = m_DefaultVolume;
    }

    // Resets the volume to the sound's default volume that was
    // passed in by the Initialize function.
    public void ResetVolume()
    {
        for (int i = 0; i < m_AudioSources.Count; ++i)
        {
            m_AudioSources[i].volume = m_DefaultVolume;
        }
    }

    #endregion

    // Functions related to Sound Playback.
    #region Playback

    // Sets the AudioSource's internal mute flag to true.
    public void Mute()
    {
        for (int i = 0; i < m_AudioSources.Count; ++i)
        {
            if (m_AudioSources[i].mute == false)
            {
                m_AudioSources[i].mute = true;
            }
        }
    }

    // Sets the AudioSource's internal mute flag to false.
    public void UnMute()
    {
        for (int i = 0; i < m_AudioSources.Count; ++i)
        {
            if (m_AudioSources[i].mute)
            {
                m_AudioSources[i].mute = false;
            }
        }
    }

    // Returns the current mute value of the AudioSource.
    public bool IsMuted()
    {
        return m_AudioSources[0].mute;
    }

    // Pauses the AudioSource.
    public void Pause()
    {
        for (int i = 0; i < m_AudioSources.Count; ++i)
        {
            if (m_AudioSources[i].isPlaying)
            {
                print("pausing: " + m_AudioSources[i].name);

                m_AudioSources[i].Pause();
            }
        }
    }

    // Stops the AudioSource from playing.
    public void Stop()
    {
        for (int i = 0; i < m_AudioSources.Count; ++i)
        {
            if (m_AudioSources[i].isPlaying)
            {
                m_AudioSources[i].Stop();
            }

            if (m_AudioSources[i].loop)
            {
                m_AudioSources[i].loop = false;
            }
        }
    }

    // Stops the specific layer.
    public void StopLayer(int layer)
    {
        if (m_AudioSources[layer].isPlaying)
        {
            m_AudioSources[layer].Stop();
        }

        if (m_AudioSources[layer].loop)
        {
            m_AudioSources[layer].loop = false;
        }
    }

    // Plays the AudioSource.
    public void Play()
    {
        if (m_AudioSources.Count == 1)
        {
            if (m_AudioSources[0].isPlaying)
            {
                m_AudioSources[0].Stop();
            }

            m_AudioSources[0].Play();
        }

        else
        {
            int soundToPlay = Random.Range(0, m_AudioSources.Count);

            if (m_AudioSources[soundToPlay].isPlaying)
            {
                m_AudioSources[soundToPlay].Stop();
            }

            m_AudioSources[soundToPlay].Play();
        }
    }

    // Plays the AudioSource at a specific volume.
    public void Play(float volume)
    {
        SetVolume(volume);

        Play();
    }

    // Plays a particular AudioSource layer.
    public void PlayLayer(int layer)
    {
        if (layer > m_AudioSources.Count)
        {
            // ERROR!
        }

        else
        {
            if (m_AudioSources[layer].isPlaying)
            {
                m_AudioSources[layer].Stop();
            }

            m_AudioSources[layer].Play();
        }
    }

    // Plays a particular AudioSource layer at a specific volume.
    public void PlayLayer(int layer, float volume)
    {
        SetLayerVolume(layer, volume);

        PlayLayer(layer);
    }

    // Plays the AudioSource from a specific time.
    public void PlayFromTime(float time)
    {
        if (m_AudioSources.Count == 1)
        {
            if (m_AudioSources[0].isPlaying)
            {
                m_AudioSources[0].Stop();
            }

            m_AudioSources[0].time = time;
            m_AudioSources[0].Play();
        }

        else
        {
            int soundToPlay = Random.Range(0, m_AudioSources.Count);

            if (m_AudioSources[soundToPlay].isPlaying)
            {
                m_AudioSources[soundToPlay].Stop();
            }

            m_AudioSources[soundToPlay].time = time;
            m_AudioSources[soundToPlay].Play();
        }
    }

    // Plays the AudioSource from a specific time from a specific volume.
    public void PlayFromTime(float time, float volume)
    {
        SetVolume(volume);

        PlayFromTime(time);
    }

    // Plays a particular layer from a specific time.
    public void PlayLayerFromTime(int layer, float time)
    {
        if (layer > m_AudioSources.Count)
        {
            // ERROR!
        }

        else
        {
            if (m_AudioSources[layer].isPlaying)
            {
                m_AudioSources[layer].Stop();
            }

            m_AudioSources[layer].time = time;
            m_AudioSources[layer].Play();
        }
    }

    // Players a particular layer from a specific time from a specific volume.
    public void PlayLayerFromTime(int layer, float time, float volume)
    {
        SetLayerVolume(layer, volume);

        PlayLayerFromTime(layer, time);
    }

    // Returns whether the AudioSource(s) are playing.
    public List<bool> ArePlaying()
    {
        List<bool> isPlaying = new List<bool>(m_AudioSources.Count);

        for (int i = 0; i < m_AudioSources.Count; ++i)
        {
            isPlaying[i] = m_AudioSources[i].isPlaying;
        }

        return isPlaying;
    }
    
    // Returns whether the AudioSource(s) are playing
    public bool IsPlaying()
    {
        for (int i = 0; i < m_AudioSources.Count; ++i)
        {
            if (m_AudioSources[i].isPlaying)
            {
                return true;
            }
        }

        return false;
    }

    // Returns whether a specific layer is playing.
    public bool IsLayerPlaying(int layer)
    {
        if (layer > m_AudioSources.Count)
        {
            return false; // ERROR!
        }

        else
        {
            return m_AudioSources[layer].isPlaying;
        }
    }

    // Plays the AudioSource, looping the sound.
    public void LoopPlay()
    {
        if (m_AudioSources.Count == 1)
        {
            if (m_AudioSources[0].isPlaying)
            {
                m_AudioSources[0].Stop();
            }

            m_AudioSources[0].loop = true;
            m_AudioSources[0].Play();
        }

        else
        {
            int soundToPlay = Random.Range(0, m_AudioSources.Count);

            if (m_AudioSources[soundToPlay].isPlaying)
            {
                m_AudioSources[soundToPlay].Stop();
            }

            m_AudioSources[soundToPlay].loop = true;
            m_AudioSources[soundToPlay].Play();
        }
    }

    // Plays the AudioSource at a specific volume, looping the sound.
    public void LoopPlay(float volume)
    {
        SetVolume(volume);

        LoopPlay();
    }

    // Plays the AudioSource from a specific time, looping the sound.
    public void LoopPlayFromTime(float time)
    {
        if (m_AudioSources.Count == 1)
        {
            if (m_AudioSources[0].isPlaying)
            {
                m_AudioSources[0].Stop();
            }

            m_AudioSources[0].loop = true;
            m_AudioSources[0].time = time;
            m_AudioSources[0].Play();
        }

        else
        {
            int soundToPlay = Random.Range(0, m_AudioSources.Count);

            if (m_AudioSources[soundToPlay].isPlaying)
            {
                m_AudioSources[soundToPlay].Stop();
            }

            m_AudioSources[soundToPlay].loop = true;
            m_AudioSources[soundToPlay].time = time;
            m_AudioSources[soundToPlay].Play();
        }
    }

    // Plays the AudioSource from a specific time and volume, looping the sound.
    public void LoopPlayFromTime(float time, float volume)
    {
        SetVolume(volume);

        LoopPlayFromTime(time);
    }

    // Plays a specific layer, looping the sound.
    public void LoopPlayLayer(int layer)
    {
        if (layer > m_AudioSources.Count)
        {
            // ERROR!
        }

        else
        {
            if (m_AudioSources[layer].isPlaying)
            {
                m_AudioSources[layer].Stop();
            }

            m_AudioSources[layer].loop = true;
            m_AudioSources[layer].Play();
        }
    }

    // Plays a specific layer at a specific volume, looping the sound.
    public void LoopPlayLayer(int layer, float volume)
    {
        SetLayerVolume(layer, volume);

        LoopPlayLayer(layer);
    }
    
    // Plays a specific layer from a specific time.
    public void LoopPlayLayerFromTime(int layer, float time)
    {
        if (layer > m_AudioSources.Count)
        {
            // ERROR!
        }

        else
        {
            if (m_AudioSources[layer].isPlaying)
            {
                m_AudioSources[layer].Stop();
            }

            m_AudioSources[layer].loop = true;
            m_AudioSources[layer].time = time;
            m_AudioSources[layer].Play();
        }
    }

    // Plays a specific layer from a specific time at a specific volume.
    public void LoopPlayLayerFromTime(int layer, float time, float volume)
    {
        SetLayerVolume(layer, volume);

        LoopPlayLayerFromTime(layer, time);
    }

    // Sets looping to true.
    public void Loop()
    {
        for (int i = 0; i < m_AudioSources.Count; ++i)
        {
            m_AudioSources[i].loop = true;
        }
    }

    // Sets a layer to loop.
    public void LoopLayer(int layer)
    {
        if (layer > m_AudioSources.Count)
        {
            // ERROR!
        }

        else
        {
            m_AudioSources[layer].loop = true;
        }
    }

    // Sets looping to false.
    public void StopLooping()
    {
        for (int i = 0; i < m_AudioSources.Count; ++i)
        {
            m_AudioSources[i].loop = false;
        }
    }

    // Sets a layer to stop looping.
    public void StopLoopingLayer(int layer)
    {
        m_AudioSources[layer].loop = false;
    }

    // Returns the time that the AudioSource(s) are currently playing at.
    public List<float> GetTimes()
    {
        List<float> tempList = new List<float>(m_AudioSources.Count);

        for (int i = 0; i < m_AudioSources.Count; ++i)
        {
            tempList[i] = m_AudioSources[i].time;
        }

        return tempList;
    }
    
    // Return the time of the first AudioSource that's playing.
    public float GetTime()
    {
        for (int i = 0; i < m_AudioSources.Count; ++i)
        {
            if (m_AudioSources[i].isPlaying)
            {
                return m_AudioSources[i].time;
            }
        }

        return 0.0f;
    }

    // Returns the time that a specific layer is currently playing at.
    public float GetLayerTime(int layer)
    {
        return m_AudioSources[layer].time;
    }

    public float GetRuntime()
    {
        return m_AudioSources[0].clip.length;
    }

    #endregion

    // Functions related to Fading
    #region Fading

    // Enables fading.
    public void EnableFading()
    {
        m_FadeEnabled = true;
    }

    // Disables fading.
    public void DisableFading()
    {
        m_FadeEnabled = false;
    }

    // Returns whether fading is enabled.
    public bool IsFadingEnabled()
    {
        return m_FadeEnabled;
    }

    // Fades in a sound over a specific time interval (in seconds).
    public void FadeIn(float timeInterval)
    {
        if (m_FadeEnabled)
        {
            m_TimeInterval = timeInterval;
            m_FadeOut = false;
            m_FadeIn = true;
			m_FadeToVolume = m_DefaultVolume;
			m_currentVolume = m_AudioSources[0].volume;
        }
    }

    // Fades in a sound over a specific time interval (in seconds) to a specific volume.
    public void FadeInToVolume(float timeInterval, float volume)
    {
        if (m_FadeEnabled)
        {
            m_FadeToVolume = volume;
            m_TimeInterval = timeInterval;
            m_FadeOut = false;
            m_FadeIn = true;
			m_currentVolume = m_AudioSources[0].volume;
        }
    }

    // Fades out a sound over a specific time interval (in seconds), to a default
    // minimum volume of zero.
    public void FadeOut(float timeInterval, float minVolume = 0)
    {
        if (m_FadeEnabled)
        {
            if (minVolume > 0)
            {
                m_MinFadeOutVolume = minVolume;
            }

            else
            {
                m_MinFadeOutVolume = 0.0f;
            }

            m_currentVolume = m_AudioSources[0].volume;
            m_TimeInterval = timeInterval;
            m_FadeIn = false;
            m_FadeOut = true;
        }
    }

    public void FadeOutToStop(float timeInterval, float minVolume = 0)
    {
        m_FadeToStop = true;

        FadeOut(timeInterval, minVolume);
    }

    // Play the sound with a fade in from volume 0, over a time interval.
    public void PlayWithFadeIn(float timeInterval)
    {
        Play(0.0f);

        FadeIn(timeInterval);
    }

    // Play the sound with a fade in from volume 0, to a volume, over a time interval.
    public void PlayWithFadeInToVolume(float timeInterval, float volume)
    {
        Play(0.0f);

        FadeInToVolume(timeInterval, volume);
    }

    // Play the sound with a fade in from a specific time.
    public void PlayWithFadeInFromTime(float timeInterval, float time)
    {
        PlayFromTime(time);

        FadeIn(timeInterval);
    }

    // Play the sound with a fade in from a specific time from a specific volume.
    public void PlayWithFadeInFromTime(float timeInterval, float time, float volume)
    {
        PlayFromTime(time, volume);

        FadeIn(timeInterval);
    }
    
    #endregion
}

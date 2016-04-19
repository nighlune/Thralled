using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
    Any and all sounds in Thralled are represented by this class. This is a class that encapsulates a Unity3D AudioSource and AudioClip in a single object. This enables a number of AudioSources with their respective AudioClips to be passed around together.

    A Unity3D AudioClip is a container for audio data. An AudioClip stores the audio file either compressed as ogg vorbis or uncompressed. Audio Clips are referenced and used by AudioSources to play sounds.

    A Unity3D AudioSource is a representation of audio sources in 3D.

    This AudioFile can contain multiple AudioSources and AudioClips for sound/music files that are related to one another and allows functions to be performed on them as a single object. Because there can be multiple AudioSources/Clips, AudioFiles support layering or layers of sounds that are related to one another. You can manipulate an AudioFile's Sources/Clips as one unit or modify volumes of individual Source/Clip pairs. To clarify, a layer is essentially one AudioSource; multiple layers are multiple AudioSources per one AudioFile.

    An AudioFile's position is always set to the camera's position. This is why volumes have to be controlled via scripting. This is the drawback of this audio system. Using 3D sounds in unity requires the sound to be at the source with the camera or other game objects having listeners. This technique removes listeners entirely. If any of the AudioFile's flags are set via the AudioManager, the AudioFile's update function knows what to do and will update according to its state, fading-in, fading-out, etc.

    This AudioFile is created in AudioEngine Load() function and added to the sound engine's list of AudioFiles.
*/
public class AudioFile : MonoBehaviour
{
    // list of AudioSources
    private List<AudioSource> m_AudioSources;

    // List of AudioClips
    private List<AudioClip> m_AudioClips;

    // Specific attributes affecting the AudioFile
    private float m_MinFadeOutVolume;
    private float m_FadeToVolume;
    private float m_TimeInterval;
	private float m_currentVolume;

    // Booleans to keep track of the state of the AudioFile
    private bool m_FadeIn;
    private bool m_FadeInLayer;
    private bool m_FadeOut;
    private bool m_FadeOutLayer;
    private bool m_FadeEnabled;
    private bool m_FadeToStop;

    // The type of AudioFile
    // Ideally this would be abstracted from the AudioFile as this is game-specific
    public AudioType m_Type;

    // List of file paths to the sound files themselves
    public List<string> m_FilePaths;    
    
    // Variables for the volume of the AudioFile
    public float m_InitialVolume;
    public float m_DefaultVolume;

	// Used for initialization
	void Awake()
    {
        // Initialize the AudioFile's Source and Clip
        m_AudioSources = new List<AudioSource>();
        m_AudioClips = new List<AudioClip>();

        m_MinFadeOutVolume = 0.0f;
        m_FadeToVolume = m_DefaultVolume;
        m_TimeInterval = 0.0f;

        m_FadeIn = false;
        m_FadeOut = false;
        m_FadeEnabled = true;
        m_FadeToStop = false;

        // For each of the different sounds related to this AudioFile
        for (int i = 0; i < m_FilePaths.Count; ++i)
        {
            // Load the AudioClip from Thralled's resources folder (containing .wav files)
            AudioClip tempClip = (AudioClip)Resources.Load(m_FilePaths[i]);

            // Add the AudioSource
            AudioSource tempSource = (AudioSource)gameObject.AddComponent("AudioSource");

            // Assign the AudioClip to the AudioSource
            tempSource.clip = tempClip;

            tempSource.volume = m_InitialVolume;

            // Get a reference to the AudioManager
            GameObject tempAudioManager = GameObject.Find("AudioManager");
            if (tempAudioManager != null)
            {
                // I'm not sure what this does...
                tempSource.transform.parent = tempAudioManager.transform;
            }

            m_AudioClips.Add(tempClip);
            m_AudioSources.Add(tempSource);
        }
	}

    // Update is called once per frame
    void Update()
    {
        // Update the AudioFile's position to the Camera's position. In retrospect, this should not be here as it's game-dependent code and should not be in the audio engine.
        if (Camera.mainCamera != null)
        {
            transform.position = Camera.mainCamera.transform.position;
        }

        // If the sound is set to fade in
        if (m_FadeIn)
        {
            // For all the AudioSources in the AudioFile
            for (int i = 0; i < m_AudioSources.Count; ++i)
            {
                // If the volume is less than the volume that we're fading TO
                if (m_AudioSources[i].volume < m_FadeToVolume)
                {
                    // Calculate the volume we should be playing at
                    float currentVolume = m_AudioSources[i].volume + (Time.deltaTime / (m_TimeInterval + 1)) * (m_FadeToVolume - m_currentVolume);
                    m_AudioSources[i].volume = currentVolume;
                }

                // If we've reached the volume that we're fading TO
                else
                {
                    // We don't want to fade anymore
                    m_FadeIn = false;

                    for (int j = 0; j < m_AudioSources.Count; ++j)
                    {
                        // Update all of the AudioSources' to be our target fade-to volume
                        m_AudioSources[j].volume = m_FadeToVolume;
                    }

                    break;
                }
            }
        }

        // If the sound is set to fade out
        if (m_FadeOut)
        {
            // For all the AudioSources in the AudioFile
            for (int i = 0; i < m_AudioSources.Count; ++i)
            {
                // If the volume is greater than the volume that we're fading TO
                if (m_AudioSources[i].volume > m_MinFadeOutVolume)
                {
                    // Calculate the volume we should be playing at
                    float currentVolume = m_AudioSources[i].volume - (Time.deltaTime / (m_TimeInterval + 1)) * (-m_MinFadeOutVolume + m_currentVolume);
                    m_AudioSources[i].volume = currentVolume;
                }

                // If we've reached the volume that we're fading TO
                else
                {
                    // We don't want to fade anymore
                    m_FadeOut = false;

                    for (int j = 0; j < m_AudioSources.Count; ++j)
                    {
                        // If we were fading all the way to stop
                        if (m_FadeToStop)
                        {
                            // Stop playing the AudioSource
                            m_AudioSources[j].Stop();
                        }

                        else
                        {
                            // Otherwise update all of the AudioSources' to be our target fade-to volume
                            m_AudioSources[j].volume = m_MinFadeOutVolume;
                        }
                    }

                    // Reset the flag to fade until stop
                    m_FadeToStop = false;

                    break;
                }
            }
        }
	}

    // Returns the AudioType
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

    /**************************** FUNCTIONS FOR VOLUME ****************************/

    // Returns the current volume of the AudioSource
    public float GetVolume()
    {
        return m_AudioSources[0].volume;
    }    

    // Sets the AudioFile's current volume
    public void SetVolume(float volume)
    {
        for (int i = 0; i < m_AudioSources.Count; ++i)
        {
            if (volume <= m_DefaultVolume)
            {
                // Set each AudioSource to the desired volume
                m_AudioSources[i].volume = volume;
            }
        }
    }

    // Returns a specific layer's volume
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

    // Returns the default volume that was passed in by the Initialize function.
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

    // Resets the volume to the sound's default volume that was passed in by the Initialize function
    public void ResetVolume()
    {
        for (int i = 0; i < m_AudioSources.Count; ++i)
        {
            m_AudioSources[i].volume = m_DefaultVolume;
        }
    }

    /**************************** FUNCTIONS FOR PLAYBACK ****************************/

    // Sets the AudioSource's internal mute flag to true
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

    // Sets the AudioSource's internal mute flag to false
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

    // Returns the current mute value of the AudioSource. If any of the AudioSources is muted this function returns true.
    public bool IsMuted()
    {
        bool isMuted = false;

        for (int i = 0; i < m_AudioSources.Count; ++i)
        {
            if (m_AudioSources[i].mute)
            {
                isMuted = true;
                break;
            }
        }
        return isMuted;
    }

    // Pauses the AudioSource
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

    // Stops the AudioSource from playing
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

    // Stops the specific layer
    public void StopLayer(int layer)
    {
        if (m_AudioSources[layer].isPlaying)
        {
            m_AudioSources[layer].Stop();
        }
    }

    // Plays the AudioFile. If the AudioFile has more than one AudioSource, a random AudioSource is chosen to play. This function will stop the sound if it's already playing and then play it from the beginning.
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

    // Plays the AudioSource at a specific volume
    public void Play(float volume)
    {
        SetVolume(volume);

        Play();
    }

    // Plays a particular AudioSource layer
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

    // Plays a particular AudioSource layer at a specific volume
    public void PlayLayer(int layer, float volume)
    {
        SetLayerVolume(layer, volume);

        PlayLayer(layer);
    }

    // Plays the AudioSource from a specific time. If there's more than one layer to the AudioFile, this function chooses one of the layers at random playing it from the specific time.
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

    // Plays the AudioSource from a specific time from a specific volume
    public void PlayFromTime(float time, float volume)
    {
        SetVolume(volume);

        PlayFromTime(time);
    }

    // Plays a particular layer from a specific time
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

    // Players a particular layer from a specific time from a specific volume
    public void PlayLayerFromTime(int layer, float time, float volume)
    {
        SetLayerVolume(layer, volume);

        PlayLayerFromTime(layer, time);
    }

    // Returns whether the AudioSource(s) are playing as a list of all the AudioSources' isPlaying boolean values
    public List<bool> ArePlaying()
    {
        List<bool> isPlaying = new List<bool>(m_AudioSources.Count);

        for (int i = 0; i < m_AudioSources.Count; ++i)
        {
            isPlaying[i] = m_AudioSources[i].isPlaying;
        }

        return isPlaying;
    }
    
    // Returns whether any of the AudioSource(s) are playing
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

    // Returns whether a specific layer is playing
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

    // Plays the AudioSource, looping the sound. If there are multiple AudioSources, then one is chosen at random to loop-play.
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

    // Plays the AudioSource from a specified time, looping the sound. If there are multiple AudioSources, then one is chosen at random to loop-play from a specified time.
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

    // Plays the AudioSource from a specific time and volume, looping the sound
    public void LoopPlayFromTime(float time, float volume)
    {
        SetVolume(volume);

        LoopPlayFromTime(time);
    }

    // Plays a specific layer, looping the sound
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

    // Plays a specific layer at a specific volume, looping the sound
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

    // Plays a specific layer from a specific time at a specific volume
    public void LoopPlayLayerFromTime(int layer, float time, float volume)
    {
        SetLayerVolume(layer, volume);

        LoopPlayLayerFromTime(layer, time);
    }

    // Sets aall the AudioSources in the AudioFile to loop
    public void Loop()
    {
        for (int i = 0; i < m_AudioSources.Count; ++i)
        {
            m_AudioSources[i].loop = true;
        }
    }

    // Sets an AudioSource's layer to loop
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

    // Sets all the AudioSources' looping booleans to false
    public void StopLooping()
    {
        for (int i = 0; i < m_AudioSources.Count; ++i)
        {
            m_AudioSources[i].loop = false;
        }
    }

    // Sets a layer to stop looping
    public void StopLoopingLayer(int layer)
    {
        m_AudioSources[layer].loop = false;
    }

    // Returns the times in a list that the AudioSources are currently playing at
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
        return m_AudioSources[0].time;
    }

    // Returns the time that a specific layer is currently playing at.
    public float GetLayerTime(int layer)
    {
        return m_AudioSources[layer].time;
    }

    // Returns the length of the AudioClip
    public float GetRuntime()
    {
        return m_AudioSources[0].clip.length;
    }

    /**************************** FUNCTIONS FOR FADING ****************************/

    // Enables fading
    public void EnableFading()
    {
        m_FadeEnabled = true;
    }

    // Disables fading
    public void DisableFading()
    {
        m_FadeEnabled = false;
    }

    // Returns whether fading is enabled
    public bool IsFadingEnabled()
    {
        return m_FadeEnabled;
    }

    // Fades in a sound over a specific time interval (in seconds)
    public void FadeIn(float timeInterval)
    {
        // If fading is enabled
        if (m_FadeEnabled)
        {
            // Set the fade time interval
            m_TimeInterval = timeInterval;

            // Stop fading out if we are
            m_FadeOut = false;

            // Set the flag to fade in
            m_FadeIn = true;

            // Set the fade to volume to the default volume
			m_FadeToVolume = m_DefaultVolume;

            // Set the current volume to the AudioSource's current volume
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

    // Fades out a sound over a specific time interval (in seconds), to a default minimum volume of zero
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

    // Play the sound with a fade in from volume 0, over a time interval
    public void PlayWithFadeIn(float timeInterval)
    {
        Play(0.0f);

        FadeIn(timeInterval);
    }

    // Play the sound with a fade in from volume 0, to a volume, over a time interval
    public void PlayWithFadeInToVolume(float timeInterval, float volume)
    {
        Play(0.0f);

        FadeInToVolume(timeInterval, volume);
    }

    // Play the sound with a fade in from a specific time
    public void PlayWithFadeInFromTime(float timeInterval, float time)
    {
        PlayFromTime(time);

        FadeIn(timeInterval);
    }

    // Play the sound with a fade in from a specific time from a specific volume
    public void PlayWithFadeInFromTime(float timeInterval, float time, float volume)
    {
        PlayFromTime(time, volume);

        FadeIn(timeInterval);
    }
}
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AudioEngine : MonoBehaviour
{
    const int NUM_OF_AUDIOFILES = 200;

    const bool m_DebugEnabled = false;

    protected bool m_Mute;
    protected bool m_BeginTransition;
    protected float m_DeltaTime;
    protected float m_FadeInInterval;
    protected float m_FadeOutInterval;
    protected float m_TransitionTimer;

    // Dictionary to hold all of the AudioFile script names with their corresponding prefab names
    protected Dictionary<string, AudioFile> m_CompleteAudioFilesList;

    // Dictionary to hold all of the loaded AudioFiles
    protected Dictionary<string, AudioFile> m_LoadedAudioFilesList;

    // Dictionary to hold all of the AudioFiles to be removed on UnloadSounds() call
    protected List<string> m_AudioFilesToUnload;

    protected virtual void Awake()
    {
        m_CompleteAudioFilesList = new Dictionary<string, AudioFile>(NUM_OF_AUDIOFILES);
        m_LoadedAudioFilesList = new Dictionary<string, AudioFile>(NUM_OF_AUDIOFILES);

        m_AudioFilesToUnload = new List<string>(30);

        m_DeltaTime = 0.0f;
        m_FadeInInterval = 0.0f;
        m_FadeOutInterval = 0.0f;
        m_TransitionTimer = 0.0f;
        m_BeginTransition = false;
    }

	// Use this for initialization
	protected virtual void Start() {}
	
	// Update is called once per frame
    void Update() {}

    #region Loading

    public void Load(string name)
    {
        if (m_LoadedAudioFilesList.ContainsKey(name))
        {
            if (m_DebugEnabled) print(name + " already loaded!");
        }

        else
        {
            if (!m_CompleteAudioFilesList.ContainsKey(name))
            {
                if (m_DebugEnabled) print(name + " not found!");
            }

            else
            {
                GameObject audioFilePrefab = (GameObject)Resources.Load(name);
                GameObject temp = (GameObject)Instantiate(audioFilePrefab);
                AudioFile audioFile = temp.GetComponent<AudioFile>();
                m_LoadedAudioFilesList.Add(name, audioFile);

                if (m_DebugEnabled) print(name + " is loaded.");
            }
        }
    }

    public void UnloadSounds()
    {
        /*
        foreach (KeyValuePair<string, AudioFile> entry in m_LoadedAudioFilesList)
        {
            //if (entry.Value.GetType() != AudioType.PLAYER || entry.Value.GetType() != AudioType.MUSIC)
            //{
                m_AudioFilesToUnload.Add(entry.Key);
            //}
        }

        for (int i = m_AudioFilesToUnload.Count - 1; i >= 0; --i)
        {
            if (IsPlaying(m_AudioFilesToUnload[i]))
            {
                Stop(m_AudioFilesToUnload[i]);
            }

            m_LoadedAudioFilesList.Remove(m_AudioFilesToUnload[i]);

            Destroy(GameObject.Find(m_AudioFilesToUnload[i]));
        }

        if (m_AudioFilesToUnload.Count > 0)
        {
            m_AudioFilesToUnload.Clear();
        }

        if (m_DebugEnabled) print("Loaded AudioFiles: " + m_LoadedAudioFilesList.Count);
        */
    }

    protected bool IsLoaded(string name)
    {
        if (m_LoadedAudioFilesList != null)
        {
            return m_LoadedAudioFilesList.ContainsKey(name);
        }

        else
        {
            if (m_DebugEnabled) print("IsLoaded(): LoadedAudioFilesList hasn't been loaded yet!");

            return false;
        }
    }

    #endregion

    #region Volume

    public float GetVolume(string mame)
    {
        if (IsLoaded(name))
        {
            return m_LoadedAudioFilesList[name].GetVolume();
        }

        else
        {
            if (m_DebugEnabled) print("GetVolume(): " + name + " is not loaded yet!");

            return 0.0f;
        }
    }

    public void SetVolume(string name, float volume)
    {
        if (IsLoaded(name))
        {
            m_LoadedAudioFilesList[name].SetVolume(volume);
        }

        else
        {
            if (m_DebugEnabled) print("SetVolume(): " + name + " is not loaded yet!");
        }
    }

    public float GetDefaultVolume(string name)
    {
        if (IsLoaded(name))
        {
            return m_LoadedAudioFilesList[name].GetDefaultVolume();
        }

        else
        {
            if (m_DebugEnabled) print("GetDefaultVolume(): " + name + " is not loaded yet!");
            return 0.0f;
        }
    }

    public void SetDefaultVolume(string name, float volume)
    {
        if (IsLoaded(name))
        {
            m_LoadedAudioFilesList[name].SetDefaultVolume(volume);
        }

        else
        {
            if (m_DebugEnabled) print("SetLayerVolume(): " + name + " is not loaded yet!");
        }
    }

    public float GetLayerVolume(string name, int layer)
    {
        if (IsLoaded(name))
        {
            return m_LoadedAudioFilesList[name].GetLayerVolume(layer);
        }

        else
        {
            if (m_DebugEnabled) print("GetLayerVolume(): " + name + " is not loaded yet!");

            return 0.0f;
        }
    }

    public void SetLayerVolume(string name, int layer, float volume)
    {
        if (IsLoaded(name))
        {
            m_LoadedAudioFilesList[name].SetLayerVolume(layer, volume);
        }

        else
        {
            if (m_DebugEnabled) print("SetLayerVolume(): " + name + " is not loaded yet!");
        }
    }

    public void ResetVolume(string name)
    {
        if (IsLoaded(name))
        {
            m_LoadedAudioFilesList[name].ResetVolume();
        }

        else
        {
            if (m_DebugEnabled) print("ResetVolume(): " + name + " is not loaded yet!");
        }
    }

    public void ResetAllVolumes()
    {
        foreach (KeyValuePair<string, AudioFile> entry in m_LoadedAudioFilesList)
        {
            entry.Value.ResetVolume();
        }
    }

    #endregion

    #region Playback

    // Sets the AudioSource's internal mute flag to true.
    public void Mute(string name)
    {
        if (IsLoaded(name))
        {
            m_LoadedAudioFilesList[name].Mute();
        }

        else
        {
            if (m_DebugEnabled) print("Mute(): " + name + " is not loaded yet!");
        }
    }

    // Sets the AudioSource's internal mute flag to false.
    public void UnMute(string name)
    {
        if (IsLoaded(name))
        {
            m_LoadedAudioFilesList[name].UnMute();
        }

        else
        {
            if (m_DebugEnabled) print("UnMute(): " + name + " is not loaded yet!");
        }
    }

    // Returns the current mute value of the AudioSource.
    public bool IsMuted(string name)
    {
        if (IsLoaded(name))
        {
            return m_LoadedAudioFilesList[name].IsMuted();
        }

        else
        {
            if (m_DebugEnabled) print("IsMuted(): " + name + " is not loaded yet!");

            return true;
        }
    }

    public void MuteAll()
    {
        foreach (KeyValuePair<string, AudioFile> entry in m_LoadedAudioFilesList)
        {
            entry.Value.Mute();
        }
    }

    public void UnMuteAll()
    {
        foreach (KeyValuePair<string, AudioFile> entry in m_LoadedAudioFilesList)
        {
            entry.Value.UnMute();
        }
    }

    // Stops the AudioSource from playing.
    public void Stop(string name)
    {
        if (IsLoaded(name))
        {
            m_LoadedAudioFilesList[name].Stop();
        }

        else
        {
            if (m_DebugEnabled) print("Stop(): " + name + " is not loaded yet!");
        }
    }

    // Stops the specific layer.
    public void StopLayer(string name, int layer)
    {
        if (IsLoaded(name))
        {
            m_LoadedAudioFilesList[name].StopLayer(layer);
        }

        else
        {
            if (m_DebugEnabled) print("StopLayer(): " + name + " is not loaded yet!");
        }
    }

    public void StopAllSounds()
    {
        foreach (KeyValuePair<string, AudioFile> entry in m_LoadedAudioFilesList)
        {
            entry.Value.Stop();
        }
    }

    // Plays the AudioSource.
    public void Play(string name)
    {
        if (IsLoaded(name))
        {
            m_LoadedAudioFilesList[name].Play();
        }

        else
        {
            if (!m_CompleteAudioFilesList.ContainsKey(name))
            {
                if (m_DebugEnabled) print("Play(): " + name + " not found!");
            }

            else
            {
                GameObject audioFilePrefab = (GameObject)Resources.Load(name);
                GameObject temp = (GameObject)Instantiate(audioFilePrefab);
                AudioFile audioFile = temp.GetComponent<AudioFile>();
                m_LoadedAudioFilesList.Add(name, audioFile);
                audioFile.Play();
            }
        }
    }

    // Plays the AudioSource at a specific volume.
    public void Play(string name, float volume)
    {
        if (IsLoaded(name))
        {
            m_LoadedAudioFilesList[name].Play(volume);
        }

        else
        {
            if (!m_CompleteAudioFilesList.ContainsKey(name))
            {
                if (m_DebugEnabled) print("Play(): " + name + " not found!");
            }

            else
            {
                GameObject audioFilePrefab = (GameObject)Resources.Load(name);
                GameObject temp = (GameObject)Instantiate(audioFilePrefab);
                AudioFile audioFile = temp.GetComponent<AudioFile>();
                m_LoadedAudioFilesList.Add(name, audioFile);
                audioFile.Play(volume);
            }
        }
    }

    // Plays a particular AudioSource layer.
    public void PlayLayer(string name, int layer)
    {
        if (IsLoaded(name))
        {
            m_LoadedAudioFilesList[name].PlayLayer(layer);
        }

        else
        {
            if (!m_CompleteAudioFilesList.ContainsKey(name))
            {
                if (m_DebugEnabled) print("PlayLayer(): " + name + " not found!");
            }

            else
            {
                GameObject audioFilePrefab = (GameObject)Resources.Load(name);
                GameObject temp = (GameObject)Instantiate(audioFilePrefab);
                AudioFile audioFile = temp.GetComponent<AudioFile>();
                m_LoadedAudioFilesList.Add(name, audioFile);
                audioFile.PlayLayer(layer);
            }
        }
    }

    // Plays a particular AudioSource layer at a specific volume.
    public void PlayLayer(string name, int layer, float volume)
    {
        if (IsLoaded(name))
        {
            m_LoadedAudioFilesList[name].PlayLayer(layer, volume);
        }

        else
        {
            if (!m_CompleteAudioFilesList.ContainsKey(name))
            {
                if (m_DebugEnabled) print("PlayLayer(): " + name + " not found!");
            }

            else
            {
                GameObject audioFilePrefab = (GameObject)Resources.Load(name);
                GameObject temp = (GameObject)Instantiate(audioFilePrefab);
                AudioFile audioFile = temp.GetComponent<AudioFile>();
                m_LoadedAudioFilesList.Add(name, audioFile);
                audioFile.PlayLayer(layer, volume);
            }
        }
    }

    // Plays the AudioSource from a specific time.
    public void PlayFromTime(string name, float time)
    {
        if (IsLoaded(name))
        {
            m_LoadedAudioFilesList[name].PlayFromTime(time);
        }

        else
        {
            if (!m_CompleteAudioFilesList.ContainsKey(name))
            {
                if (m_DebugEnabled) print("PlayFromTime(): " + name + " not found!");
            }

            else
            {
                GameObject audioFilePrefab = (GameObject)Resources.Load(name);
                GameObject temp = (GameObject)Instantiate(audioFilePrefab);
                AudioFile audioFile = temp.GetComponent<AudioFile>();
                m_LoadedAudioFilesList.Add(name, audioFile);
                audioFile.PlayFromTime(time);
            }
        }
    }

    // Plays the AudioSource from a specific time from a specific volume.
    public void PlayFromTime(string name, float time, float volume)
    {
        SetVolume(name, volume);

        PlayFromTime(name, time);
    }

    // Plays a particular layer from a specific time.
    public void PlayLayerFromTime(string name, int layer, float time)
    {
        if (IsLoaded(name))
        {
            m_LoadedAudioFilesList[name].PlayLayerFromTime(layer, time);
        }

        else
        {
            if (!m_CompleteAudioFilesList.ContainsKey(name))
            {
                if (m_DebugEnabled) print("PlayLayerFromTime(): " + name + " not found!");
            }

            else
            {
                GameObject audioFilePrefab = (GameObject)Resources.Load(name);
                GameObject temp = (GameObject)Instantiate(audioFilePrefab);
                AudioFile audioFile = temp.GetComponent<AudioFile>();
                m_LoadedAudioFilesList.Add(name, audioFile);
                audioFile.PlayLayerFromTime(layer, time);
            }
        }
    }

    // Players a particular layer from a specific time from a specific volume.
    public void PlayLayerFromTime(string name, int layer, float time, float volume)
    {
        SetVolume(name, volume);

        PlayLayerFromTime(name, layer, time);
    }

    // Returns whether the AudioSource(s) are playing.
    public List<bool> ArePlaying(string name)
    {
        if (IsLoaded(name))
        {
            return m_LoadedAudioFilesList[name].ArePlaying();
        }

        else
        {
            if (m_DebugEnabled) print("ArePlaying(): " + name + " is not loaded yet!");

            return null;
        }
    }

    // Returns whether the AudioSource(s) are playing.
    public bool IsPlaying(string name)
    {
        if (IsLoaded(name))
        {
            return m_LoadedAudioFilesList[name].IsPlaying();
        }

        else
        {
            if (m_DebugEnabled) print("IsPlaying(): " + name + " is not loaded yet!");

            return false;
        }
    }

    // Returns whether a specific layer is playing.
    public bool IsLayerPlaying(string name, int layer)
    {
        if (IsLoaded(name))
        {
            return m_LoadedAudioFilesList[name].IsLayerPlaying(layer);
        }

        else
        {
            if (m_DebugEnabled) print("IsLayerPlaying(): " + name + " is not loaded yet!");

            return false;
        }
    }

    // Plays the AudioSource, looping the sound.
    public void LoopPlay(string name)
    {
        if (IsLoaded(name))
        {
            m_LoadedAudioFilesList[name].LoopPlay();
        }

        else
        {
            if (!m_CompleteAudioFilesList.ContainsKey(name))
            {
                if (m_DebugEnabled) print("LoopPlay(): " + name + " not found!");
            }

            else
            {
                GameObject audioFilePrefab = (GameObject)Resources.Load(name);
                GameObject temp = (GameObject)Instantiate(audioFilePrefab);
                AudioFile audioFile = temp.GetComponent<AudioFile>();
                m_LoadedAudioFilesList.Add(name, audioFile);
                audioFile.LoopPlay();
            }
        }
    }

    // Plays the AudioSource at a specific volume, looping the sound.
    public void LoopPlay(string name, float volume)
    {
        if (IsLoaded(name))
        {
            m_LoadedAudioFilesList[name].LoopPlay(volume);
        }

        else
        {
            if (!m_CompleteAudioFilesList.ContainsKey(name))
            {
                if (m_DebugEnabled) print("LoopPlay(): " + name + " not found!");
            }

            else
            {
                GameObject audioFilePrefab = (GameObject)Resources.Load(name);
                GameObject temp = (GameObject)Instantiate(audioFilePrefab);
                AudioFile audioFile = temp.GetComponent<AudioFile>();
                m_LoadedAudioFilesList.Add(name, audioFile);
                audioFile.LoopPlay(volume);
            }
        }
    }

    // Plays the AudioSource from a specific time, looping the sound.
    public void LoopPlayFromTime(string name, float time)
    {
        if (IsLoaded(name))
        {
            m_LoadedAudioFilesList[name].LoopPlayFromTime(time);
        }

        else
        {
            if (!m_CompleteAudioFilesList.ContainsKey(name))
            {
                if (m_DebugEnabled) print("LoopPlay(): " + name + " not found!");
            }

            else
            {
                GameObject audioFilePrefab = (GameObject)Resources.Load(name);
                GameObject temp = (GameObject)Instantiate(audioFilePrefab);
                AudioFile audioFile = temp.GetComponent<AudioFile>();
                m_LoadedAudioFilesList.Add(name, audioFile);
                audioFile.LoopPlayFromTime(time);
            }
        }
    }

    // Plays the AudioSource from a specific time and volume, looping the sound.
    public void LoopPlayFromTime(string name, float time, float volume)
    {
        SetVolume(name, volume);

        LoopPlayFromTime(name, time);
    }

    // Plays a specific layer, looping the sound.
    public void LoopPlayLayer(string name, int layer)
    {
        if (IsLoaded(name))
        {
            m_LoadedAudioFilesList[name].LoopPlayLayer(layer);
        }

        else
        {
            if (!m_CompleteAudioFilesList.ContainsKey(name))
            {
                if (m_DebugEnabled) print("LoopPlayLayer(): " + name + " not found!");
            }

            else
            {
                GameObject audioFilePrefab = (GameObject)Resources.Load(name);
                GameObject temp = (GameObject)Instantiate(audioFilePrefab);
                AudioFile audioFile = temp.GetComponent<AudioFile>();
                m_LoadedAudioFilesList.Add(name, audioFile);
                audioFile.LoopPlayLayer(layer);
            }
        }
    }

    // Plays a specific layer at a specific volume, looping the sound.
    public void LoopPlayLayer(string name, int layer, float volume)
    {
        if (IsLoaded(name))
        {
            m_LoadedAudioFilesList[name].LoopPlayLayer(layer, volume);
        }

        else
        {
            if (!m_CompleteAudioFilesList.ContainsKey(name))
            {
                if (m_DebugEnabled) print("LoopPlayLayer(): " + name + " not found!");
            }

            else
            {
                GameObject audioFilePrefab = (GameObject)Resources.Load(name);
                GameObject temp = (GameObject)Instantiate(audioFilePrefab);
                AudioFile audioFile = temp.GetComponent<AudioFile>();
                m_LoadedAudioFilesList.Add(name, audioFile);
                audioFile.LoopPlayLayer(layer, volume);
            }
        }
    }

    // Plays a specific layer from a specific time.
    public void LoopPlayLayerFromTime(string name, int layer, float time)
    {
        if (IsLoaded(name))
        {
            m_LoadedAudioFilesList[name].LoopPlayLayerFromTime(layer, time);
        }

        else
        {
            if (!m_CompleteAudioFilesList.ContainsKey(name))
            {
                if (m_DebugEnabled) print("LoopPlayLayerFromTime(): " + name + " not found!");
            }

            else
            {
                GameObject audioFilePrefab = (GameObject)Resources.Load(name);
                GameObject temp = (GameObject)Instantiate(audioFilePrefab);
                AudioFile audioFile = temp.GetComponent<AudioFile>();
                m_LoadedAudioFilesList.Add(name, audioFile);
                audioFile.LoopPlayLayerFromTime(layer, time);
            }
        }
    }

    // Plays a specific layer from a specific time at a specific volume.
    public void LoopPlayLayerFromTime(string name, int layer, float time, float volume)
    {
        SetVolume(name, volume);
        LoopPlayLayerFromTime(name, layer, time);
    }

    // Sets looping to true.
    public void Loop(string name)
    {
        if (IsLoaded(name))
        {
            m_LoadedAudioFilesList[name].Loop();
        }

        else
        {
            if (m_DebugEnabled) print("Loop(): " + name + " is not loaded yet!");
        }
    }

    // Sets a layer to loop.
    public void LoopLayer(string name, int layer)
    {
        if (IsLoaded(name))
        {
            m_LoadedAudioFilesList[name].LoopLayer(layer);
        }

        else
        {
            if (m_DebugEnabled) print("LoopLayer(): " + name + " is not loaded yet!");
        }
    }

    // Sets looping to false.
    public void StopLooping(string name)
    {
        if (IsLoaded(name))
        {
            m_LoadedAudioFilesList[name].StopLooping();
        }

        else
        {
            if (m_DebugEnabled) print("StopLooping(): " + name + " is not loaded yet!");
        }
    }

    // Sets a layer to stop looping.
    public void StopLoopingLayer(string name, int layer)
    {
        if (IsLoaded(name))
        {
            m_LoadedAudioFilesList[name].StopLoopingLayer(layer);
        }

        else
        {
            if (m_DebugEnabled) print("StopLoopingLayer(): " + name + " is not loaded yet!");
        }
    }

    // Returns the time that the AudioSource(s) are currently playing at.
    public List<float> GetTimes(string name)
    {
        if (IsLoaded(name))
        {
            return m_LoadedAudioFilesList[name].GetTimes();
        }

        else
        {
            if (m_DebugEnabled) print("GetTimes(): " + name + " is not loaded yet!");
            return null;
        }
    }

    public float GetTime(string name)
    {
        if (IsLoaded(name))
        {
            return m_LoadedAudioFilesList[name].GetTime();
        }

        else
        {
            if (m_DebugEnabled) print("GetTime(): " + name + " is not loaded yet!");
            return 0.0f;
        }
    }

    // Returns the time that a specific layer is currently playing at.
    public float GetLayerTime(string name, int layer)
    {
        if (IsLoaded(name))
        {
            return m_LoadedAudioFilesList[name].GetLayerTime(layer);
        }

        else
        {
            if (m_DebugEnabled) print("GetLayerTime(): " + name + " is not loaded yet!");
            return 0.0f;
        }
    }

    public float GetRuntime(string name)
    {
        if (IsLoaded(name))
        {
            return m_LoadedAudioFilesList[name].GetRuntime();
        }

        else
        {
            if (m_DebugEnabled) print("GetRuntime(): " + name + " is not loaded yet!");
            return 0.0f;
        }
    }

    #endregion

    #region Fading

    public void EnableFading(string name)
    {
        if (IsLoaded(name))
        {
            m_LoadedAudioFilesList[name].EnableFading();
        }

        else
        {
            if (m_DebugEnabled) print("EnableFading(): " + name + " is not loaded yet!");
        }
    }

    public void DisableFading(string name)
    {
        if (IsLoaded(name))
        {
            m_LoadedAudioFilesList[name].DisableFading();
        }

        else
        {
            if (m_DebugEnabled) print("DisableFading(): " + name + " is not loaded yet!");
        }
    }

    public bool IsFadingEnabled(string name)
    {
        if (IsLoaded(name))
        {
            return m_LoadedAudioFilesList[name].IsFadingEnabled();
        }

        else
        {
            if (m_DebugEnabled) print("IsFadingEnabled(): " + name + " is not loaded yet!");
            return false;
        }
    }

    public void FadeIn(string name, float timeInterval)
    {
        if (IsLoaded(name))
        {
            m_LoadedAudioFilesList[name].FadeIn(timeInterval);
        }

        else
        {
            if (m_DebugEnabled) print("FadeIn(): " + name + " is not loaded yet!");
        }
    }

    public void FadeInToVolume(string name, float timeInterval, float volume)
    {
        if (IsLoaded(name))
        {
            m_LoadedAudioFilesList[name].FadeInToVolume(timeInterval, volume);
        }

        else
        {
            if (m_DebugEnabled) print("FadeInToVolume(): " + name + " is not loaded yet!");
        }
    }

    public void FadeInAllSounds(float timeInterval)
    {
        foreach (KeyValuePair<string, AudioFile> entry in m_LoadedAudioFilesList)
        {
            entry.Value.FadeIn(timeInterval);
        }
    }
    
    public void FadeOut(string name, float timeInterval, float minVolume = 0)
    {
        if (IsLoaded(name))
        {
            m_LoadedAudioFilesList[name].FadeOut(timeInterval, minVolume);
        }

        else
        {
            if (m_DebugEnabled) print("FadeOut(): " + name + " is not loaded yet!");
        }
    }

    public void FadeOutToStop(string name, float timeInterval, float minVolume = 0)
    {
        if (IsLoaded(name))
        {
            m_LoadedAudioFilesList[name].FadeOutToStop(timeInterval, minVolume);
        }

        else
        {
            if (m_DebugEnabled) print("FadeOutToStop(): " + name + " is not loaded yet!");
        }
    }

    public void PlayWithFadeIn(string name, float timeInterval)
    {
        Play(name);
        FadeIn(name, timeInterval);
    }

    public void PlayWithFadeInToVolume(string name, float timeInterval, float volume)
    {
        Play(name);
        FadeInToVolume(name, timeInterval, volume);
    }

    public void PlayWithFadeInFromTime(string name, float timeInterval, float time)
    {
        if (IsLoaded(name))
        {
            m_LoadedAudioFilesList[name].PlayWithFadeInFromTime(timeInterval, time);
        }

        else
        {
            if (m_DebugEnabled) print("PlayWithFadeInFromTime(): " + name + " is not loaded yet!");
        }
    }

    public void PlayWithFadeInFromTime(string name, float timeInterval, float time, float volume)
    {
        SetVolume(name, volume);
        PlayWithFadeInFromTime(name, timeInterval, time);
    }

    #endregion

    #region Engine

    // Function to return an interval of time between two values (in seconds).
    protected float GetInterval(int lowerBound, int upperBound)
    {
        return (float)Random.Range(lowerBound, upperBound + 1);
    }

    // Function to add an AudioFile to the list of AudioFiles.
    protected void AddAudioFile(string prefabName)
    {
        m_CompleteAudioFilesList.Add(prefabName, null);
    }

    // Returns the number of AudioSources within a given AudioFile.
    public int GetNumOfSources(string name)
    {
        if (IsLoaded(name))
        {
            return m_LoadedAudioFilesList[name].GetNumOfSources();
        }

        else
        {
            if (m_DebugEnabled) print("GetNumOfSources(): " + name + " is not loaded yet!");
            return 0;
        }
    }

    #endregion
}

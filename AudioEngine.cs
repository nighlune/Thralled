using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
    This class is the base class of AudioManager which is the sound engine for Thralled based on the Singleton design pattern. The AudioEngine instantiates the AudioFiles and uses dictionaries to manage which AudioFiles have been loaded and are in use. This class has functionality to load, play, pause, mute, fade-in/out, and set the volume for AudioFiles with corresponding functions to see if an AudioFile is already playing, is muted, is fading, etc.
*/
public class AudioEngine : MonoBehaviour
{
    // Max number of AudioFiles Thralled will used (used for the size of the dictionaries of AudioFiles)
    const int NUM_OF_AUDIOFILES = 200;

    // Debug flag for printing error messages to the console
    const bool m_DebugEnabled = false;

    // Flag for transitioning between different sound ambiances
    protected bool m_BeginTransition;

    protected float m_DeltaTime;
    protected float m_FadeInInterval;
    protected float m_FadeOutInterval;
    protected float m_TransitionTimer;

    // Dictionary to hold all of the AudioFiles to be used in the game accessible by name. This is the master list of AudioFiles used in Thralled.
    protected Dictionary<string, AudioFile> m_CompleteAudioFilesList;

    // Dictionary to hold all of the loaded AudioFiles. These are the AudioFiles that are instantiated.
    protected Dictionary<string, AudioFile> m_LoadedAudioFilesList;

    // List to hold the names of the AudioFiles to be removed on UnloadSounds() call
    protected List<string> m_AudioFilesToUnload;

    // Used for initialization
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

	// Used for when the script is enabled
	protected virtual void Start() {}
	
	// Update is called once per frame
    void Update() {}

    /**************************** FUNCTIONS FOR LOADING AUDIOFILES ****************************/

    /*
        This function loads an AudioFile creating a GameObject from its respective resource. The GameObject is instantiated and we set the AudioFile to the GameObject's component. We then load the AudioFile into the LoadedAudioFilesList which is a dictionary mapping the AudioFile with its name (a string). Loading an AudioFile is necessary for it to be used in Thralled. Loading the object here has the same definition as instantiating the object based off of the object's pre-existing prefab.
    */
    public void Load(string name)
    {
        // If the AudioFile has already been loaded it's in the LoadedAudioFilesList; we don't want to load it again
        if (m_LoadedAudioFilesList.ContainsKey(name))
        {
            if (m_DebugEnabled) print(name + " already loaded!");
        }

        else
        {
            // If the AudioFile doesn't exist for us to load
            if (!m_CompleteAudioFilesList.ContainsKey(name))
            {
                if (m_DebugEnabled) print(name + " not found!");
            }

            // Load the AudioFile, i.e. instantiate it
            else
            {
                // Create the prefab of the AudioFile from the resource's GameObject
                GameObject audioFilePrefab = (GameObject)Resources.Load(name);

                // Instantiate the GameObject using the prefab
                GameObject temp = (GameObject)Instantiate(audioFilePrefab);

                // Create the AudioFile boject from the GameObject's AudioFile component
                AudioFile audioFile = temp.GetComponent<AudioFile>();

                // Add the AudioFile to the list of loaded AudioFiles
                m_LoadedAudioFilesList.Add(name, audioFile);

                if (m_DebugEnabled) print(name + " is loaded.");
            }
        }
    }

    // Returns whether or not an AudioFile has been loaded, i.e. exists in the dictionary of Loaded AudioFiles
    protected bool IsLoaded(string name)
    {
        // If the list of loaded AudioFiles exists
        if (m_LoadedAudioFilesList != null)
        {
            // Return whether or not the AudioFile exists in the list of loaded AudioFiles
            return m_LoadedAudioFilesList.ContainsKey(name);
        }

        else
        {
            if (m_DebugEnabled) print("IsLoaded(): LoadedAudioFilesList hasn't been loaded yet!");

            return false;
        }
    }

    /**************************** FUNCTIONS FOR VOLUME ****************************/

    // Returns the volume of the AudioFile
    public float GetVolume(string mame)
    {
        // If the AudioFile has been loaded
        if (IsLoaded(name))
        {
            // Return it's volume
            return m_LoadedAudioFilesList[name].GetVolume();
        }

        else
        {
            if (m_DebugEnabled) print("GetVolume(): " + name + " is not loaded yet!");

            return 0.0f;
        }
    }

    // Sets the AudioFile's AudioSource volume
    public void SetVolume(string name, float volume)
    {
        if (IsLoaded(name))
        {
            // Set the volume of the AudioFile
            m_LoadedAudioFilesList[name].SetVolume(volume);
        }

        else
        {
            if (m_DebugEnabled) print("SetVolume(): " + name + " is not loaded yet!");
        }
    }

    // Returns the default volume of the AudioFile
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

    // Sets the default volume of the AudioFile
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

    // Returns the current volume of a specific AudioSource (layer) of the AudioFile
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

    // Sets the volume of a specific AudioSource (layer) of the AudioFile
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

    // Resets the volume of the AudioFile to its default volume
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

    // Resets the volume of every loaded AudioFile to its default volume
    public void ResetAllVolumes()
    {
        foreach (KeyValuePair<string, AudioFile> entry in m_LoadedAudioFilesList)
        {
            entry.Value.ResetVolume();
        }
    }

    /**************************** FUNCTIONS FOR PLAYBACK ****************************/

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

    // Returns the current value of the AudioFile's AudioSources' mute flags (true if any are muted, false otherwise)
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

    // Mutes all of the loaded AudioFiles
    public void MuteAll()
    {
        foreach (KeyValuePair<string, AudioFile> entry in m_LoadedAudioFilesList)
        {
            entry.Value.Mute();
        }
    }

    // Unmutes all of the loaded AudioFiles
    public void UnMuteAll()
    {
        foreach (KeyValuePair<string, AudioFile> entry in m_LoadedAudioFilesList)
        {
            entry.Value.UnMute();
        }
    }

    // Stops the AudioFile's AudioSource(s) from playing
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

    // Stops the specific AudioSource (layer) of the AudioFile
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

    // Stops all of the AudioSources of all of the loaded AudioFiles
    public void StopAllSounds()
    {
        foreach (KeyValuePair<string, AudioFile> entry in m_LoadedAudioFilesList)
        {
            entry.Value.Stop();
        }
    }

    // Plays the AudioFile's AudioSource
    public void Play(string name)
    {
        Load(name);

        m_LoadedAudioFilesList[name].Play();
    }

    // Plays the AudioFile's AudioSource at a specific volume
    public void Play(string name, float volume)
    {
        Load(name);

        m_LoadedAudioFilesList[name].Play(volume);
    }

    // Plays a particular AudioSource (layer) of an AudioFile
    public void PlayLayer(string name, int layer)
    {
        Load(name);

        m_LoadedAudioFilesList[name].PlayLayer(layer);
    }

    // Plays a particular AudioSource (layer) of an AudioFile at a specific volume
    public void PlayLayer(string name, int layer, float volume)
    {
        Load(name);

        m_LoadedAudioFilesList[name].PlayLayer(layer, volume);
    }

    // Plays the AudioSource from a specific time
    public void PlayFromTime(string name, float time)
    {
        Load(name);

        m_LoadedAudioFilesList[name].PlayFromTime(time);
    }

    // Plays the AudioSource from a specific time from a specific volume
    public void PlayFromTime(string name, float time, float volume)
    {
        SetVolume(name, volume);

        PlayFromTime(name, time);
    }

    // Plays a particular AudioSource (layer) from a specific time
    public void PlayLayerFromTime(string name, int layer, float time)
    {
        Load(name);

        m_LoadedAudioFilesList[name].PlayLayerFromTime(layer, time);
    }

    // Players a particular AudioSource (layer) from a specific time from a specific volume
    public void PlayLayerFromTime(string name, int layer, float time, float volume)
    {
        SetVolume(name, volume);

        PlayLayerFromTime(name, layer, time);
    }

    // Returns whether the AudioSource(s) are playing
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

    // Returns whether the AudioSource is playing
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

    // Returns whether a specific AudioSource (layer) is playing
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

    // Plays the AudioSource, looping the sound
    public void LoopPlay(string name)
    {
        Load(name);

        m_LoadedAudioFilesList[name].LoopPlay();
    }

    // Plays the AudioSource at a specific volume, looping the sound
    public void LoopPlay(string name, float volume)
    {
        Load(name);

        m_LoadedAudioFilesList[name].LoopPlay(volume);
    }

    // Plays the AudioSource from a specific time, looping the sound
    public void LoopPlayFromTime(string name, float time)
    {
        Load(name);

        m_LoadedAudioFilesList[name].LoopPlayFromTime(time);
    }

    // Plays the AudioSource from a specific time and volume, looping the sound
    public void LoopPlayFromTime(string name, float time, float volume)
    {
        SetVolume(name, volume);

        LoopPlayFromTime(name, time);
    }

    // Plays a specific AudioSource (layer), looping the sound
    public void LoopPlayLayer(string name, int layer)
    {
        Load(name);

        m_LoadedAudioFilesList[name].LoopPlayLayer(layer);
    }

    // Plays a specific AudioSource (layer) at a specific volume, looping the sound
    public void LoopPlayLayer(string name, int layer, float volume)
    {
        Load(name);

        m_LoadedAudioFilesList[name].LoopPlayLayer(layer, volume);
    }

    // Plays a specific AudioSource (layer) from a specific time
    public void LoopPlayLayerFromTime(string name, int layer, float time)
    {
        Load(name);

        m_LoadedAudioFilesList[name].LoopPlayLayerFromTime(layer, time);
    }

    // Plays a specific AudioSource (layer) from a specific time at a specific volume
    public void LoopPlayLayerFromTime(string name, int layer, float time, float volume)
    {
        SetVolume(name, volume);

        LoopPlayLayerFromTime(name, layer, time);
    }

    // Sets the AudioFile's looping flag to true
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

    // Sets an AudioSource (layer) to loop
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

    // Sets the AudioFile's looping flag to false
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

    // Sets an AudioSource (layer) to stop looping
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

    // Returns the times that the AudioSources are currently playing at
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

    // Returns the time the AudioSource is playing at
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

    // Returns the time that a specific AudioSource (layer) is currently playing at
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

    // Returns the length (or runtime) of the AudioFile's AudioClip
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

    /**************************** FUNCTIONS FOR FADING AUDIOFILES IN AND OUT ****************************/

    // Set's the AudioFile's fade flag to true to enable fading for the AudioFile's AudioSource
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

    // Set's the AudioFile's fade flag to false to disable fading for the AudioFile's AudioSource
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

    // Returns whether or not fading is enabled for the AudioFile
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

    // Sets the AudioFile to fade in over a specified time interval
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

    // Sets the AudioFile to fade in over a specified time interval to a specified volume
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

    // Fade in all AudioFiles from 0 volume to their default volume over a specified time interval
    public void FadeInAllSounds(float timeInterval)
    {
        foreach (KeyValuePair<string, AudioFile> entry in m_LoadedAudioFilesList)
        {
            entry.Value.FadeIn(timeInterval);
        }
    }
    
    // Fade out an AudioFile to a specified volume (defaulted to 0 volume) over a specified time interval
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

    // Fade out an AudioFile to a specified volume (defaulted to 0 volume) over a specified time interval and stop the AudioFile when the volume reaches the minimum volume
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

    // Begin playing an AudioFile with a gradual fade in to its default volume over a specified time interval
    public void PlayWithFadeIn(string name, float timeInterval)
    {
        Play(name);

        FadeIn(name, timeInterval);
    }

    // Begin playing an AudioFile with a gradual fade in to a specified volume over a specific time interval
    public void PlayWithFadeInToVolume(string name, float timeInterval, float volume)
    {
        Play(name);

        FadeInToVolume(name, timeInterval, volume);
    }

    // Begin playing an AudioFile with a gradual fade in over a specified time interval from a specific time of the AudioClip
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

    // Begin playing an AudioFile with a gradual fade in over a specified time interval from a specific time of the AudioClip, fading in to a specified volume
    public void PlayWithFadeInFromTime(string name, float timeInterval, float time, float volume)
    {
        SetVolume(name, volume);

        PlayWithFadeInFromTime(name, timeInterval, time);
    }

    /**************************** AUDIOENGINE FUNCTIONS ****************************/

    // Function to return a random interval of time between two values (in seconds).
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
}
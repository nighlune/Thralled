using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*

*/
public class AudioManager : AudioEngine
{
    private bool m_AudioEnabled = true;

    /**************************** SINGLETON ****************************
    Code that causes the AudioManager to be a Singleton pattern in Unity3D
    ********************************************************************/
    private static AudioManager instance;

    public static AudioManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new GameObject("AudioManager").AddComponent<AudioManager>();
            }

            return instance;
        }
    }

    public void OnApplicationQuit()
    {
        instance = null;
    }

    void Awake()
    {
        DontDestroyOnLoad(this);

        base.Awake();
    }
    /**************************** END SINGLETON ****************************/

    private GameObject m_CurrentMusicPrefab;
    public GameObject currentMusicPrefab
    {
        get { return m_CurrentMusicPrefab; }
        set { m_CurrentMusicPrefab = value; }
    }

    // Baby sound variables
    private float m_BabyCryVolume;
    private float m_BabyCalmTimer;
    private float m_BabyCalmInterval;
    private float m_BabyHappyTimer;
    private float m_BabyHappyInterval;
    private float m_BabyUpsetTimer;
    private float m_BabyUpsetInterval;
    private float m_BabyCryTimer;
    private float m_BabyCryInterval;
    private float m_BabyFadeOutInterval;
    private float m_BabyCryMaxVolume;
    private float m_BabyCryMinVolume;  
    private float m_BabyCryVolumeIncrement;
    private float m_BabyCryVolumeUpperBound;
    private float m_BabyCryVolumeLowerBound;  
    private int m_BabyHappyUpperBound;
    private int m_BabyHappyLowerBound;
    private int m_BabyCryUpperBound;
    private int m_BabyCryLowerBound;    
    private bool m_BabyCalmingStateTriggered;
    private bool m_BabyFadeOut;

    // Enums that register the baby's current state; this controls the baby's audible behavior.
    private enum BabyState
    {
        HAPPY,
        UPSET,
        CRYING,
        CALMING,
        NO_BABY
    };
    private BabyState m_BabyState = BabyState.NO_BABY;

    // Ambiance sound variables
    private float m_CricketsTimer1;
    private float m_CricketsTimer2;
    private float m_CricketsInterval1;
    private float m_CricketsInterval2;
    private float m_FliesTimer;
    private float m_FliesInterval;
    private float m_WrenChirpTimer;
    private float m_WrenSongTimer;
    private float m_WrenChirpInterval;
    private float m_WrenSongInterval;
    private float m_WaterDropTimer;
    private float m_WaterDropInterval;
    private float m_HeavenwardTidesVolume;
    private float m_WoodCreakTimer;
    private float m_WoodCreakInterval;
    private float m_ThunderTimer;
    private float m_ThunderInterval;
    private float m_HowlingMonkeyTimer1;
    private float m_HowlingMonkeyTimer2;
    private float m_HowlingMonkeyInterval1;
    private float m_HowlingMonkeyInterval2;
    private float m_RainStaticVolume;
    private float m_RainCryingVolume;
    private float m_HeavenwardTidesFadeInterval;
    private float m_HeavenwardTidesFadeInTimer;

    private int m_CricketsUpperBound;
    private int m_CricketsLowerBound;
    private int m_FliesUpperBound;
    private int m_FliesLowerBound;
    private int m_WrenChirpUpperBound;
    private int m_WrenChirpLowerBound;
    private int m_WrenSongUpperBound;
    private int m_WrenSongLowerBound;
    private int m_WaterDropUpperBound;
    private int m_WaterDropLowerBound;
    private int m_WoodCreakUpperBound;
    private int m_WoodCreakLowerBound;
    private int m_ThunderUpperBound;
    private int m_ThunderLowerBound;
    private int m_HowlingMonkeyUpperBound;
    private int m_HowlingMonkeyLowerBound;

    private bool m_CricketsActive;
    private bool m_FliesActive;
    private bool m_WrenChirpActive;
    private bool m_WrenSongActive;
    private bool m_WindActive;
    private bool m_TorchActive;
    private bool m_WaterDropActive;
    private bool m_WoodCreakActive;
    private bool m_ThunderActive;
    private bool m_HowlingMonkiesActive;

    private enum HeavenwardTidesState
    {
        FADE_IN,
        STATIC,
        ATTACK,
        NONE
    };
    private HeavenwardTidesState m_HTState;

    private enum RainState
    {
        STATIC,
        CRYING,
        NONE
    };
    private RainState m_RainState;
    private RainState m_PreviousRainState = RainState.NONE;

    Globals.Ambiance m_TransitionToAmbiance;
    private bool m_ChangeAmbiance;


    // Use this for initialization
    void Start()
    {
        base.Start();

        #region Player Sound Variables

        AddAudioFile("FootstepGrass");
        AddAudioFile("FootstepStone");
        AddAudioFile("FootstepWood");
        AddAudioFile("FootstepWoodLadder");
        AddAudioFile("GrabChain");
        AddAudioFile("GrabLedge");
        AddAudioFile("GrabRope");
        AddAudioFile("HeartbeatLoop");
        AddAudioFile("HoldLedge");
        AddAudioFile("LandingGrass");
        AddAudioFile("LandingWood");
        AddAudioFile("SlidingLoop");
        AddAudioFile("SugarRowingLoop");
        AddAudioFile("IsauraDeath");

        #endregion

        #region Baby Sound Variables

        AddAudioFile("BabyAmbiance");
        AddAudioFile("BabyCry");
        AddAudioFile("BabyCryLoop");
        AddAudioFile("BabyCryStart");
        AddAudioFile("BabyCryStop");
        AddAudioFile("BabySetDown");
        AddAudioFile("BabyHug");

        m_BabyCryVolume = 0.1f;
        m_BabyCalmTimer = 0.0f;
        m_BabyCalmInterval = 3.0f;
        m_BabyHappyTimer = 0.0f;
        m_BabyHappyInterval = GetInterval(20, 40);
        m_BabyUpsetTimer = 0.0f;
        m_BabyUpsetInterval = 0.0f;
        m_BabyCryTimer = 0.0f;
        m_BabyCryInterval = GetInterval(5, 8);
        m_BabyFadeOutInterval = 0.0f;
        m_BabyCryMaxVolume = 0.6f;
        m_BabyCryMinVolume = 0.1f;

        m_BabyHappyUpperBound = 40;
        m_BabyHappyLowerBound = 20;
        m_BabyCryUpperBound = 8;
        m_BabyCryLowerBound = 5;

        m_BabyCalmingStateTriggered = false;
        m_BabyFadeOut = false;

        #endregion

        #region Ambiance Sound Variables

        AddAudioFile("CricketsLoop1");
        AddAudioFile("CricketsLoop2");
        AddAudioFile("WindForestLightLoop");
        AddAudioFile("WrenChirp");
        AddAudioFile("WrenSong");
        AddAudioFile("TorchLoop");
        AddAudioFile("WaterDropAmbiance");
        AddAudioFile("FliesLoop");
        AddAudioFile("HeavenwardTidesStaticLoop");
        AddAudioFile("HeavenwardTidesAttackLoop");
        AddAudioFile("HeavenwardTidesCountdown");
        AddAudioFile("HeavenwardTidesSplashdown");
        AddAudioFile("WoodCreakAmbiance");
        AddAudioFile("ThunderAmbiance");
        AddAudioFile("ThunderClap");
        AddAudioFile("HowlingMonkey1");
        AddAudioFile("HowlingMonkey2");
        AddAudioFile("RainLight");
        AddAudioFile("RainMedium");
        AddAudioFile("RainHeavy");

        m_CricketsTimer1 = 0.0f;
        m_CricketsTimer2 = 0.0f;
        m_CricketsInterval1 = GetInterval(0, 2);
        m_CricketsInterval2 = GetInterval(1, 3);
        m_FliesTimer = 0.0f;
        m_FliesInterval = GetInterval(2, 4);
        m_WrenChirpTimer = 0.0f;
        m_WrenSongTimer = 0.0f;
        m_WrenChirpInterval = GetInterval(20, 25);
        m_WrenSongInterval = GetInterval(15, 20);
        m_WaterDropTimer = 0.0f;
        m_WaterDropInterval = GetInterval(7, 9);
        m_WoodCreakTimer = 0.0f;
        m_WoodCreakInterval = 15.0f;
        m_ThunderTimer = 0.0f;
        m_ThunderInterval = 25.0f;
        m_HowlingMonkeyTimer1 = 0.0f;
        m_HowlingMonkeyTimer2 = 0.0f;
        m_HowlingMonkeyInterval1 = GetInterval(20, 30);
        m_HowlingMonkeyInterval2 = GetInterval(13, 23);

        m_CricketsUpperBound = 11;
        m_CricketsLowerBound = 10;
        m_FliesUpperBound = 4;
        m_FliesLowerBound = 2;
        m_WrenChirpUpperBound = 30;
        m_WrenChirpLowerBound = 25;
        m_WrenSongUpperBound = 20;
        m_WrenSongLowerBound = 15;
        m_WoodCreakUpperBound = 25;
        m_WoodCreakLowerBound = 12;
        m_ThunderUpperBound = 30;
        m_ThunderLowerBound = 20;
        m_WaterDropUpperBound = 9;
        m_WaterDropLowerBound = 7;
        m_HowlingMonkeyUpperBound = 60;
        m_HowlingMonkeyLowerBound = 40;

        m_CricketsActive = false;
        m_FliesActive = false;
        m_WrenChirpActive = false;
        m_WrenSongActive = false;
        m_WindActive = false;
        m_TorchActive = false;
        m_WaterDropActive = false;
        m_WoodCreakActive = false;
        m_ThunderActive = false;
        m_HowlingMonkiesActive = false;

        m_HeavenwardTidesFadeInterval = 4.0f;
        m_HeavenwardTidesFadeInTimer = 0.0f;
        m_HeavenwardTidesVolume = 0.0f;
        m_RainStaticVolume = 0.15f;
        m_RainCryingVolume = 0.8f;

        m_ChangeAmbiance = false;

        #endregion

        #region Object Sound Variables

        AddAudioFile("CartDragGrass");
        AddAudioFile("CartDragStone");
        AddAudioFile("CartDragWood");
        AddAudioFile("CartLand");
        AddAudioFile("CartFall");
        AddAudioFile("CartImpact");
        AddAudioFile("DoorMetalSlam");
        AddAudioFile("DoorWoodClose");
        AddAudioFile("DoorWoodOpen");
        AddAudioFile("LatchOpen");
        AddAudioFile("KeyGrab");
        AddAudioFile("KeyUnlock");
        AddAudioFile("ChainDrag");
        AddAudioFile("MacheteGrab");
        AddAudioFile("PulleyRopeCut");
        AddAudioFile("TorchFall");
        AddAudioFile("TroncoFall");
        AddAudioFile("WoodCrack");
        AddAudioFile("SugarcaneRustle");
        AddAudioFile("WaterDrop");
        AddAudioFile("GateSlam");
        AddAudioFile("GateDescendLoop");
        AddAudioFile("GateAscendLatch");
        AddAudioFile("SugarcaneGrab");
        AddAudioFile("SugarcaneGrinder");
        AddAudioFile("WoodCreak");
        AddAudioFile("MirrorBreak");
        AddAudioFile("RotationEffect");
        AddAudioFile("GearsLoop");
        AddAudioFile("Breath");
        AddAudioFile("ReflectionFade");
        AddAudioFile("FireOutdoor");
        
        #endregion

        #region Music Variables

        // Chapter 1 Music
        AddAudioFile("MusicCh1Ambiance1");
        AddAudioFile("MusicCh1Ambiance2");
        AddAudioFile("MusicCh1Ambiance3");

        // Chapter 2 Music
        AddAudioFile("MusicCh2Ending");
        AddAudioFile("MusicCh2Intro");
        AddAudioFile("MusicCh2Layer1");
        AddAudioFile("MusicCh2Layer2");
        AddAudioFile("MusicCh2Layer2Short");
        AddAudioFile("MusicCh2Layer3");
        AddAudioFile("MusicCh2Layer4");

        // Chapter 3 Music
        AddAudioFile("MusicCh3Ambiance1");
        AddAudioFile("MusicCh3Ambiance2");
        AddAudioFile("MusicCh3Ambiance3");

        // Chapter 4 Music
        AddAudioFile("MusicCh4Ambiance1");
        AddAudioFile("MusicCh4Ambiance2");
        AddAudioFile("MusicCh4Layer1");
        AddAudioFile("MusicCh4Layer2");
        AddAudioFile("MusicCh4Layer3");
        AddAudioFile("MusicCh4Layer4");

        #endregion

        PreLoadSounds();
    }

    // Update is called once per frame
    void Update()
    {
        if (m_AudioEnabled)
        {
            m_DeltaTime = Time.deltaTime;

            #region Ambiance Transition

            if (m_BeginTransition)
            {
                m_TransitionTimer -= m_DeltaTime;

                if (m_TransitionTimer <= 0.0f)
                {
                    m_BeginTransition = false;
                    Globals.CURRENT_AMBIANCE = m_TransitionToAmbiance;
                    ChangeAmbiance();
                }
            }

            #endregion

            #region Ambiant Sounds

            if (m_CricketsActive)
            {
                m_CricketsTimer1 += m_DeltaTime;
                m_CricketsTimer2 += m_DeltaTime;

                if (m_CricketsTimer1 >= m_CricketsInterval1)
                {
                    m_CricketsTimer1 = 0.0f;
                    m_CricketsInterval1 = GetInterval(m_CricketsLowerBound, m_CricketsUpperBound);

                    if (!IsPlaying("CricketsLoop1"))
                    {
                        Play("CricketsLoop1");
                    }
                }

                if (m_CricketsTimer2 >= m_CricketsInterval2)
                {
                    m_CricketsTimer2 = 0.0f;
                    m_CricketsInterval2 = GetInterval(m_CricketsLowerBound, m_CricketsUpperBound);

                    if (!IsPlaying("CricketsLoop2"))
                    {
                        Play("CricketsLoop2");
                    }
                }
            }

            if (m_HowlingMonkiesActive)
            {
                m_HowlingMonkeyTimer1 += m_DeltaTime;
                m_HowlingMonkeyTimer2 += m_DeltaTime;

                if (m_HowlingMonkeyTimer1 >= m_HowlingMonkeyInterval1)
                {
                    m_HowlingMonkeyTimer1 = 0.0f;
                    m_HowlingMonkeyInterval1 = GetInterval(m_HowlingMonkeyLowerBound, m_HowlingMonkeyUpperBound);

                    if (!IsPlaying("HowlingMonkey1"))
                    {
                        Play("HowlingMonkey1");
                    }
                }

                if (m_HowlingMonkeyTimer2 >= m_HowlingMonkeyInterval2)
                {
                    m_HowlingMonkeyTimer2 = 0.0f;
                    m_HowlingMonkeyInterval2 = GetInterval(m_HowlingMonkeyLowerBound, m_HowlingMonkeyUpperBound);

                    if (!IsPlaying("HowlingMonkey2"))
                    {
                        Play("HowlingMonkey2");
                    }
                }
            }

            if (m_WrenChirpActive)
            {
                m_WrenChirpTimer += m_DeltaTime;

                if (m_WrenChirpTimer >= m_WrenChirpInterval)
                {
                    m_WrenChirpTimer = 0.0f;
                    m_WrenChirpInterval = GetInterval(m_WrenChirpLowerBound, m_WrenChirpUpperBound);

                    Play("WrenChirp");
                }
            }

            if (m_WrenSongActive)
            {
                m_WrenSongTimer += m_DeltaTime;

                if (m_WrenSongTimer >= m_WrenSongInterval)
                {
                    m_WrenSongTimer = 0.0f;
                    m_WrenSongInterval = GetInterval(m_WrenSongLowerBound, m_WrenSongUpperBound);

                    Play("WrenSong");
                }
            }

            if (m_WindActive)
            {
                if (!IsPlaying("WindForestLightLoop"))
                {
                    Play("WindForestLightLoop");
                }
            }

            if (m_TorchActive)
            {
                if (!IsPlaying("TorchLoop"))
                {
                    Play("TorchLoop");
                }
            }

            if (m_WaterDropActive)
            {
                m_WaterDropTimer += m_DeltaTime;

                if (m_WaterDropTimer >= m_WaterDropInterval)
                {
                    m_WaterDropTimer = 0.0f;
                    m_WaterDropInterval = GetInterval(m_WaterDropLowerBound, m_WaterDropUpperBound);

                    Play("WaterDropAmbiance");
                }
            }

            if (m_ThunderActive)
            {
                m_ThunderTimer += m_DeltaTime;

                if (m_ThunderTimer >= m_ThunderInterval)
                {
                    m_ThunderTimer = 0.0f;
                    m_ThunderInterval = GetInterval(m_ThunderLowerBound, m_ThunderUpperBound);

                    Play("ThunderAmbiance");
                }
            }

            if (m_WoodCreakActive)
            {
                m_WoodCreakTimer += m_DeltaTime;

                if (m_WoodCreakTimer >= m_WoodCreakInterval)
                {
                    m_WoodCreakTimer = 0.0f;
                    m_WoodCreakInterval = GetInterval(m_WoodCreakLowerBound, m_WoodCreakUpperBound);

                    Play("WoodCreakAmbiance");
                }
            }

            #endregion

            #region Heavenward Tides

            if (m_HTState == HeavenwardTidesState.STATIC)
            {
                SetVolume("HeavenwardTidesStaticLoop", m_HeavenwardTidesVolume);
            }

            else if (m_HTState == HeavenwardTidesState.FADE_IN)
            {
                m_HeavenwardTidesFadeInTimer += Time.deltaTime;

                if (m_HeavenwardTidesFadeInTimer >= m_HeavenwardTidesFadeInterval)
                {
                    m_HeavenwardTidesFadeInTimer = 0.0f;
                    m_HTState = HeavenwardTidesState.STATIC;
                }
            }

            #endregion

            #region Baby Sounds

            if (m_BabyState == BabyState.HAPPY)
            {
                m_BabyHappyTimer += m_DeltaTime;

                if (m_RainState == RainState.CRYING)
                {
                    RainStaticState();
                }

                if (m_BabyHappyTimer >= m_BabyHappyInterval)
                {
                    m_BabyHappyTimer = 0.0f;
                    m_BabyHappyInterval = GetInterval(m_BabyHappyLowerBound, m_BabyHappyUpperBound);

                    Play("BabyAmbiance");
                }
            }

            else if (m_BabyState == BabyState.CRYING)
            {
                SetVolume("BabyCry", m_BabyCryVolume);
                SetVolume("BabyCryLoop", m_BabyCryVolume);

                if (m_BabyCryVolume <= (m_BabyCryMaxVolume * 0.5f))
                {
                    if (!IsPlaying("BabyCryStart") && !IsPlaying("BabyCry"))
                    {
                        Play("BabyCry");
                    }
                }

                else
                {
                    if (!IsPlaying("BabyCryStart") && !IsPlaying("BabyCry") && !IsPlaying("BabyCryLoop"))
                    {
                        Play("BabyCryLoop");
                    }
                }
            }

            #endregion

            if (m_ChangeAmbiance)
            {
                m_ChangeAmbiance = false;
                FadeInAmbiance(m_FadeInInterval);
            }
        }
    }

    /**************************** FUNCTIONS FOR MODIFYING GAMES AMBIANT SOUNDS ****************************/

    public void ChangeAmbiance()
    {
        if (m_AudioEnabled)
        {
            switch (Globals.CURRENT_AMBIANCE)
            {
                case Globals.Ambiance.JUNGLE:
                    JungleAmbiance(true);
                    PlantationAmbiance(false);
                    CaveAmbiance(false);
                    SenzalaAmbiance(false);
                    DungeonAmbiance(false);
                    MastersHouseAmbiance(false);
                    SlaveShipAmbiance(false);
                    break;

                case Globals.Ambiance.PLANTATION:
                    JungleAmbiance(false);
                    PlantationAmbiance(true);
                    CaveAmbiance(false);
                    SenzalaAmbiance(false);
                    DungeonAmbiance(false);
                    MastersHouseAmbiance(false);
                    SlaveShipAmbiance(false);
                    break;

                case Globals.Ambiance.CAVE:
                    JungleAmbiance(false);
                    PlantationAmbiance(false);
                    CaveAmbiance(true);
                    SenzalaAmbiance(false);
                    DungeonAmbiance(false);
                    MastersHouseAmbiance(false);
                    SlaveShipAmbiance(false);
                    break;

                case Globals.Ambiance.SENZALA:
                    JungleAmbiance(false);
                    PlantationAmbiance(false);
                    CaveAmbiance(false);
                    SenzalaAmbiance(true);
                    DungeonAmbiance(false);
                    MastersHouseAmbiance(false);
                    SlaveShipAmbiance(false);
                    break;

                case Globals.Ambiance.DUNGEON:
                    JungleAmbiance(false);
                    PlantationAmbiance(false);
                    CaveAmbiance(false);
                    SenzalaAmbiance(false);
                    DungeonAmbiance(true);
                    MastersHouseAmbiance(false);
                    SlaveShipAmbiance(false);
                    break;

                case Globals.Ambiance.MASTERS_HOUSE:
                    JungleAmbiance(false);
                    PlantationAmbiance(false);
                    CaveAmbiance(false);
                    SenzalaAmbiance(false);
                    DungeonAmbiance(false);
                    MastersHouseAmbiance(true);
                    SlaveShipAmbiance(false);
                    break;

                case Globals.Ambiance.SLAVE_SHIP:
                    JungleAmbiance(false);
                    PlantationAmbiance(false);
                    CaveAmbiance(false);
                    SenzalaAmbiance(false);
                    DungeonAmbiance(false);
                    MastersHouseAmbiance(false);
                    SlaveShipAmbiance(true);
                    break;

                case Globals.Ambiance.NONE:
                    JungleAmbiance(false);
                    PlantationAmbiance(false);
                    CaveAmbiance(false);
                    SenzalaAmbiance(false);
                    DungeonAmbiance(false);
                    MastersHouseAmbiance(false);
                    SlaveShipAmbiance(false);
                    break;

                case Globals.Ambiance.DEBUG:
                    JungleAmbiance(false);
                    PlantationAmbiance(false);
                    CaveAmbiance(false);
                    SenzalaAmbiance(false);
                    DungeonAmbiance(false);
                    MastersHouseAmbiance(false);
                    SlaveShipAmbiance(false);
                    break;
            }
            
            m_ChangeAmbiance = true;
        }
    }

    // Function to begin a transition, fading out and fading in, between
    // two ambiance types.
    public void StartAmbianceTransition(Globals.Ambiance ambiance, float fadeOutInterval, float fadeInInterval)
    {
        if (m_AudioEnabled)
        {
            m_BeginTransition = true;
            m_FadeOutInterval = fadeOutInterval;
            m_FadeInInterval = fadeInInterval;
            m_TransitionTimer = fadeOutInterval;
            m_TransitionToAmbiance = ambiance;
            FadeOutAmbiance(m_FadeOutInterval, 0.0f);
        }
    }

    // Function to enable the JUNGLE ambiant sounds. 
    private void JungleAmbiance(bool flag)
    {
        m_WindActive = flag;
        m_CricketsActive = flag;
        m_WrenChirpActive = flag;
        m_WrenSongActive = flag;
        m_HowlingMonkiesActive = flag;
    }

    // Function to enable the PLANTATION ambiant sounds.
    private void PlantationAmbiance(bool flag)
    {

    }

    // Function to enable the CAVE ambiant sounds.
    private void CaveAmbiance(bool flag)
    {
        //m_TorchActive = flag;
        m_WaterDropActive = flag;
    }

    // Function to enable the SENZALA ambiant sounds.
    private void SenzalaAmbiance(bool flag)
    {
        m_WoodCreakActive = flag;
    }

    // Function to enable the DUNGEON ambiant sounds.
    private void DungeonAmbiance(bool flag)
    {

    }

    // Function to enable the MASTERS HOUSE ambiant sounds.
    private void MastersHouseAmbiance(bool flag)
    {
        m_WoodCreakActive = flag;
    }

    // Function to enable the SLAVE SHIP ambiant sounds.
    private void SlaveShipAmbiance(bool flag)
    {
        m_WoodCreakActive = flag;
        m_WaterDropActive = flag;
    }

    /**************************** FUNCTIONS FOR FADING ****************************/

    // Function to fade in all of the ambiant sounds that should
    // currently be playing over a specific time interval.
    public void FadeInAmbiance(float timeInterval)
    {
        if (m_AudioEnabled)
        {
            AudioType type = AudioType.NONE;

            switch (Globals.CURRENT_AMBIANCE)
            {
                case Globals.Ambiance.CAVE:
                    type = AudioType.CAVE;
                    break;
                case Globals.Ambiance.DUNGEON:
                    type = AudioType.DUNGEON;
                    break;
                case Globals.Ambiance.JUNGLE:
                    type = AudioType.JUNGLE;
                    break;
                case Globals.Ambiance.MASTERS_HOUSE:
                    type = AudioType.MASTERS_HOUSE;
                    break;
                case Globals.Ambiance.PLANTATION:
                    type = AudioType.PLANTATION;
                    break;
                case Globals.Ambiance.SENZALA:
                    type = AudioType.SENZALA;
                    break;
                case Globals.Ambiance.SLAVE_SHIP:
                    type = AudioType.SLAVE_SHIP;
                    break;
                case Globals.Ambiance.NONE:
                    break;
            }

            foreach (KeyValuePair<string, AudioFile> entry in m_LoadedAudioFilesList)
            {
                if (entry.Value.GetType() == type)
                {
                    if (IsLoaded(entry.Key))
                    {
                        entry.Value.FadeIn(timeInterval);
                    }
                    else
                    {
                        entry.Value.PlayWithFadeIn(timeInterval);
                    }
                }
            }
        }
    }

    // Function to fade out all currently playing ambiant sounds
    // to a minimum volume, over a specific time interval.
    public void FadeOutAmbiance(float timeInterval, float volume)
    {
        if (m_AudioEnabled)
        {
            AudioType type = AudioType.NONE;

            switch (Globals.CURRENT_AMBIANCE)
            {
                case Globals.Ambiance.CAVE:
                    type = AudioType.CAVE;
                    break;
                case Globals.Ambiance.DUNGEON:
                    type = AudioType.DUNGEON;
                    break;
                case Globals.Ambiance.JUNGLE:
                    type = AudioType.JUNGLE;
                    break;
                case Globals.Ambiance.MASTERS_HOUSE:
                    type = AudioType.MASTERS_HOUSE;
                    break;
                case Globals.Ambiance.PLANTATION:
                    type = AudioType.PLANTATION;
                    break;
                case Globals.Ambiance.SENZALA:
                    type = AudioType.SENZALA;
                    break;
                case Globals.Ambiance.SLAVE_SHIP:
                    type = AudioType.SLAVE_SHIP;
                    break;
                case Globals.Ambiance.NONE:
                    break;
            }

            foreach (KeyValuePair<string, AudioFile> entry in m_LoadedAudioFilesList)
            {
                if (IsLoaded(entry.Key) && entry.Value.GetType() == type)
                {
                    if (volume > 0.0f)
                    {
                        entry.Value.FadeOutToStop(timeInterval, volume);
                    }
                    else
                    {
                        entry.Value.Stop();
                    }
                }
            }
        }
    }

    // Function to fade out all sounds that aren't music, to a
    // minimum volume over a specific interval.
    public void FadeOutSounds(float timeInterval, float volume)
    {
        if (m_AudioEnabled)
        {
            foreach (KeyValuePair<string, AudioFile> entry in m_LoadedAudioFilesList)
            {
                if (entry.Value.GetType() != AudioType.MUSIC && entry.Value.IsPlaying())
                {
                    entry.Value.FadeOut(timeInterval, volume);
                }
            }
        }
    }

    public void FadeInMusic(float timeInterval)
    {
        if (m_AudioEnabled)
        {
            foreach (KeyValuePair<string, AudioFile> entry in m_LoadedAudioFilesList)
            {
                if (entry.Value.GetType() == AudioType.MUSIC && entry.Value.IsPlaying())
                {
                    entry.Value.FadeIn(timeInterval);
                }
            }
        }
    }

    // Function to fade out all music, to a
    // minimum volume over a specific interval.
    public void FadeOutMusic(float timeInterval, float volume = 0)
    {
        if (m_AudioEnabled)
        {
            foreach (KeyValuePair<string, AudioFile> entry in m_LoadedAudioFilesList)
            {
                if (entry.Value.GetType() == AudioType.MUSIC && entry.Value.IsPlaying())
                {
                    entry.Value.FadeOutToStop(timeInterval, volume);
                }
            }
        }
    }

    /**************************** FUNCTIONS FOR STOPPING SOUNDS/MUSIC ****************************/

    // Function to stop all sounds that aren't music.
    public void StopSounds()
    {
        if (m_AudioEnabled)
        {
            foreach (KeyValuePair<string, AudioFile> entry in m_LoadedAudioFilesList)
            {
                if (entry.Value.GetType() != AudioType.MUSIC && entry.Value.IsPlaying())
                {
                    entry.Value.Stop();
                }
            }
        }
    }

    // Function to stop all music sounds.
    public void StopMusic()
    {
        if (m_AudioEnabled)
        {
            foreach (KeyValuePair<string, AudioFile> entry in m_LoadedAudioFilesList)
            {
                if (entry.Value.GetType() == AudioType.MUSIC && entry.Value.IsPlaying())
                {
                    entry.Value.Stop();
                }
            }
        }
    }

    /**************************** FUNCTIONS FOR BABY ****************************/

    public void BabyHappyState()
    {
        if (m_AudioEnabled && m_BabyState != BabyState.HAPPY)
        {
            m_BabyState = BabyState.HAPPY;
            Debug.Log("BS: HAPPY");

            if (m_BabyFadeOut)
            {
                foreach (KeyValuePair<string, AudioFile> entry in m_LoadedAudioFilesList)
                {
                    if (IsLoaded(entry.Key) && entry.Value.GetType() == AudioType.BABY && entry.Value.IsPlaying())
                    {
                        entry.Value.FadeOutToStop(m_BabyFadeOutInterval);
                    }
                }

                m_BabyFadeOut = false;
            }

            else
            {
                if (IsPlaying("BabyCry"))
                {
                    Stop("BabyCry");
                }

                if (IsPlaying("BabyCryLoop"))
                {
                    Stop("BabyCryLoop");
                }
            }
        }
    }

    public void BabyUpsetState()
    {
        if (m_AudioEnabled && m_BabyState != BabyState.UPSET)
        {
            m_BabyState = BabyState.UPSET;
            Debug.Log("BS: UPSET");
            Play("BabySetDown", m_BabyCryMinVolume);
        }
    }

    public void BabyCryingState()
    {
        if (m_AudioEnabled && m_BabyState != BabyState.CRYING)
        {
            m_BabyState = BabyState.CRYING;
            Debug.Log("BS: CRYING");
            Play("BabyCryStart", m_BabyCryMinVolume);            
        }
    }

    public void BabyCalmingState()
    {
        if (m_AudioEnabled && m_BabyState != BabyState.CALMING)
        {               
            m_BabyState = BabyState.CALMING;
            Debug.Log("BS: CALMING");            
        }
    }

    public void BabyNoState()
    {
        if (m_AudioEnabled && m_BabyState != BabyState.NO_BABY)
        {
            m_BabyState = BabyState.NO_BABY;
            Debug.Log("BS: NONE");

            foreach (KeyValuePair<string, AudioFile> entry in m_LoadedAudioFilesList)
            {
                if (IsLoaded(entry.Key) && entry.Value.GetType() == AudioType.BABY && entry.Value.IsPlaying())
                {
                    entry.Value.Stop();
                }
            }           
        }
    }

    public void SetBabyCryVolume(float volume)
    {
        if (m_AudioEnabled)
        {
            m_BabyCryVolume = Mathf.Lerp(m_BabyCryMinVolume, m_BabyCryMaxVolume, volume);
        }
    }

    public void BabyFadeOut(float timeInterval)
    {
        if (m_AudioEnabled && m_BabyFadeOut != true)
        {
            m_BabyFadeOut = true;
            m_BabyFadeOutInterval = timeInterval;
        }
    }

    #endregion

    /**************************** FUNCTIONS FOR HEAVENWARD TIDES ****************************/

    public void SetHeavenwardTidesVolume(float volume)
    {
        if (m_AudioEnabled)
        {
            if (m_HTState == HeavenwardTidesState.STATIC)
            {
                m_HeavenwardTidesVolume = volume;
            }
        }
    }

    public void HeavenwardTidesFadeInState(float volume, float timeInterval)
    {
        if (m_AudioEnabled && m_HTState != HeavenwardTidesState.FADE_IN)
        {
            m_HTState = HeavenwardTidesState.FADE_IN;
            Debug.Log("HWTS: FADE_IN");

            if (IsPlaying("HeavenwardTidesAttackLoop"))
            {
                FadeOutToStop("HeavenwardTidesAttackLoop", m_HeavenwardTidesFadeInterval);
            }

            m_HeavenwardTidesFadeInterval = timeInterval;
            LoopPlay("HeavenwardTidesStaticLoop");
            FadeInToVolume("HeavenwardTidesStaticLoop", timeInterval, volume);           
        }
    }

    public void HeavenwardTidesStaticState()
    {
        if (m_AudioEnabled && m_HTState != HeavenwardTidesState.STATIC)
        {
            m_HTState = HeavenwardTidesState.STATIC;
            Debug.Log("HWTS: STATIC");

            if (!IsPlaying("HeavenwardTidesStaticLoop"))
            {
                LoopPlay("HeavenwardTidesStaticLoop");
            }
            if (IsPlaying("HeavenwardTidesAttackLoop"))
            {
                FadeOutToStop("HeavenwardTidesAttackLoop", m_HeavenwardTidesFadeInterval);
            }
            if (IsPlaying("HeavenwardTidesCountdown"))
            {
                FadeOutToStop("HeavenwardTidesCountdown", m_HeavenwardTidesFadeInterval);
            }            
        }
    }

    public void HeavenwardTidesAttackState()
    {
        if (m_AudioEnabled && m_HTState != HeavenwardTidesState.ATTACK)
        {
            m_HTState = HeavenwardTidesState.ATTACK;
            Debug.Log("HWTS: ATTACK");

            if (IsPlaying("HeavenwardTidesStaticLoop"))
            {
                FadeOutToStop("HeavenwardTidesStaticLoop", m_HeavenwardTidesFadeInterval);
            }
            if (!IsPlaying("HeavenwardTidesAttackLoop"))
            {
                LoopPlay("HeavenwardTidesAttackLoop");
                FadeIn("HeavenwardTidesAttackLoop", m_HeavenwardTidesFadeInterval);
            }            
        }
    }

    public void HeavenwardTidesNoState()
    {
        if (m_AudioEnabled && m_HTState = HeavenwardTidesState.NONE)
        {
            m_HTState = HeavenwardTidesState.NONE;
            Debug.Log("HWTS: NONE");

            foreach (KeyValuePair<string, AudioFile> entry in m_LoadedAudioFilesList)
            {
                if (IsLoaded(entry.Key) && entry.Value.GetType() == AudioType.HEAVENWARD_TIDES && entry.Value.IsPlaying())
                {
                    entry.Value.Stop();
                }
            }
        }
    }

    #endregion

    #region Thunder

    public void ThunderAmbianceStart()
    {
        if (m_AudioEnabled && m_ThunderActive != true)
        {
            m_ThunderActive = true;
        }
    }

    public void ThunderAmbianceStop()
    {
        if (m_AudioEnabled && m_ThunderActive != false)
        {
            if (IsPlaying("ThunderAmbiance"))
            {
                Stop("ThunderAmbiance");
            }

            m_ThunderActive = false;
        }
    }

    #endregion

    #region Rain

    public void RainStaticState()
    {
        m_RainState = RainState.STATIC;
        Debug.Log("RS: STATIC");

        ThunderAmbianceStart();

        if (!IsPlaying("RainMedium"))
        {
            SetVolume("RainMedium", 0.0f);
            LoopPlay("RainMedium");
            FadeInToVolume("RainMedium", 4.0f, m_RainStaticVolume);
        }

        if (IsPlaying("RainMedium") && m_PreviousRainState == RainState.CRYING)
        {
            FadeOut("RainMedium", 4.0f, m_RainStaticVolume);
        }

        m_PreviousRainState = RainState.STATIC;

        
    }

    public void RainCryingState()
    {
        m_RainState = RainState.CRYING;
        Debug.Log("RS: CRYING");

        if (IsPlaying("RainMedium"))
        {
            FadeInToVolume("RainMedium", 5.0f, m_RainCryingVolume);
        }

        if (!IsPlaying("RainMedium"))
        {
            LoopPlay("RainMedium");
            FadeInToVolume("RainMedium", 5.0f, m_RainCryingVolume);
        }

        m_PreviousRainState = RainState.CRYING;        
    }

    public void RainNoState()
    {
        m_RainState = RainState.NONE;
        Debug.Log("RS: NONE");

        ThunderAmbianceStop();

        if (IsPlaying("RainMedium"))
        {
            FadeOutToStop("RainMedium", 2.0f);
        }

        m_PreviousRainState = RainState.NONE;
    }

    #endregion

    #region Engine

    public void DisableAudio()
    {
        m_AudioEnabled = false;
    }

    public void EnableAudio()
    {
        m_AudioEnabled = true;
    }

    public bool IsAudioEnabled()
    {
        return m_AudioEnabled;
    }

    public void PreLoadSounds()
    {
        Load("HeavenwardTidesCountdown");
    }

    #endregion
}
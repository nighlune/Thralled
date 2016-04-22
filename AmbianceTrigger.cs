using UnityEngine;
using System.Collections;

/*
    This class encapsulates a GameObject and a script to trigger a transition in Thralled's ambiance. This script is attached to a collider that interacts with the player character and will transition the game's ambiance. Because this is a GameObject, the variables that are public are initialized via the GameObject in Unity3D with values inputed by the game.
*/
public class AmbianceTrigger : MonoBehaviour
{
    // The ambiance we want to transition to
    public Globals.Ambiance m_TransitionToAmbiance;

    // The time interval over which to fade out
    public float m_FadeOutAmbianceTime;

    // The time interval over which to fade in
    public float m_FadeInAmbianceTime;

    // A flag that determines whether or not there's a delay in the transition
    public bool m_Delay;

    // The delay in activating the transition
    public float m_DelayTime;

    // The delay timer
    private float m_DelayTimer;

    // A flag to indicate whether the collider has been triggered
    private bool m_TriggerOnce;

    // A flag we don't set until the delay has concluded so that we call the AudioManager to begin the transition only once
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
        // If the collider has been triggered once and we haven't be
        if (m_TriggerOnce && !m_IsPlaying)
        {
            // If we've set a transition delay
            if (m_Delay)
            {
                // Increment the timer according to the delta time
                m_DelayTimer += Time.deltaTime;

                // If the timer has reached the delay time
                if (m_DelayTimer >= m_DelayTime)
                {
                    // Set the flag to ensure we invoke the AudioManager only once
                    m_IsPlaying = true;

                    // Reset the delay timer
                    m_DelayTimer = 0.0f;

                    // Invoke the AudioManager to begin the ambiance transition according to the specified ambiance, the current ambiance fade out time, and the time interval to which to fade in the next ambiance
                    AudioManager.Instance.StartAmbianceTransition(m_TransitionToAmbiance, m_FadeOutAmbianceTime, m_FadeInAmbianceTime);
                }
            }

            // If we haven't set a delay
            else
            {
                m_IsPlaying = true;
                AudioManager.Instance.StartAmbianceTransition(m_TransitionToAmbiance, m_FadeOutAmbianceTime, m_FadeInAmbianceTime);
            }
        }
    }

    // Function to receive the collision from the player object, triggering the delay and/or ambiance transition
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" && !m_TriggerOnce)
        {
            m_TriggerOnce = true;            
        }
    }
}

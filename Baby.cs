using UnityEngine;
using System.Collections;

public class Baby : InteractiveObject 
{
	private const string COMFORT_ICON_FILE_PATH = "Textures/ComfortBaby";
	
	public GameObject m_CalmBabyModule;
	private ControllerModule m_CalmBabyScript;
	private bool m_LoopComfort;
	
	// Variables related to the baby's crying mechanic
	public bool m_IsCrying;
	public float m_CryCounter;
	public float m_MaxCryValue;
	public float m_CryThreshold;


	public float m_CaptorSpawnThreshold;
	public float m_ComfortTime;
	private bool m_BeingComforted;
	private bool m_ShowComfortIcon;
	private float m_ComfortCounter;
	
	private Texture2D m_ComfortIcon;


	// Use this for initialization
	new void Start () 
	{
		Debug.Log ("Baby Start");
		base.Start();

		m_Type = InteractiveObjectType.BABY;
		
		m_CalmBabyScript = m_CalmBabyModule.GetComponent<ControllerModule>();
		m_CalmBabyScript.OnSelect += OnSelectCalmBaby;
		m_PickupXOffset = 0.112f;
		m_BeingComforted = false;
		m_ComfortCounter = 0;
		
		m_ComfortIcon = (Texture2D) Resources.Load (COMFORT_ICON_FILE_PATH);
	}
	
	public override void SetAsPickedUp(PlayerObjectInteraction poiScript, bool IsPlayer)
	{
		m_POIScript = poiScript;
		m_IsPickedUp = true;
		m_IsPickedUpByPlayer = IsPlayer;
		renderer.enabled = false;
		PlayerModel.CanLedgeClimb = false;
		if(m_Outline != null)
			m_Outline.renderer.enabled = false;
		ControllerManager.MainController.RemoveModule(m_PickupModuleScript);
	}
	
	public override void EndPickUp()
	{
		ControllerManager.MainController.Enabled = true;
		
		m_IsPickedUp = true;
		
		if(m_IsCrying)
		{
			ControllerManager.Active = false;
			m_ShowComfortIcon = true;
		}
		else
		{
			m_CryCounter = 0;
		}
	}
	
	void OnGUI()
	{
		if(m_ShowComfortIcon)
		{
			Rect hitbox = new Rect(ControllerManager.ControllerPosition.x - m_ComfortIcon.width * .5f, ControllerManager.ControllerPosition.y - m_ComfortIcon.height * .5f, m_ComfortIcon.width, m_ComfortIcon.height);
			
			
			Vector2 mousePos = Utils.MousePositionUnityPlayer();
			
			Color originalColor = GUI.color;
			
			if(Input.GetMouseButton(0) && hitbox.Contains(mousePos))
			{
				GUI.color = new Color(.86f, .67f, .043f, originalColor.a);
				Use (false);
			}
			
			GUI.DrawTexture(hitbox, m_ComfortIcon, ScaleMode.StretchToFill);
			
			GUI.color = originalColor;
		}
	}
	
	// Update is called once per frame
	protected override void Update () 
	{
		if(m_IsPickedUpByPlayer == PlayerModel.IsPlayer)
		{			
			if(m_IsPickedUp)
			{
				//Debug.Log(m_POIScript);
				if(m_POIScript.m_InSafeZone && !m_PutDownModuleScript.Added)
				{
					ControllerManager.MainController.AddModule(m_PutDownModuleScript);
				}
				else if(!m_POIScript.m_InSafeZone && m_PutDownModuleScript.Added)
				{
					ControllerManager.MainController.RemoveModule(m_PutDownModuleScript);
				}
			}
		}

		// This is the code within the baby affecting her crying patterns when she's left on the ground or when she's picked up by Isaura.

		// If the baby isn't being held by Isaura		
		if(!m_IsPickedUp)
		{
			// Increment the crying counter according to the game's current time
			m_CryCounter += Time.deltaTime;
			m_CryCounter = Mathf.Clamp(m_CryCounter, 0, m_MaxCryValue);

			// Begin the baby crying
			if(!m_IsCrying && m_CryCounter >= m_CryThreshold)
			{
				m_IsCrying = true;

				// Set the AudioManager's states (this containts the logic for playing the sounds)
                AudioManager.Instance.BabyCryingState();
                AudioManager.Instance.RainCryingState();
			}
		}
		else // The baby is being held
		{
			// The baby is being comforted
			if(m_BeingComforted)
			{
				// Increment the comfort counter according to the game's current time
				m_ComfortCounter += Time.deltaTime;
				
				// If the "hug" sound isn't playing, play it
                if(!AudioManager.Instance.IsPlaying("BabyHug"))
                {
                    AudioManager.Instance.Play("BabyHug");
                }

                // Set the AudioManager's state
                AudioManager.Instance.BabyCalmingState();

				if(m_IsCrying && m_ComfortCounter >= m_ComfortTime)
				{
					m_IsCrying = false;
					ControllerManager.Active = true;
					m_ShowComfortIcon = false;
					m_BeingComforted = false;
					m_ComfortCounter = 0;
					m_CryCounter = 0;

                    AudioManager.Instance.BabyHappyState();
                    AudioManager.Instance.HeavenwardTidesStaticState();
				}
			}
			else
			{
				m_ComfortCounter = 0;
			}
		}
		
		if(m_IsCrying)
		{
            float cryVolume = (m_CryCounter - m_CryThreshold) / (m_MaxCryValue - m_CryThreshold);
            AudioManager.Instance.SetBabyCryVolume(cryVolume);
		}
	}
	
	public override void Use(bool left)
	{
		if(!m_BeingComforted)
		{
			PlayerModel.CanMove = false;
			PlayerModel.Anim.Play(PlayerModel.UseAnimation);
			PlayerModel.ArmsAnim.Play (PlayerModel.ArmUseAnimation);
			PlayerEventManager.AddArmAnimationEventDelegate(OnBabyLoop);
			PlayerEventManager.AddAnimationCompleteDelegate(OnUseComplete);	
			m_BeingComforted = true;
		}
		else
		{
			m_LoopComfort = true;
		}
	}
	
	public void OnBabyLoop(tk2dAnimatedSprite sprite, tk2dSpriteAnimationClip clip, tk2dSpriteAnimationFrame frame, int frameNum)
	{
		//Debug.Log ("event baby use " + frame.eventInfo);
		if(frame.eventInfo == "loopHold")
		{
			//m_CryCounter -= m_CryThreshold / 3;

			if(m_LoopComfort)
			{
				PlayerModel.Anim.Play(PlayerModel.UseAnimation, .458333f);
				PlayerModel.ArmsAnim.Play (PlayerModel.ArmUseAnimation, .458333f);
				m_LoopComfort = false;
			}
			else
			{
				PlayerEventManager.RemoveArmAnimationEventDelegate(OnBabyLoop);
			}
		}
	}
	
	public void OnUseComplete(tk2dAnimatedSprite sprite, int clipId)
	{
		PlayerModel.CanMove = true;
		m_BeingComforted = false;
		base.Use(true);
	}
	
	public override void PickUp()
	{		
		PlayerModel.ShowArms(true);
		PlayerModel.CanLedgeClimb = false;
		
		if(m_Placement == InteractiveObjectPlacement.DOWN)
		{
			PlayerModel.Anim.Play (PlayerModel.PickUpAnimation, .583f);
			PlayerModel.ArmsAnim.Play(PlayerModel.ArmPickUpAnimation, .583f);
		}
		
		
		
		base.PickUp();
	}
	
	public override void SetAnimations ()
	{
		Debug.Log ("baby set animations");
		
		PlayerModel.JumpAnimation = "JumpBody";
		PlayerModel.StandAnimation = "StandBody";
		PlayerModel.JumpForwardAnimation = "JumpForwardBody";
		PlayerModel.JogAnimation = "JogBody";
		PlayerModel.PickUpAnimation = "PickUpBody";
		PlayerModel.PickUpFrontAnimation = "PickUpFrontBody";
		PlayerModel.SlideAnimation = "SlideBody";
		PlayerModel.LandAnimation = "LandBody";
		PlayerModel.UseAnimation = "UseBabyBody";
		PlayerModel.FallAnimation = "FallWithBabyBody";
		PlayerModel.PutDownAnimation = "PutDownBody";
		
		
		PlayerModel.ArmPickUpAnimation = "PickUpArmBaby";
		PlayerModel.ArmPickUpFrontAnimation = "PickUpFrontArmBaby";
		PlayerModel.ArmJumpAnimation = "JumpArmBaby";
		PlayerModel.ArmStandAnimation = "StandArmBaby";
		PlayerModel.ArmJumpForwardAnimation = "JumpForwardArmBaby";
		PlayerModel.ArmJogAnimation = "JogArmBaby";
		PlayerModel.ArmSlideAnimation = "SlideArmBaby";
		PlayerModel.ArmLandAnimation = "LandArmBaby";
		PlayerModel.ArmUseAnimation = "UseArmBaby";
		PlayerModel.ArmFallAnimation = "FallArmBaby";
		PlayerModel.ArmPutDownAnimation = "PutDownArmBaby";
		PlayerModel.ArmSlopeClimbAnimation = "SlopeClimbArmBaby";
		base.SetAnimations ();
	}
	
	public override void PutDown()
	{
		Debug.Log ("baby put down");
		
		PlayerModel.UseDefaultAnimations();
		PlayerModel.CanLedgeClimb = false;
		PlayerModel.Anim.Play (PlayerModel.PutDownAnimation, .583f);
		PlayerModel.ArmsAnim.Play(PlayerModel.ArmPutDownAnimation, .583f);
		float putdownYvalue;
		if(CurrentHolder.gameObject.name == "Player")
		{
			putdownYvalue = -0.2626549f;
		}
		else
		{
			putdownYvalue = 0.2626549f;
		}
		
		if(PlayerModel.IsFacingRight)
		{
			transform.position = CurrentHolder.transform.position + new Vector3(0.1167164f, putdownYvalue, -0.5f);
			transform.eulerAngles = new Vector3(0, 0, 0);
		}
		else
		{
			transform.position = CurrentHolder.transform.position + new Vector3(-0.1167164f, putdownYvalue, -0.5f);
			transform.eulerAngles = new Vector3(0, 0, 0);
		}

        AudioManager.Instance.BabyUpsetState();

		base.PutDown();
	}
	
	public void ResetAfterRespawn()
	{
		//resetCounter();
		Debug.Log("reset after respawn");
		if(m_Outline != null)
			m_Outline.renderer.enabled = false;
		
		m_IsPickedUp = true;
		renderer.enabled = false;
		PlayerModel.CanLedgeClimb = false;
		PlayerModel.ShowArms(true);
		
		SetAnimations ();
		m_CryCounter = 0;
		m_Sprite.color = Color.white;
		m_IsCrying = false;
		
	}
	
	public bool IsCrying
	{
		get { return m_IsCrying; }
	}
}

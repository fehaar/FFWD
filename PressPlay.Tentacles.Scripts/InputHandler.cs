using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using PressPlay.FFWD;
using PressPlay.FFWD.Components;
using Microsoft.Xna.Framework.Content;

namespace PressPlay.Tentacles.Scripts {
	public class InputHandler : MonoBehaviour {
		
		public Plane screenInputPlane;
		
		
		private bool collectInputFlickVectors = false;
        private List<Vector2> inputList = new List<Vector2>();
        private List<Vector2> inputDifferenceList = new List<Vector2>();
		public ControlSchemeButton[] inputFlickActivationScheme = new ControlSchemeButton[]{ControlSchemeButton.leftMouse};
		public ControlSchemeScreenPosition inputFlickPositionScheme = ControlSchemeScreenPosition.mousePosition;
		//public ControlSchemeButton[] endInputFlickScheme = new ControlSchemeButton[]{ControlSchemeButton.leftMouseUp};
		
		public ControlSchemeButton[] keepConnectionScheme = new ControlSchemeButton[]{ControlSchemeButton.leftMouse};
		
		public ControlSchemeButton[] shootClawScheme = new ControlSchemeButton[]{ControlSchemeButton.rightMouseDown};
		public ControlSchemeButton[] shootTentacleScheme = new ControlSchemeButton[]{ControlSchemeButton.leftMouseDown};
		public ControlSchemeButton[] controlTentacleScheme = new ControlSchemeButton[]{ControlSchemeButton.leftMouse};
		public ControlSchemeScreenPosition inputPositionScheme = ControlSchemeScreenPosition.mousePosition;
		
		public ControlSchemeVector tentacleDirectionScheme = ControlSchemeVector.leftAnalog;
		public ControlSchemeVector clawDirectionScheme = ControlSchemeVector.leftAnalog;
		
		public bool turnOff = false;
		
		private float lastUpdateTime = 0;
		
		private Vector2 inputPosition;
		private Vector2 lastInputPosition;
		
		[ContentSerializerIgnore]
		public float dTime = 0;
		
		private float leftAnalogXAxis = 0f;
		private float leftAnalogYAxis = 0f;
		public float leftAnalogDeadZone = 0.2f;
		private float leftAnalogMagnitude = 0;
		public float leftAnalogCurvePow = 1.6f;
		private Vector2 leftAnalogVector = Vector2.Zero;
		
		private float rightAnalogXAxis = 0f;
		private float rightAnalogYAxis = 0f;
		public float rightAnalogDeadZone = 0.2f;
		private float rightAnalogMagnitude = 0;
		public float rightAnalogCurvePow = 1.6f;
		private Vector2 rightAnalogVector = Vector2.Zero;
		
        //private float wasdXAxis = 0f;
        //private float wasdYAxis = 0f;
		private Vector2 wasdVector = Vector2.Zero;
		
        //private float arrowKeysXAxis = 0f;
        //private float arrowKeysYAxis = 0f;
		private Vector2 arrowKeysVector = Vector2.Zero;
	
		private float leftTriggerAxis = -1;
		private float rightTriggerAxis = -1;
		
		public float leftTriggerAxisCurvePow = 1f;
		public float rightTriggerAxisCurvePow = 1f;
		
		private static InputHandler instance;
		
		public static bool isLoaded = false;
		
		public enum ControlSchemeButton{
			buttonA,
			buttonB,
			buttonX,
			buttonY,
			rightTriggerButton,
			leftTriggerButton,
			rightTriggerAxisHalfWay,
			rightTriggerAxisHairTrigger,
			leftTriggerAxisHalfWay,
			leftTriggerAxisHairTrigger,
			rightAnalogButton,
			leftAnalogButton,
			bothAnalogButtons,
			buttonDownA,
			buttonDownB,
			buttonDownX,
			buttonDownY,
			rightAnalogButtonDown,
			leftAnalogButtonDown,
			leftMouse,
			rightMouse,
			middleMouse,
			leftMouseDown,
			rightMouseDown,
			middleMouseDown,
			leftMouseUp,
			rightMouseUp,
			middleMouseUp
		}
		
		public enum ControlSchemeAxis{
	
			rightAnalogX,
			rightAnalogY,
			rightAnalogMagnitude,
			leftAnalogX,
			leftAnalogY,
			leftAnalogMagnitude,
			rightTriggerAxis,
			leftTriggerAxis,
			rightTriggerAxis01,
			leftTriggerAxis01
		}
		
		public enum ControlSchemeVector{
	
			rightAnalog,
			leftAnalog,
			wasd,
			arrowKeys
		}
		
		public enum ControlSchemeScreenPosition{
	
			mousePosition
		}

        private static InputHandler preInstance;
		public static InputHandler Instance
	    {
	        get
	        {
	            if (instance == null)
	            {
                    if (preInstance == null)
                    {
                        GameObject go = new GameObject();
                        preInstance = go.AddComponent(new InputHandler());
                    }
                    return preInstance;
                    //Debug.LogError("Attempt to access instance of InputHandler singleton earlier than Start or without it being attached to a GameObject.");
	            }
	
	            return instance;
	        }
	    }
		
		public override void Awake()
	    {
			//Debug.Log("      ------GameHandler.Awake");
			
	        
	        if (instance != null)
	        {
	            Debug.LogError("Cannot have two instances of InputHandler. Self destruction in 3...");
	            //Destroy(instance);
	            return;
	        }
			isLoaded = true;
	        instance = this;
			
			//DontDestroyOnLoad(gameObject);
			
			lastUpdateTime = Time.realtimeSinceStartup;
	    }

        private bool GetControlSchemeButton(ControlSchemeButton _scheme)
        {
            if (turnOff)
            {
                return false;
            }

            switch (_scheme)
            {

                case ControlSchemeButton.buttonA:
                    return Input.GetButton("ButtonA");

                case ControlSchemeButton.buttonB:
                    return Input.GetButton("ButtonB");

                case ControlSchemeButton.buttonX:
                    return Input.GetButton("ButtonX");

                case ControlSchemeButton.buttonY:
                    return Input.GetButton("ButtonY");

                case ControlSchemeButton.leftAnalogButton:
                    return Input.GetButton("LeftAnalogButton");

                case ControlSchemeButton.rightAnalogButton:
                    return Input.GetButton("RightAnalogButton");

                case ControlSchemeButton.leftTriggerAxisHalfWay:
                    return (leftTriggerAxis > 0);

                case ControlSchemeButton.leftTriggerAxisHairTrigger:
                    return (leftTriggerAxis > -0.8f);

                case ControlSchemeButton.rightTriggerAxisHalfWay:
                    return (rightTriggerAxis > 0);

                case ControlSchemeButton.rightTriggerAxisHairTrigger:
                    return (rightTriggerAxis > -0.8f);

                case ControlSchemeButton.leftTriggerButton:
                    return Input.GetButton("LeftTriggerButton");

                case ControlSchemeButton.rightTriggerButton:
                    return Input.GetButton("RightTriggerButton");

                case ControlSchemeButton.bothAnalogButtons:
                    return (Input.GetButton("LeftAnalogButton") && Input.GetButton("RightAnalogButton"));

                case ControlSchemeButton.buttonDownA:
                    return Input.GetButtonDown("ButtonA");

                case ControlSchemeButton.buttonDownB:
                    return Input.GetButtonDown("ButtonB");

                case ControlSchemeButton.buttonDownX:
                    return Input.GetButtonDown("ButtonX");

                case ControlSchemeButton.buttonDownY:
                    return Input.GetButtonDown("ButtonY");

                case ControlSchemeButton.leftAnalogButtonDown:
                    return Input.GetButtonDown("LeftAnalogButton");

                case ControlSchemeButton.rightAnalogButtonDown:
                    return Input.GetButtonDown("RightAnalogButton");

                case ControlSchemeButton.leftMouse:
                    return Input.GetMouseButton(0);

                case ControlSchemeButton.rightMouse:
                    return Input.GetMouseButton(1);

                case ControlSchemeButton.middleMouse:
                    return Input.GetMouseButton(2);

                case ControlSchemeButton.leftMouseDown:
                    return Input.GetMouseButtonDown(0);

                case ControlSchemeButton.rightMouseDown:
                    return Input.GetMouseButtonDown(1);

                case ControlSchemeButton.middleMouseDown:
                    return Input.GetMouseButtonDown(2);

                case ControlSchemeButton.leftMouseUp:
                    return Input.GetMouseButtonUp(0);

                case ControlSchemeButton.rightMouseUp:
                    return Input.GetMouseButtonUp(1);

                case ControlSchemeButton.middleMouseUp:
                    return Input.GetMouseButtonUp(2);
            }

            return false;
        }

        private float GetControlSchemeAxis(ControlSchemeAxis _scheme)
        {
            if (turnOff)
            {
                return 0;
            }

            switch (_scheme)
            {
                case ControlSchemeAxis.leftAnalogX:
                    return leftAnalogXAxis;

                case ControlSchemeAxis.leftAnalogY:
                    return leftAnalogYAxis;

                case ControlSchemeAxis.rightAnalogX:
                    return rightAnalogXAxis;

                case ControlSchemeAxis.rightAnalogY:
                    return rightAnalogYAxis;

                case ControlSchemeAxis.leftTriggerAxis:
                    return leftTriggerAxis;

                case ControlSchemeAxis.rightTriggerAxis:
                    return rightTriggerAxis;

                case ControlSchemeAxis.leftTriggerAxis01:
                    return (0.8f + leftTriggerAxis) / 1.8f;

                case ControlSchemeAxis.rightTriggerAxis01:
                    return (0.8f + rightTriggerAxis) / 1.8f; ;

                case ControlSchemeAxis.rightAnalogMagnitude:
                    return rightAnalogMagnitude;

                case ControlSchemeAxis.leftAnalogMagnitude:
                    return leftAnalogMagnitude;
            }

            return 0;
        }
		
		private Vector2 GetControlSchemeScreenPosition(ControlSchemeScreenPosition _scheme)
		{
			if (turnOff)
			{
				return Vector2.Zero;
			}
			
			switch(_scheme)
			{
				case ControlSchemeScreenPosition.mousePosition:
				return Input.mousePosition;
			}
			
			return Vector2.Zero;
		}
		
		
		private Vector2 GetControlSchemeVector(ControlSchemeVector _scheme)
		{
			if (turnOff)
			{
				return Vector2.Zero;
			}
			
			switch(_scheme)
			{
				case ControlSchemeVector.leftAnalog:
				return leftAnalogVector;
				
				case ControlSchemeVector.rightAnalog:
				return rightAnalogVector;
				
				case ControlSchemeVector.wasd:
				return wasdVector;
				
				case ControlSchemeVector.arrowKeys:	
				return arrowKeysVector;
			}
			
			return Vector2.Zero;
		}
		
		
		// Update is called once per frame
		public override void Update()
		{
			if (turnOff)
			{
				return;
			}

            Input.Update();
            DoUpdate();
		}
		
		/*public void FixedUpdate()
		{
			DoUpdate();
		}*/
		
		public void DoUpdate () 
		{
			dTime = Time.realtimeSinceStartup - lastUpdateTime;
			
			lastUpdateTime = Time.realtimeSinceStartup;
			
			//get input vector list
			if (!collectInputFlickVectors)
			{
				if (GetOneButtonActive(inputFlickActivationScheme))
				{
					inputList.Clear();
					inputDifferenceList.Clear();
					collectInputFlickVectors = true;
				}
			}
			
			if (collectInputFlickVectors)
			{
				inputList.Add(GetInputScreenPosition());
				
				if (!GetOneButtonActive(inputFlickActivationScheme))
				{
					collectInputFlickVectors = false;
				}
			}
			
			//right analog input vector handling
			rightAnalogXAxis = Input.GetAxis("RightAnalogX");
			rightAnalogYAxis = -Input.GetAxis("RightAnalogY");
			rightAnalogVector.X = rightAnalogXAxis;
			rightAnalogVector.Y = rightAnalogYAxis;
			rightAnalogMagnitude = rightAnalogVector.Length();
			rightAnalogMagnitude = (rightAnalogMagnitude - rightAnalogDeadZone)/(1-rightAnalogDeadZone);
			if (rightAnalogMagnitude<0)
			{
				rightAnalogMagnitude = 0;
				rightAnalogVector.X = 0;
				rightAnalogVector.Y = 0;
			}	
			if (rightAnalogMagnitude > 0)
			{
				rightAnalogVector /= rightAnalogMagnitude;
			}
			rightAnalogMagnitude = Mathf.Pow(rightAnalogMagnitude,rightAnalogCurvePow);
			rightAnalogVector *= rightAnalogMagnitude;
			
			//left analog input vector handling
			leftAnalogXAxis = Input.GetAxis("LeftAnalogX");
			leftAnalogYAxis = -Input.GetAxis("LeftAnalogY");
			leftAnalogVector.X = leftAnalogXAxis;
			leftAnalogVector.Y = leftAnalogYAxis;
			leftAnalogMagnitude = leftAnalogVector.Length();
			leftAnalogMagnitude = (leftAnalogMagnitude - leftAnalogDeadZone)/(1-leftAnalogDeadZone);
			if (leftAnalogMagnitude<0)
			{
				leftAnalogMagnitude = 0;
				leftAnalogVector.X = 0;
				leftAnalogVector.Y = 0;
			}	
			if (leftAnalogMagnitude > 0)
			{
				leftAnalogVector /= leftAnalogMagnitude;
			}
			leftAnalogMagnitude = Mathf.Pow(leftAnalogMagnitude, leftAnalogCurvePow);
			leftAnalogVector *= leftAnalogMagnitude;
			
			//Debug.Log("leftAnalogVector : "+leftAnalogVector);
			
			/*//wasd vector handling
			wasdXAxis = Input.GetAxis("Horizontal");
			wasdYAxis = Input.GetAxis("Vertical");
			wasdVector.X = wasdXAxis;
			wasdVector.Y = wasdYAxis;
			
			//arrow keys vector handling
			arrowKeysXAxis = Input.GetAxis("Horizontal");
			arrowKeysYAxis = Input.GetAxis("Vertical");
			arrowKeysVector.X = wasdXAxis;
			arrowKeysVector.Y = wasdYAxis;*/
			
			//trigger axis handling
			leftTriggerAxis = Mathf.Pow(Input.GetAxis("LeftTriggerAxis"),leftAnalogCurvePow);
			rightTriggerAxis = Mathf.Pow(Input.GetAxis("RightTriggerAxis"),rightAnalogCurvePow); //(0.8f + Input.GetAxis("RightTriggerAxis"))/1.8f;
			
			lastInputPosition = inputPosition;
			inputPosition = GetControlSchemeScreenPosition(inputPositionScheme);
		}
		
		private Vector2 GetLongestVectorScheme(ControlSchemeVector[] _schemes)
		{
			if (_schemes.Length == 0)
			{
				return Vector2.Zero;
			}
			
			Vector2 _longestVec = GetControlSchemeVector(_schemes[0]);
	
			for (int i = 1; i < _schemes.Length; i++) {
				if (GetControlSchemeVector(_schemes[i]).Length() > _longestVec.Length())	
				{
					_longestVec = GetControlSchemeVector(_schemes[i]);
				}
			}
			
			return _longestVec;
		}
		
		private float GetHighestAxisScheme(ControlSchemeAxis[] _schemes)
		{
			if (_schemes.Length == 0)
			{
				return 0;
			}
			
			float _highest = GetControlSchemeAxis(_schemes[0]);
	
			for (int i = 1; i < _schemes.Length; i++) {
				if (GetControlSchemeAxis(_schemes[i]) > _highest)	
				{
					_highest = GetControlSchemeAxis(_schemes[i]);
				}
			}
			
			return _highest;
		}
		
		private bool GetOneButtonActive(ControlSchemeButton[] _schemes)
		{
			for (int i = 0; i < _schemes.Length; i++) {
				if (GetControlSchemeButton(_schemes[i]))
				{
					return true;			
				}
			}
			return false;
		}
		
		public bool GetKeepConnection()
		{
			return GetOneButtonActive(keepConnectionScheme);
		}
		
		public bool GetShootTentacle()
		{
			return GetOneButtonActive(shootTentacleScheme);
		}
		
		public bool GetShootClaw()
		{
			return GetOneButtonActive(shootClawScheme);
		}
		
		public bool GetControlTentacle()
		{
			return GetOneButtonActive(controlTentacleScheme);
		}
		
		public Vector2 GetInputScreenPositionDifference()
		{
			return inputPosition - lastInputPosition;
		}
		
		public Vector2 GetInputScreenPosition()
		{
            return inputPosition;
		}

        public List<Vector2> GetInputArraylist()
		{
			return inputList;
		}
		
		public Vector2 GetTentacleDirection()
		{
			return GetControlSchemeVector(tentacleDirectionScheme);
		}
		
		public Vector2 GetClawDirection()
		{
			return GetControlSchemeVector(clawDirectionScheme);
		}
		
		
		public static Vector3 InputVecToWorldVec(Camera _veiwingCam, Vector2 _inputVec)
		{
			Vector3 newVec = _inputVec.X * _veiwingCam.transform.right + _inputVec.Y * _veiwingCam.transform.up;
			
			//Debug.DrawRay(_veiwingCam.transform.position, newVec, Color.green);
			
			//Debug.Log("InputVecToWorldVec : "+_inputVec + " newVec : "+newVec);
			
			return newVec;
		}
		
		/// <summary>
		/// Screens the ray check.
		/// </summary>
		/// <returns>
		/// ScreenRayCheckHit.hitLayer determines if the raycast hit a collider in the correct layer. ScreenRayCheckHit.position is either the position of this hit, or else is the intersection of the ray with the input plane
		/// </returns>
		/// <param name='_ray'>
		/// _ray.
		/// </param>
		/// <param name='_layerMask'>
		/// _layer mask.
		/// </param>
		public ScreenRayCheckHit ScreenRayCheck(Ray _ray, LayerMask _layerMask)
		{
			screenInputPlane.Normal = Vector3.Up;

            ScreenRayCheckHit hit = new ScreenRayCheckHit();
            float _intersectionDist;
            screenInputPlane.Raycast(_ray, out _intersectionDist);

            hit.position = _ray.Position + _ray.Direction * _intersectionDist;
			
			RaycastHit rh;
            if (Physics.Pointcast(hit.position.To2d(), out rh, _layerMask))
            {
                hit.hitObjectInLayer = true;
                hit.position = rh.point;
                hit.obj = rh.collider.gameObject;
                return hit;
            }

            hit.hitObjectInLayer = false;
			return hit;
		}
	}
	
	public struct ScreenRayCheckHit{
		public bool hitObjectInLayer;
		public Vector3 position;
		public GameObject obj;
		public override string ToString()
		{
			return "hitObjectInLayer : " + hitObjectInLayer + " position : " + position;
		}
	}
	
}
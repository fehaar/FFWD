using System;
using System.Collections.Generic;
using System.Text;
using PressPlay.FFWD;
using PressPlay.FFWD.Components;

namespace PressPlay.Tentacles.Scripts {
	public class MainBody : MonoBehaviour {
        //public Renderer damageRendererOverlay;
        //public Material damageOverlay;

        public ObjectReference gfxTransform;

        Microsoft.Xna.Framework.Quaternion oldRotation;

        //public PPAnimationHandler anim;

        //private float healthFraction;

        //private Color damageOverlayNeutralColor;

        //private Color damageOverlayFullAlphaColor;// = new Color(1,1,1,1);
        //private Color damageOverlayNoAlphaColor;// = new Color(1,1,1,0);


        //public UVSpriteSheetAnimator bodySpriteAnim;

        private float stateLength;
        private float stateChangeTime;
        private State lastState;
        private State state;
        private State nextState;

        public bool isAnimatingTakingDamage
        {
            get
            {
                return (state == State.animatingTakingDamage);
            }
        }

        public enum State
        {
            neutral,
            excited,
            animatingTakingDamage,
            openMouth,
            chew,
            closeMouth
        }


        // Use this for initialization
        public override void Start()
        {
            LookRight();
            ChangeState(State.neutral);

            //damageOverlayFullAlphaColor = new Color(0.5f,0.5f,0.5f,1);
            //damageOverlayNoAlphaColor = new Color(0.5f,0.5f,0.5f,0);
        }

        void ChangeState(State _state)
        {
            //Debug.Log("Main Body Change State : "+_state);

            HandleExitState(_state, state);


            lastState = state;
            state = _state;
            stateChangeTime = Time.time;
            stateLength = -1;
            switch (state)
            {
                case State.neutral:
                    //anim.Play("idleCalm");
                    break;

                case State.animatingTakingDamage:
                    //anim.Play("takeDamage");
                    nextState = MainBody.State.neutral;
                    stateLength = 0.8f;
                    break;

                case State.openMouth:
                    //anim.Play("eatStart", null);
                    //nextState = MainBody.State.chew;
                    //stateLength = 0.2f;

                    break;

                case State.chew:
                    //anim.Play("eatChew", AnimationDoneCallback);
                    nextState = MainBody.State.closeMouth;
                    //stateLength = 0.75f;
                    break;

                case State.closeMouth:
                    //anim.Play("eatReturn", AnimationDoneCallback);
                    nextState = MainBody.State.neutral;

                    //stateLength = 0.5f;
                    break;
            }
        }

        void AnimationDoneCallback()
        {
            ChangeState(nextState);
        }


        void HandleExitState(State _oldState, State _newState)
        {
            switch (_oldState)
            {
                case State.neutral:
                    break;

                case State.animatingTakingDamage:
                    //damageOverlay.SetColor("_Color", damageOverlayNeutralColor);
                    //damageRendererOverlay.materials[1].SetColor("_Color", damageOverlayNeutralColor);

                    break;

                case State.openMouth:

                    break;

                case State.chew:

                    break;

                case State.closeMouth:

                    break;
            }
        }

        void UpdateState()
        {
            if (stateLength != -1 && Time.time > stateChangeTime + stateLength)
            {
                ChangeState(nextState);
            }


            switch (state)
            {
                case State.neutral:
                    break;
                case State.animatingTakingDamage:
                    //damageOverlay.SetColor("_Color", Color.Lerp(damageOverlayFullAlphaColor, damageOverlayNeutralColor, (Time.time - stateChangeTime) / stateLength));
                    //damageRendererOverlay.materials[1].SetColor("_Color", Color.Lerp(damageOverlayFullAlphaColor, damageOverlayNeutralColor, (Time.time - stateChangeTime) / stateLength));

                    if (Time.time > stateChangeTime + stateLength)
                    {
                        //damageOverlay.SetColor("_Color", damageOverlayNeutralColor);
                        //damageRendererOverlay.materials[1].SetColor("_Color", damageOverlayNeutralColor);
                        ChangeState(nextState);
                    }

                    break;
            }
        }

        // Update is called once per frame
        public override void FixedUpdate()
        {


            oldRotation = transform.rotation;
            //transform.LookAt(transform.position + LevelHandler.Instance.cam.GetForwardDirection());

            //transform.rotation = Quaternion.Lerp(oldRotation, transform.rotation, Time.deltaTime * 2f);

            UpdateState();
        }

        public void LookUp()
        {
            //gfxTransform.localEulerAngles = Vector3.zero;
        }

        public void LookRight()
        {
            //gfxTransform.localEulerAngles = Vector3.right * 30;
        }

        public void SetHealthFraction(float _fraction)
        {
            //healthFraction = _fraction;

            //damageOverlayNeutralColor = Color.Lerp(damageOverlayFullAlphaColor,damageOverlayNoAlphaColor, Mathf.Pow(_fraction,2f));
            //bodySpriteAnim.Play("Damage");
            //bodySpriteAnim.SetCurrentAnimFraction(Mathf.Pow(1 - _fraction, 0.7f));
            //bodySpriteAnim.Stop();

            if (!isAnimatingTakingDamage)
            {
                //damageOverlay.color = damageOverlayNeutralColor;
                //damageOverlay.SetColor("_Color", damageOverlayNeutralColor);
                //damageRendererOverlay.materials[1].SetColor("_Color", damageOverlayNeutralColor);
            }
            //damageOverlay.color = new Color(damageOverlay.color.r,damageOverlay.color.g,damageOverlay.color.b,1-_fraction);
        }

        public void StartTakeDamageAnimation()
        {
            ChangeState(State.animatingTakingDamage);
        }

        public void OpenMouth()
        {
            if (state != MainBody.State.openMouth && state != MainBody.State.chew)
            {
                ChangeState(State.openMouth);
            }
        }

        public void Chew()
        {
            ChangeState(State.chew);
        }
    }
}
using Microsoft.Xna.Framework.Content;
using PressPlay.FFWD;

namespace PressPlay.Tentacles.Scripts {
    public class CheckPoint : TriggeredByLemmy
    {
        [ContentSerializer(Optional = true)]
        public PathFollowCamNode connectionedNode { get; set; }

        //public PathFollowCamNode connectionedNode;
        public bool start = false;

        public int noDamageBonus = 0;
        public bool startDamageChallenge = false;

        public bool startChallenge = false;
        //public Challenge challenge;

        private bool _isCheckPointActive = false;
        public bool isCheckPointActive
        {
            get { return _isCheckPointActive; }
        }

        protected override void DoOnTrigger()
        {
            if (start)
            {
                return;
            }

            ActivateCheckPoint();
        }

        public void ActivateCheckPoint()
        {
            if (isCheckPointActive)
            {
                return;
            }

            //Debug.Log("--------------------------------ACTIVATING CHECKPOINT");

            _isCheckPointActive = true;
            LevelHandler.Instance.ActivateCheckpoint(this);

            //if (startDamageChallenge)
            //{
            //    LevelHandler.Instance.feedback.Show(LevelHandler.Instance.library.ingameGUIText, Vector3.zero, Quaternion.identity, Color.green + "Damage Challenge Start");
            //}

            DoOnActivate();
        }

        public virtual Vector3 GetSpawnPosition()
        {
            return transform.position;
        }

        public virtual void DoOnActivate()
        {

        }

        public virtual void DoOnSpawnLemmy()
        {

        }
    }
}
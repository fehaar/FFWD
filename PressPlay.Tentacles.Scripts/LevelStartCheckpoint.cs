using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PressPlay.FFWD;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace PressPlay.Tentacles.Scripts
{
    public class LevelStartCheckPoint : CheckPoint
    {
        // TODO: We need a more elegant solution than this. IE. a TransformReference that works like the object reference.
        private Transform _lemmyStartPosition;
        [ContentSerializerIgnore]
        public Transform lemmyStartPosition
        {
            get
            {
                if (_lemmyStartPosition == null)
                {
                    _lemmyStartPosition = Application.Instance.Find(lemmyStartPositionHelper).transform;
                }
                return _lemmyStartPosition;
            }
            set
            {
                _lemmyStartPosition = value;
            }
        }

        [ContentSerializer(ElementName = "lemmyStartPosition")]
        private int lemmyStartPositionHelper = 0;

        public Ease lemmyMoveEase = Ease.EaseCircOut;

        public Vector3 GetStartTweenPosition(float _moveFraction)
        {
            float easedFraction = Equations.ChangeFloat(_moveFraction, 0, 1, 1, lemmyMoveEase);

            return Vector3.Lerp(lemmyStartPosition.position, transform.position, easedFraction);
        }

        public override Vector3 GetSpawnPosition()
        {
            return base.GetSpawnPosition();
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PressPlay.FFWD;
using Microsoft.Xna.Framework.Content;

namespace PressPlay.Tentacles.Scripts
{
    public class LevelStartCheckPoint : CheckPoint
    {
        [ContentSerializer(SharedResource = true)]
        public Transform lemmyStartPosition;

        public Ease lemmyMoveEase = Ease.EaseCircOut;

        public Vector3 GetStartTweenPosition(float _moveFraction)
        {
            float easedFraction = Equations.ChangeFloat(_moveFraction, 0, 1, 1, lemmyMoveEase);
            // TODO: The code should be as follows as lemmyStartPosition should be a Transform
            //return Vector3.Lerp(lemmyStartPosition.position, transform.position, easedFraction);
            return Vector3.Lerp(lemmyStartPosition.gameObject.transform.position, transform.position, easedFraction);
        }

        public override Vector3 GetSpawnPosition()
        {
            return base.GetSpawnPosition();
        }

    }
}

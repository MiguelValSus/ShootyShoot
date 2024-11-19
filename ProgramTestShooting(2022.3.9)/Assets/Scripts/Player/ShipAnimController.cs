using UnityEngine;

namespace Player
{
    public class ShipAnimController : MonoBehaviour
    {
        [Header("Animations")]
        public Animator ShipAnimator;
        public Animator ShadowAnimator;

        [System.Serializable]
        public class ShipAnims
        {
            public string name;
            public RuntimeAnimatorController controller;
        }
        public ShipAnims[] Animations;

        #region Initialization
        private void Awake()
        {
            Play("Ship_Idle");
        }

        private RuntimeAnimatorController GetAnimControllerByName(string contName)
        {
            for (var a = 0; a < Animations.Length; ++a)
            {
                var animation = Animations[a];
                if (animation.name == contName)
                    return animation.controller;
            }
            return null;
        }
        #endregion

        #region Animation playback
        public void Play(string animName)
        {
            var animationController = GetAnimControllerByName(animName);
            if (animationController == null) return;
            ShipAnimator.runtimeAnimatorController = animationController;
            ShadowAnimator.runtimeAnimatorController = animationController;
            ShipAnimator.Play(animName);
            ShadowAnimator.Play(animName);
        }
        #endregion
    }
}
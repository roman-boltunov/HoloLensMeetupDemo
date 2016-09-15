namespace Assets.Scripts
{
    using JetBrains.Annotations;

    using UnityEngine;

    /// <summary>
    /// The kitten emotions.
    /// </summary>
    [RequireComponent(typeof(Animation))]
    public class KittenEmotions : MonoBehaviour
    {
        /// <summary>
        /// The itching emotion name.
        /// </summary>
        private const string ItchingEmotionName = "Ithcing";

        /// <summary>
        /// The meow emotion name.
        /// </summary>
        private const string MeowEmotionName = "Meow";

        /// <summary>
        /// The idle sit emotion name.
        /// </summary>
        private const string IdleSitEmotionName = "IdleSit";

        /// <summary>
        /// The start.
        /// </summary>
        [UsedImplicitly]
        private void Start()
        {
            var animationComponent = this.GetComponent<Animation>();

            animationComponent[KittenEmotions.ItchingEmotionName].layer = 1;
            animationComponent[KittenEmotions.ItchingEmotionName].wrapMode = WrapMode.Once;
            animationComponent[KittenEmotions.IdleSitEmotionName].layer = 1;
            animationComponent[KittenEmotions.IdleSitEmotionName].wrapMode = WrapMode.Once;
            animationComponent[KittenEmotions.MeowEmotionName].layer = 1;
            animationComponent[KittenEmotions.MeowEmotionName].wrapMode = WrapMode.Once;
        }

        /// <summary>
        /// The update.
        /// </summary>
        [UsedImplicitly]
        private void Update()
        {
            var gamepad = GamepadClientSingleton.Instance.GamepadClient;

            if (gamepad == null)
            {
                return;
            }

            if (gamepad.controllers[0].button_shoulder_L)
            {
                this.GetComponent<Animation>().Play(KittenEmotions.ItchingEmotionName);
            }

            if (gamepad.controllers[0].button_B)
            {
                this.GetComponent<Animation>().Play(KittenEmotions.MeowEmotionName);
            }
        }
    }
}

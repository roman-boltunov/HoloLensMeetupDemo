namespace Assets.Scripts
{
    using JetBrains.Annotations;

    using UnityEngine;

    /// <summary>
    /// The kitten emotions.
    /// </summary>
    public class KittenEmotions : MonoBehaviour
    {
        /// <summary>
        /// The itching emotion name.
        /// </summary>
        private const string ItchingEmotionName = "Itching";

        /// <summary>
        /// The meow emotion name.
        /// </summary>
        private const string MeowEmotionName = "Meow";

        /// <summary>
        /// The start.
        /// </summary>
        [UsedImplicitly]
        private void Start()
        {
            this.GetComponent<Animation>()[KittenEmotions.ItchingEmotionName].layer = 1;
            this.GetComponent<Animation>()[KittenEmotions.ItchingEmotionName].wrapMode = WrapMode.Once;
            this.GetComponent<Animation>()[KittenEmotions.MeowEmotionName].layer = 1;
            this.GetComponent<Animation>()[KittenEmotions.MeowEmotionName].wrapMode = WrapMode.Once;
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
                GetComponent<Animation>().Play(KittenEmotions.ItchingEmotionName);
            }

            if (gamepad.controllers[0].button_B)
            {
                GetComponent<Animation>().Play(KittenEmotions.MeowEmotionName);
            }
        }
    }
}

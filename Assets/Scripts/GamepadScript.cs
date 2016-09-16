namespace Assets.Scripts
{
    using HoloToolkit.Unity;

    using JetBrains.Annotations;

    using UnityEngine;

    /// <summary>
    /// The spawn player.
    /// </summary>
    public class GamepadScript : MonoBehaviour
    {
        #region Fields

        /// <summary>
        /// The mesh switch timeout.
        /// </summary>
        public float meshSwitchTimeout = 0.25f;

        /// <summary>
        /// The player.
        /// </summary>
        private GameObject player;

        /// <summary>
        /// The last time switched.
        /// </summary>
        private float lastTimeSwitched;

        /// <summary>
        /// The gamepad.
        /// </summary>
        private Gamepad_Client gamepad;

        /// <summary>
        /// The controller ID.
        /// </summary>
        private int controllerId;

        #endregion

        #region Other Methods

        /// <summary>
        /// The update.
        /// </summary>
        [UsedImplicitly]
        private void Update()
        {
            this.gamepad = GamepadClientSingleton.Instance.GamepadClient;

            // X button spawns player
            if (this.gamepad && this.gamepad.controllers[this.controllerId].button_X)
            {
                if (this.player == null)
                {
                    return;
                }

                this.player.SetActive(true);
#if UNITY_EDITOR
                this.player.transform.position = Camera.main.transform.forward;
                this.player.transform.position = GazeManager.Instance.HitInfo.point;
#else
                this.player.transform.position = GazeManager.Instance.HitInfo.point;
#endif
            }

            // Y button switches mesh
            if (this.gamepad && this.gamepad.controllers[this.controllerId].button_Y && Time.time > this.lastTimeSwitched + this.meshSwitchTimeout)
            {
                this.lastTimeSwitched = Time.time;
                SpatialMappingManager.Instance.DrawVisualMeshes = !SpatialMappingManager.Instance.DrawVisualMeshes;
            }
        }

        /// <summary>
        /// The start.
        /// </summary>
        [UsedImplicitly]
        private void Start()
        {
            this.player = GameObject.FindGameObjectWithTag("Player");
            this.player.SetActive(false);
            this.gamepad = GamepadClientSingleton.Instance.GamepadClient;
            this.controllerId = 0;
        }

        #endregion
    }
}

namespace Assets
{
    using HoloToolkit.Unity;

    using JetBrains.Annotations;

    using UnityEngine;

    /// <summary>
    /// The spawn player.
    /// </summary>
    public class PlayerScript : MonoBehaviour
    {
        #region Fields

        /// <summary>
        /// The player.
        /// </summary>
        public GameObject player;

        /// <summary>
        /// The mesh switch timeout.
        /// </summary>
        public float meshSwitchTimeout = 0.25f;

        /// <summary>
        /// The last time switched.
        /// </summary>
        private float lastTimeSwitched;

        /// <summary>
        /// The gamepad.
        /// </summary>
        private Gamepad_Client gamepad;

        /// <summary>
        /// The is spawned.
        /// </summary>
        private bool isSpawned;

        /// <summary>
        /// The controller ID.
        /// </summary>
        private int controllerId;

        #endregion

        #region Other Methods

        /// <summary>
        /// The fixed update.
        /// </summary>
        [UsedImplicitly]
        private void FixedUpdate()
        {
            this.gamepad = GamepadClientSingleton.Instance.GamepadClient;

            // X button spawns player
            if (this.gamepad && this.gamepad.controllers[this.controllerId].button_X && !this.isSpawned)
            {
                UnityEngine.Object.Instantiate(this.player, Camera.main.transform.forward, Quaternion.identity);
                this.isSpawned = true;
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
            this.gamepad = GamepadClientSingleton.Instance.GamepadClient;
            this.controllerId = 0;
        }

        #endregion
    }
}
namespace Assets
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
        /// The spawned player.
        /// </summary>
        private GameObject spawnedPlayer;

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

                

#if UNITY_EDITOR
                this.spawnedPlayer = this.spawnedPlayer ?? (GameObject)UnityEngine.Object.Instantiate(this.player, Camera.main.transform.forward, Quaternion.identity);
#else
                if (this.spawnedPlayer != null)
                {
                    this.spawnedPlayer.transform.position = GazeManager.Instance.HitInfo.point;
                    return;
                }

                this.spawnedPlayer = (GameObject)UnityEngine.Object.Instantiate(this.player, GazeManager.Instance.HitInfo.point, Quaternion.identity);
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
            this.gamepad = GamepadClientSingleton.Instance.GamepadClient;
            this.controllerId = 0;
        }

        #endregion
    }
}

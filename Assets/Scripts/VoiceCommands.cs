namespace Assets.Scripts
{
    using UnityEngine;

    /// <summary>
    /// The voice commands.
    /// </summary>
    public class VoiceCommands : MonoBehaviour
    {
        /// <summary>
        /// The move player.
        /// </summary>
        public void MakeMeow()
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            player.GetComponent<Animation>().Play("Meow");
            player.GetComponent<AudioSource>().Play();
        }

        /// <summary>
        /// The play sit animation.
        /// </summary>
        public void PlayerSit()
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            player.GetComponent<Animation>().Play("IdleSit");
        }
    }
}

using UnityEngine;

[RequireComponent(typeof(Gamepad_Client))]
[RequireComponent(typeof(Rigidbody))]
public class SimpleCharacterController : MonoBehaviour
{
    #region Fields

    public float jumpSpeed = 20f;

    public float speed = 10f;

    private Gamepad_Client gamepad;

    private bool isFalling = true;

    private Rigidbody rigid_body;

    #endregion

    #region Other Methods

    // when we touch something we're not falling anymore
    private void OnCollisionStay(Collision collisionInfo)
    {
        isFalling = false;
    }

    private void Start()
    {
        gamepad = GetComponent<Gamepad_Client>();
        rigid_body = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (gamepad && rigid_body)
        {
            // target the first gamepad found
            var id = 0;

            // left thumb stick steers
            /*var velocity = rigid_body.velocity;
            velocity.x = speed * gamepad.controllers[id].thumb_Lx;
            velocity.z = speed * gamepad.controllers[id].thumb_Ly;
            rigid_body.velocity = velocity;*/

            // A button jumps
            if (gamepad.controllers[id].button_A && !isFalling)
            {
                
                // rigid_body.AddForce(Vector3.up * jumpSpeed, ForceMode.Impulse);
                isFalling = true;
            }

            // Triggers set vibration
            gamepad.SetVibrate(id, gamepad.controllers[id].trigger_L, gamepad.controllers[id].trigger_R);
        }
    }

    #endregion
}
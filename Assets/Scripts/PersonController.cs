using JetBrains.Annotations;

using UnityEngine;

namespace Assets.Scripts
{
    [RequireComponent(typeof(CharacterController))]
    public class PersonController : MonoBehaviour
    {

        public AnimationClip idleAnimation;

        public AnimationClip walkAnimation;

        public AnimationClip runAnimation;

        public AnimationClip jumpPoseAnimation;

        public float walkMaxAnimationSpeed = 0.75f;

        public float trotMaxAnimationSpeed = 1.0f;

        public float runMaxAnimationSpeed = 1.0f;

        public float jumpAnimationSpeed = 1.15f;

        public float landAnimationSpeed = 1.0f;

        private Animation _animation;

        private enum CharacterState
        {
            Idle = 0,

            Walking = 1,

            Trotting = 2,

            Running = 3,

            Jumping = 4,
        }

        private CharacterState _characterState;

        // The speed when walking
        public float walkSpeed = 1.0f;

        // after trotAfterSeconds of walking we trot with trotSpeed
        public float trotSpeed = 1.5f;

        // when pressing "Fire3" button (cmd) we start running
        public float runSpeed = 2f;

        public float inAirControlAcceleration = 1.0f;

        // How high do we jump when pressing jump and letting go immediately
        public float jumpHeight = 1f;

        // The gravity for the character
        public float gravity = 5.0f;

        // The gravity in controlled descent mode
        public float speedSmoothing = 10.0f;

        public float rotateSpeed = 500.0f;

        public float trotAfterSeconds = 3.0f;

        public bool canJump = true;

        private float jumpRepeatTime = 0.05f;

        private float jumpTimeout = 0.15f;

        private float groundedTimeout = 0.25f;

        // The camera doesnt start following the target immediately but waits for a split second to avoid too much waving around.
        private float lockCameraTimer = 0.0f;

        // The current move direction in x-z
        private Vector3 moveDirection = Vector3.zero;

        // The current vertical speed
        private float verticalSpeed = 0.0f;

        // The current x-z move speed
        private float moveSpeed = 0.0f;

        // The last collision flags returned from controller.Move
        private CollisionFlags collisionFlags;

        // Are we jumping? (Initiated with jump button and not grounded yet)
        private bool jumping = false;

        private bool jumpingReachedApex = false;

        // Are we moving backwards (This locks the camera to not do a 180 degree spin)
        private bool movingBack = false;

        // Is the user pressing any keys?
        private bool isMoving = false;

        // When did the user start walking (Used for going into trot after a while)
        private float walkTimeStart = 0.0f;

        // Last time the jump button was clicked down
        private float lastJumpButtonTime = -10.0f;

        // Last time we performed a jump
        private float lastJumpTime = -1.0f;


        // the height we jumped from (Used to determine for how long to apply extra jump power after jumping.)
        private float lastJumpStartHeight = 0.0f;


        private Vector3 inAirVelocity = Vector3.zero;

        private float lastGroundedTime = 0.0f;

        private bool isControllable = true;

        [UsedImplicitly]
        private void Awake()
        {
            this.moveDirection = this.transform.TransformDirection(Vector3.forward);

            this._animation = this.GetComponent<Animation>();
            if (!this._animation) Debug.Log("The character you would like to control doesn't have animations. Moving her might look weird.");

            /*
    public var idleAnimation : AnimationClip;
    public var walkAnimation : AnimationClip;
    public var runAnimation : AnimationClip;
    public var jumpPoseAnimation : AnimationClip;   
        */
            if (!this.idleAnimation)
            {
                this._animation = null;
                Debug.Log("No idle animation found. Turning off animations.");
            }
            if (!this.walkAnimation)
            {
                this._animation = null;
                Debug.Log("No walk animation found. Turning off animations.");
            }
            if (!this.runAnimation)
            {
                this._animation = null;
                Debug.Log("No run animation found. Turning off animations.");
            }
            if (!this.jumpPoseAnimation && this.canJump)
            {
                this._animation = null;
                Debug.Log("No jump animation found and the character has canJump enabled. Turning off animations.");
            }

        }

        private void UpdateSmoothedMovementDirection()
        {
            var cameraTransform = Camera.main.transform;
            var grounded = this.IsGrounded();

            // Forward vector relative to the camera along the x-z plane    
            var forward = cameraTransform.TransformDirection(Vector3.forward);
            forward.y = 0;
            forward = forward.normalized;

            // Right vector relative to the camera
            // Always orthogonal to the forward vector
            var right = new Vector3(forward.z, 0, -forward.x);

            // var v = Input.GetAxis("Vertical");
            // var h = Input.GetAxis("Horizontal");

            var gamepad = GamepadClientSingleton.Instance.GamepadClient;
            var v = gamepad.controllers[0].thumb_Ly;
            var h = gamepad.controllers[0].thumb_Lx;

            // Are we moving backwards or looking backwards
            if (v < -0.2) this.movingBack = true;
            else this.movingBack = false;

            var wasMoving = this.isMoving;
            this.isMoving = Mathf.Abs(h) > 0.1 || Mathf.Abs(v) > 0.1;

            // Target direction relative to the camera
            var targetDirection = h * right + v * forward;

            // Grounded controls
            if (grounded)
            {
                // Lock camera for short period when transitioning moving & standing still
                this.lockCameraTimer += Time.deltaTime;
                if (this.isMoving != wasMoving) this.lockCameraTimer = 0.0f;

                // We store speed and direction seperately,
                // so that when the character stands still we still have a valid forward direction
                // moveDirection is always normalized, and we only update it if there is user input.
                if (targetDirection != Vector3.zero)
                {
                    // If we are really slow, just snap to the target direction
                    if (this.moveSpeed < this.walkSpeed * 0.9 && grounded)
                    {
                        this.moveDirection = targetDirection.normalized;
                    }
                    // Otherwise smoothly turn towards it
                    else
                    {
                        this.moveDirection = Vector3.RotateTowards(
                            this.moveDirection,
                            targetDirection,
                            this.rotateSpeed * Mathf.Deg2Rad * Time.deltaTime,
                            1000);

                        this.moveDirection = this.moveDirection.normalized;
                    }
                }

                // Smooth the speed based on the current target direction
                var curSmooth = this.speedSmoothing * Time.deltaTime;

                // Choose target speed
                //* We want to support analog input but make sure you cant walk faster diagonally than just forward or sideways
                var targetSpeed = Mathf.Min(targetDirection.magnitude, 1.0f);

                this._characterState = CharacterState.Idle;

                // Pick speed modifier
                if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift) || gamepad.controllers[0].button_shoulder_R)
                {
                    targetSpeed *= this.runSpeed;
                    this._characterState = CharacterState.Running;
                }
                else if (Time.time - this.trotAfterSeconds > this.walkTimeStart)
                {
                    targetSpeed *= this.trotSpeed;
                    this._characterState = CharacterState.Trotting;
                }
                else
                {
                    targetSpeed *= this.walkSpeed;
                    this._characterState = CharacterState.Walking;
                }

                this.moveSpeed = Mathf.Lerp(this.moveSpeed, targetSpeed, curSmooth);

                // Reset walk time start when we slow down
                if (this.moveSpeed < this.walkSpeed * 0.3) this.walkTimeStart = Time.time;
            }
            // In air controls
            else
            {
                // Lock camera while in air
                if (this.jumping) this.lockCameraTimer = 0.0f;

                if (this.isMoving) this.inAirVelocity += targetDirection.normalized * Time.deltaTime * this.inAirControlAcceleration;
            }
        }

        private void ApplyJumping()
        {
            // Prevent jumping too fast after each other
            if (this.lastJumpTime + this.jumpRepeatTime > Time.time) return;

            if (this.IsGrounded())
            {
                // Jump
                // - Only when pressing the button down
                // - With a timeout so you can press the button slightly before landing     
                if (this.canJump && Time.time < this.lastJumpButtonTime + this.jumpTimeout)
                {
                    this.verticalSpeed = CalculateJumpVerticalSpeed(this.jumpHeight);
                    this.SendMessage("DidJump", SendMessageOptions.DontRequireReceiver);
                }
            }
        }


        private void ApplyGravity()
        {
            if (this.isControllable) // don't move player at all if not controllable.
            {
                // Apply gravity
                var jumpButton = Input.GetButton("Jump");


                // When we reach the apex of the jump we send out a message
                if (this.jumping && !this.jumpingReachedApex && this.verticalSpeed <= 0.0)
                {
                    this.jumpingReachedApex = true;
                    this.SendMessage("DidJumpReachApex", SendMessageOptions.DontRequireReceiver);
                }

                if (this.IsGrounded()) this.verticalSpeed = 0.0f;
                else this.verticalSpeed -= this.gravity * Time.deltaTime;
            }
        }

        private float CalculateJumpVerticalSpeed(float targetJumpHeight)
        {
            // From the jump height and gravity we deduce the upwards speed 
            // for the character to reach at the apex.
            return Mathf.Sqrt(2 * targetJumpHeight * this.gravity);
        }

        private void DidJump()
        {
            this.jumping = true;
            this.jumpingReachedApex = false;
            this.lastJumpTime = Time.time;
            this.lastJumpStartHeight = this.transform.position.y;
            this.lastJumpButtonTime = -10;

            this._characterState = CharacterState.Jumping;
        }

        private void Update()
        {

            if (!this.isControllable)
            {
                // kill all inputs if not controllable.
                Input.ResetInputAxes();
            }

            var gamepad = GamepadClientSingleton.Instance.GamepadClient;

            if (Input.GetButtonDown("Jump") || gamepad.controllers[0].button_A)
            {
                this.lastJumpButtonTime = Time.time;
            }

            this.UpdateSmoothedMovementDirection();

            // Apply gravity
            // - extra power jump modifies gravity
            // - controlledDescent mode modifies gravity
            this.ApplyGravity();

            // Apply jumping logic
            this.ApplyJumping();

            // Calculate actual motion
            var movement = this.moveDirection * this.moveSpeed + new Vector3(0, this.verticalSpeed, 0) + this.inAirVelocity;
            movement *= Time.deltaTime;

            // Move the controller
            var controller =  this.GetComponent<CharacterController>();
            this.collisionFlags = controller.Move(movement);

            // ANIMATION sector
            if (this._animation)
            {
                if (this._characterState == CharacterState.Jumping)
                {
                    if (!this.jumpingReachedApex)
                    {
                        this._animation[this.jumpPoseAnimation.name].speed = this.jumpAnimationSpeed;
                        this._animation[this.jumpPoseAnimation.name].wrapMode = WrapMode.ClampForever;
                        this._animation.CrossFade(this.jumpPoseAnimation.name);
                    }
                    else
                    {
                        this._animation[this.jumpPoseAnimation.name].speed = -this.landAnimationSpeed;
                        this._animation[this.jumpPoseAnimation.name].wrapMode = WrapMode.ClampForever;
                        this._animation.CrossFade(this.jumpPoseAnimation.name);
                    }
                }
                else
                {
                    if (controller.velocity.sqrMagnitude < 0.1)
                    {
                        this._animation.CrossFade(this.idleAnimation.name);
                    }
                    else
                    {
                        if (this._characterState == CharacterState.Running)
                        {
                            this._animation[this.runAnimation.name].speed = Mathf.Clamp(
                                controller.velocity.magnitude,
                                0.0f,
                                this.runMaxAnimationSpeed);
                            this._animation.CrossFade(this.runAnimation.name);
                        }
                        else if (this._characterState == CharacterState.Trotting)
                        {
                            this._animation[this.walkAnimation.name].speed = Mathf.Clamp(
                                controller.velocity.magnitude,
                                0.0f,
                                this.trotMaxAnimationSpeed);
                            this._animation.CrossFade(this.walkAnimation.name);
                        }
                        else if (this._characterState == CharacterState.Walking)
                        {
                            this._animation[this.walkAnimation.name].speed = Mathf.Clamp(
                                controller.velocity.magnitude,
                                0.0f,
                                this.walkMaxAnimationSpeed);
                            this._animation.CrossFade(this.walkAnimation.name);
                        }

                    }
                }
            }
            // ANIMATION sector

            // Set rotation to the move direction
            if (this.IsGrounded())
            {

                this.transform.rotation = Quaternion.LookRotation(this.moveDirection);

            }
            else
            {
                var xzMove = movement;
                xzMove.y = 0;
                if (xzMove.sqrMagnitude > 0.001)
                {
                    this.transform.rotation = Quaternion.LookRotation(xzMove);
                }
            }

            // We are in jump mode but just became grounded
            if (this.IsGrounded())
            {
                this.lastGroundedTime = Time.time;
                this.inAirVelocity = Vector3.zero;
                if (this.jumping)
                {
                    this.jumping = false;
                    this.SendMessage("DidLand", SendMessageOptions.DontRequireReceiver);
                }
            }
        }

        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            //  Debug.DrawRay(hit.point, hit.normal);
            if (hit.moveDirection.y > 0.01) return;
        }

        private float GetSpeed()
        {
            return this.moveSpeed;
        }

        private bool IsJumping()
        {
            return this.jumping;
        }

        private bool IsGrounded()
        {
            return (this.collisionFlags & CollisionFlags.CollidedBelow) != 0;
        }

        private Vector3 GetDirection()
        {
            return this.moveDirection;
        }

        private bool IsMovingBackwards()
        {
            return this.movingBack;
        }

        private float GetLockCameraTimer()
        {
            return this.lockCameraTimer;
        }

        private bool IsMoving()
        {
            return Mathf.Abs(Input.GetAxisRaw("Vertical")) + Mathf.Abs(Input.GetAxisRaw("Horizontal")) > 0.5;
        }

        private bool HasJumpReachedApex()
        {
            return this.jumpingReachedApex;
        }

        private bool IsGroundedWithTimeout()
        {
            return this.lastGroundedTime + this.groundedTimeout > Time.time;
        }

        private void Reset()
        {
            this.gameObject.tag = "Player";
        }
    }
}

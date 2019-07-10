using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Text;

namespace MFPP
{
    [HelpURL("https://ashkoredracson.github.io/MFPP/#player")]
    [RequireComponent(typeof(CharacterController))]
    public class Player : MonoBehaviour
    {
        /// <summary>
        /// All of the modules attached to this <see cref="Player"/>.
        /// </summary>
        public IEnumerable<PlayerModule> Modules
        {
            get { return GetComponents<PlayerModule>(); }
        }

        [SerializeField, Space(10)]
        private MainSettings mainSettings;
        /// <summary>
        /// Main settings.
        /// </summary>
        public MainSettings Main
        {
            get { return mainSettings; }
            set { mainSettings = value; }
        }
        [SerializeField]
        private ControlSettings controlSettings;
        /// <summary>
        /// Control input settings.
        /// </summary>
        public ControlSettings Controls
        {
            get { return controlSettings; }
            set { controlSettings = value; }
        }
        [SerializeField]
        private CollisionSettings collisionSettings;
        /// <summary>
        /// Collision settings.
        /// </summary>
        public CollisionSettings Collisions
        {
            get { return collisionSettings; }
            set { collisionSettings = value; }
        }
        [SerializeField]
        private MovementSettings movementSettings;
        /// <summary>
        /// Character movement settings.
        /// </summary>
        public MovementSettings Movement
        {
            get { return movementSettings; }
            set { movementSettings = value; }
        }
        [SerializeField]
        private FootstepSettings footstepSettings;
        /// <summary>
        /// Footstep sounds settings.
        /// </summary>
        public FootstepSettings Footstep
        {
            get { return footstepSettings; }
            set { footstepSettings = value; }
        }

        // EVENTS
        /// <summary>
        /// Event delegate when the character just aired.
        /// </summary>
        public delegate void OnAiredHandler();
        /// <summary>
        /// Event delegate when the character just landed.
        /// </summary>
        /// <param name="velocity">The velocity at which the character just landed.</param>
        public delegate void OnLandedHandler(Vector3 velocity);
        /// <summary>
        /// Event delegate when the character just emitted a footstep.
        /// </summary>
        /// <param name="footstep">The footstep sound.</param>
        /// <param name="volume">The volume of the footstep.</param>
        /// <param name="pitch">The pitch of the footstep.</param>
        public delegate void OnFootstepHandler(AudioClip footstep, float volume, float pitch);
        /// <summary>
        /// Event delegate just before applying the final movement.
        /// </summary>
        /// <param name="finalMovement">The final movement vector.</param>
        public delegate void BeforeFinalMovementHandler(Vector3 finalMovement);
        /// <summary>
        /// Event delegate just after applying the final movement.
        /// </summary>
        /// <param name="finalMovement">The final movement vector.</param>
        public delegate void AfterFinalMovementHandler(Vector3 finalMovement);
        /// <summary>
        /// Event delegate when a collision with this <see cref="Player"/> is detected.
        /// </summary>
        /// <param name="hit">The collision hit info.</param>
        public delegate void CollisionDetectedHandler(ControllerColliderHit hit);
        /// <summary>
        /// Event when the character just aired.
        /// </summary>
        public event OnAiredHandler OnAired;
        /// <summary>
        /// Event when the character just landed.
        /// </summary>
        public event OnLandedHandler OnLanded;
        /// <summary>
        /// Event when the character just emitted a footstep.
        /// </summary>
        public event OnFootstepHandler OnFootstep;
        /// <summary>
        /// Event just before applying the final movement.
        /// </summary>
        public event BeforeFinalMovementHandler BeforeFinalMovement;
        /// <summary>
        /// Event just after applying the final movement.
        /// </summary>
        public event AfterFinalMovementHandler AfterFinalMovement;
        /// <summary>
        /// Event when a collision with this <see cref="Player"/> is detected.
        /// </summary>
        public event CollisionDetectedHandler CollisionDetected;

        // OTHER PROPERTIES
        /// <summary>
        /// The attached <see cref="UnityEngine.CharacterController"/> to this <see cref="Player"/>.
        /// </summary>
        public CharacterController CharacterController { get; private set; }

        /// <summary>
        /// The minimum move distance of this <see cref="CharacterController"/>, same as <see cref="UnityEngine.CharacterController.minMoveDistance"/>
        /// </summary>
        public float MinMoveDistance
        {
            get { return CharacterController.minMoveDistance; }
            set { CharacterController.minMoveDistance = value; }
        }
        /// <summary>
        /// The radius of this <see cref="CharacterController"/>, same as <see cref="UnityEngine.CharacterController.radius"/>
        /// </summary>
        public float Radius
        {
            get { return CharacterController.radius; }
            set { CharacterController.radius = value; }
        }
        /// <summary>
        /// The radius combined with the skin width of this <see cref="CharacterController"/>
        /// </summary>
        public float RadiusWithSkinWidth
        {
            get { return Radius + SkinWidth; }
        }
        /// <summary>
        /// The skin width of this <see cref="CharacterController"/>, same as <see cref="UnityEngine.CharacterController.skinWidth"/>
        /// </summary>
        public float SkinWidth
        {
            get { return CharacterController.skinWidth; }
            set { CharacterController.skinWidth = value; }
        }
        /// <summary>
        /// The slope limit in degrees of this <see cref="CharacterController"/>, same as <see cref="UnityEngine.CharacterController.slopeLimit"/>
        /// </summary>
        public float SlopeLimit
        {
            get { return CharacterController.slopeLimit; }
            set { CharacterController.slopeLimit = value; }
        }
        /// <summary>
        /// The step offset in meters of this <see cref="CharacterController"/>, same as <see cref="UnityEngine.CharacterController.stepOffset"/>
        /// </summary>
        public float StepOffset
        {
            get { return CharacterController.stepOffset; }
            set { CharacterController.stepOffset = value; }
        }
      


        /// <summary>
        /// The look angles of the <see cref="Player"/>.
        /// </summary>
        public Vector2 LookAngles { get; set; }
        /// <summary>
        /// The look angles delta of the <see cref="Player"/>.
        /// </summary>
        public Vector2 LookAnglesDelta { get; set; }
        /// <summary>
        /// The target look angles of the <see cref="Player"/> unaffected by mouse smoothing.
        /// </summary>
        public Vector2 TargetLookAngles { get; set; }

        /// <summary>
        /// The desired movement of the <see cref="Player"/> in local space.
        /// </summary>
        public Vector3 DesiredMovement { get; protected set; }
        /// <summary>
        /// The desired movement of the <see cref="Player"/> in world space.
        /// </summary>
        public Vector3 DesiredWorldMovement { get; protected set; }
        /// <summary>
        /// The normalized desired movement of the <see cref="Player"/> in local space.
        /// </summary>
        public Vector3 NormalizedDesiredMovement { get; protected set; }
        /// <summary>
        /// The normalized desired movement of the <see cref="Player"/> in world space.
        /// </summary>
        public Vector3 NormalizedDesiredWorldMovement { get; protected set; }
        /// <summary>
        /// The final movement of the <see cref="Player"/> when calling <see cref="UnityEngine.CharacterController.Move(Vector3)"/>
        /// </summary>
        public Vector3 FinalMovement { get; set; }
        /// <summary>
        /// The velocity of this <see cref="CharacterController"/>. Does not include additional speeds like kinematic movement.
        /// </summary>
        public Vector3 Velocity
        {
            get { return CharacterController.velocity; }
        }
        /// <summary>
        /// The planar velocity (X and Z only) of this <see cref="CharacterController"/>. Does not include additional speeds like kinematic movement.
        /// </summary>
        public Vector3 PlanarVelocity
        {
            get { return new Vector3(Velocity.x, 0, Velocity.z); }
        }
        /// <summary>
        /// The total velocity of this <see cref="CharacterController"/>, kinematic movement included.
        /// </summary>
        public Vector3 TotalVelocity { get; protected set; }
        /// <summary>
        /// The target speed magnitude of this <see cref="Player"/>.
        /// </summary>
        public float TargetSpeed
        {
            get
            {
                if (IsCrouching)
                    return Movement.Speed * Movement.Crouch.SpeedMultiplier;
                if (IsSprinting)
                    return Movement.Speed * Movement.Sprint.SpeedMultiplier;

                return Movement.Speed;
            }
        }

        /// <summary>
        /// Is this <see cref="Player"/> grounded?
        /// </summary>
        public bool IsGrounded
        {
            get { return CharacterController.isGrounded; }
        }
        /// <summary>
        /// Is this <see cref="Player"/> falling?
        /// </summary>
        public bool IsFalling
        {
            get { return !IsGrounded; }
        }

        /// <summary>
        /// Is this <see cref="Player"/> applying a step offset?
        /// </summary>
        public bool IsStepOffsetting { get; protected set; }
        /// <summary>
        /// Was this <see cref="Player"/> applying a step offset in the previous frame?
        /// </summary>
        public bool OldIsStepOffsetting { get; protected set; }

        /// <summary>
        /// Did this <see cref="Player"/> just land?
        /// </summary>
        public bool JustLanded
        {
            get {return IsGrounded && !OldIsGrounded; }
        }
        /// <summary>
        /// Did this <see cref="Player"/> just air?
        /// </summary>
        public bool JustAired
        {
            get { return !IsGrounded && OldIsGrounded; }
        }

        /// <summary>
        /// Is this <see cref="Player"/> sprinting?
        /// </summary>
        public bool IsSprinting
        {
            get { return Movement.Sprint.AllowSprint && ControlManager.Instance.GetButton("Sprint"); }
        }
        /// <summary>
        /// Is this <see cref="Player"/> jumping?
        /// </summary>
        public bool IsJumping
        {
            get
            {
                Ray ceilingRay = new Ray(transform.position + Vector3.up * (CurrentHeight - Radius), Vector3.up);
                float ceilingDistance = Radius / 2f;
                bool ceilingRaycast = Physics.SphereCast(ceilingRay, Radius, ceilingDistance, CollisionLayers);

                Ray slopeRay = new Ray(transform.position + Vector3.up * Radius, Vector3.down);
                RaycastHit slopeHit;
                Physics.SphereCast(slopeRay, Radius, out slopeHit, Mathf.Infinity, CollisionLayers);
                bool canJumpOnThisSlope = Vector3.Angle(Vector3.up, slopeHit.normal) < SlopeLimit;

                return Movement.AllowMovement && Movement.Jump.AllowJump && canJumpOnThisSlope && !ceilingRaycast && Controls.ControlsEnabled && (Movement.Jump.AutoJump ? ControlManager.Instance.GetButton("Jump") : CheckJumpPressed());
            }
        }

        public bool IsBouncing
        {
            get; set;
        }
        /// <summary>
        /// Is this <see cref="Player"/> crouching?
        /// </summary>
        public bool IsCrouching
        {
            get
            {
                Ray r = new Ray(transform.position + Vector3.up * (CurrentHeight - Radius), Vector3.up);

                float distance = Main.Height - CurrentHeight;
                bool raycast = distance > 0 && Physics.SphereCast(r, Radius, distance, CollisionLayers);

                bool canCrouch = Movement.Crouch.AllowCrouchJump;
                if (!canCrouch)
                    canCrouch = IsGrounded;

                return Movement.Crouch.AllowCrouch && canCrouch && (Movement.AllowMovement && GetButton(Controls.CrouchButton) || raycast);
            }
        }

        /// <summary>
        /// Movement detection threshold for <see cref="IsMoving"/>, <see cref="WantsToMove"/>, <see cref="IsAscending"/>, <see cref="IsDescending"/> (By default 0.001).
        /// </summary>
        public float MovementDetectionThreshold { get; set; }
        /// <summary>
        /// Is this <see cref="Player"/> moving? (Excluding kinematic movement)
        /// </summary>
        public bool IsMoving
        {
            get { return Velocity.magnitude > MovementDetectionThreshold; }
        }
        /// <summary>
        /// Does this <see cref="Player"/> want to move? (Excluding kinematic movement, jumping and crouching)
        /// </summary>
        public bool WantsToMove
        {
            get { return DesiredMovement.sqrMagnitude > MovementDetectionThreshold; }
        }
        /// <summary>
        /// Is this <see cref="Player"/> ascending? (Excluding kinematic movement)
        /// </summary>
        public bool IsAscending
        {
            get { return Velocity.y > MovementDetectionThreshold; }
        }
        /// <summary>
        /// Is this <see cref="Player"/> descending? (Excluding kinematic movement)
        /// </summary>
        public bool IsDescending
        {
            get { return Velocity.y < -MovementDetectionThreshold; }
        }

        /// <summary>
        /// The radius multiplier of the slope limit of this <see cref="CharacterController"/>.
        /// </summary>
        public float SlopeRadiusMultiplier
        {
            get
            {
                return Mathf.Sin(Mathf.Deg2Rad * SlopeLimit);
            }
        }

        /// <summary>
        /// The current footstep data activated based on the <see cref="Material"/> or <see cref="Texture"/> we're standing on.
        /// </summary>
        public virtual FootstepAsset.Data CurrentFootstepData
        {
            get
            {
                Ray r = new Ray(transform.position + Vector3.up * Radius, Vector3.down);
                RaycastHit hit;

                if (Physics.SphereCast(r, Radius, out hit, Mathf.Infinity, CollisionLayers))
                {
                    return Footstep.Asset.GetData(hit);
                }

                return Footstep.Asset.DefaultFootsteps;
            }
        }

        /// <summary>
        /// Was this <see cref="Player"/> grounded in the previous frame?
        /// </summary>
        public bool OldIsGrounded { get; protected set; }
        /// <summary>
        /// The velocity of this <see cref="Player"/> in the previous frame.
        /// </summary>
        public Vector3 OldVelocity { get; protected set; }
        /// <summary>
        /// The force buffer that awaits execution for the next frame.
        /// </summary>
        protected Vector3 ForceBuffer { get; set; }
        /// <summary>
        /// The impulse buffer that awaits execution for the next frame.
        /// </summary>
        public Vector3 ImpulseBuffer { get; set; }
        /// <summary>
        /// The current height of this <see cref="Player"/>.
        /// </summary>
        public float CurrentHeight { get; protected set; }
        /// <summary>
        /// The current camera height of this <see cref="Player"/>.
        /// </summary>
        public float CurrentCameraHeight { get; protected set; }
        /// <summary>
        /// The current speed multiplier of this <see cref="Player"/> (<see cref="CurrentSlopeSpeedMultiplier"/> not included).
        /// </summary>
        public float CurrentSpeedMultiplier { get; protected set; }
        /// <summary>
        /// The current slope speed multiplier of this <see cref="Player"/>.
        /// </summary>
        public float CurrentSlopeSpeedMultiplier { get; protected set; }

        public int? collisionLayers;
        /// <summary>
        /// The collision layers that this <see cref="Player"/> collides into, see Physics window to change collision layers.
        /// </summary>
        public int CollisionLayers
        {
            get
            {
                //if (GetComponent<flipScript>().flippedThisFrame) { collisionLayers = null; }
                if (collisionLayers == null)
                {
                    
                    int layers = 0;

                    for (int i = 0; i < 32; i++) // Check through all layers.
                    {
                        bool collides = !Physics.GetIgnoreLayerCollision(gameObject.layer, i); // Gathers if we are colliding with the selected layer or not.
                        if (collides)
                            layers |= 1 << i; // If so, add it to the layer mask.
                    }

                    collisionLayers = layers;
                   // Debug.Log(collisionLayers);
                }

                return collisionLayers ?? ~0;
            }
        }

        private static readonly Vector3[] continuousCollisionSweepVectors =
        {
        new Vector3(1, 0, 0),
        new Vector3(-1, 0, 0),

        new Vector3(0, 1, 0),
        new Vector3(0, -1, 0),

        new Vector3(0, 0, 1),
        new Vector3(0, 0, -1)
    };

        public Player()
        {
            Main = new MainSettings(this);
            Controls = new ControlSettings(this);
            Collisions = new CollisionSettings(this);
            Movement = new MovementSettings(this)
            {
                Jump = new MovementSettings.JumpSettings(this),
                Sprint = new MovementSettings.SprintSettings(this),
                Crouch = new MovementSettings.CrouchSettings(this)
            };
            Footstep = new FootstepSettings(this);
        }

        /// <summary>
        /// Adds a force to this <see cref="Player"/>.
        /// </summary>
        /// <param name="force">The force.</param>
        public void AddForce(Vector3 force)
        {
            ForceBuffer += force;
        }
        /// <summary>
        /// Adds an impulse to this <see cref="Player"/>.
        /// </summary>
        /// <param name="impulse">The impulse.</param>
        public void AddImpulse(Vector3 impulse)
        {
            ImpulseBuffer += impulse;
            //Debug.Log("adding impulse " + impulse.sqrMagnitude);
        }
        /// <summary>
        /// Teleports the player to a specified position.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="resetVelocity">Should we reset the velocity of the player?</param>
        public void TeleportTo(Vector3 position, bool resetVelocity = true)
        {
            transform.position = position;

            if (resetVelocity)
            {
                DesiredMovement = Vector3.zero;
                DesiredWorldMovement = Vector3.zero;
                FinalMovement = Vector3.zero;
                KinematicMovement = Vector3.zero;
                KinematicAngles = Vector3.zero;
                ForceBuffer = Vector3.zero;
                ImpulseBuffer = Vector3.zero;
                TotalVelocity = Vector3.zero;
                //Velocity = Vector3.zero;
                OldVelocity = Vector3.zero;
                GetComponent<CharacterController>().Move(Vector3.zero);
                LookAnglesDelta = Vector2.zero;
            }
        }

        private int overlappingCollidersCount;
        private Collider[] overlappingColliders;
        private List<Collider> ignoredColliders;
        private bool jumpPressed = false;

        private void Awake()
        {
            if (!Toolbox.Instance)
                StartCoroutine(LoadToolbox());
            else
            {
                Toolbox.Instance.SetPlayer(gameObject);
                Toolbox.Instance.UpdateToolbox();
            }
            
                
        }

        private IEnumerator LoadToolbox()
        {
            AsyncOperation _async = SceneManager.LoadSceneAsync("Toolbox", LoadSceneMode.Additive);
            _async.allowSceneActivation = true;

            while (!_async.isDone)
            {
                yield return null;
            }
            
            Toolbox.Instance.SetPlayer(gameObject);


            Toolbox.Instance.UpdateToolbox();

        }


        private void Start()
        {
            overlappingCollidersCount = 0;
            overlappingColliders = new Collider[256];
            ignoredColliders = new List<Collider>(256);

            CharacterController = GetComponent<CharacterController>();
            CharacterController.enableOverlapRecovery = Collisions.EnableOverlapRecovery;

            CollidingColliders = new List<Collider>();
            ColliderHits = new List<ControllerColliderHit>();

            OldIsGrounded = IsGrounded;
            CurrentHeight = Main.Height;
            CurrentCameraHeight = Main.CameraHeight;
            CurrentSpeedMultiplier = 1f;
            MovementDetectionThreshold = 0.001f;

            if (Footstep.Asset != null)
                FootstepTimer = Footstep.Asset.WalkTimer;

            foreach (PlayerModule module in Modules.OrderBy(m => m.ExecutionOrder)) // Initialize modules
                module.Initialize();
        }

        private void Update()
        {
            if (!LevelLoadingMenu.gameIsPaused)
            {
                BeforeUpdate(); // Call before update (along with its modules)
                DoMouse(); // Mouse related
                SetJumpPressed();
                //DoMovement(); // Movement related
                UpdateHeight(); // Update the height
                DoFootsteps(); // Footsteps related
                AfterUpdate(); // Call after update (along with its modules)
            }
        }

        private void FixedUpdate()
        {
            if (!LevelLoadingMenu.gameIsPaused)
            {
                DoMovement();
                SetJumpPressedOff();
            }
        }

        /// <summary>
        /// Executed before the Update.
        /// </summary>
        protected virtual void BeforeUpdate()
        {
            foreach (PlayerModule module in Modules.Where(m => m.Enabled).OrderBy(m => m.ExecutionOrder)) // Call BeforeUpdate on all enabled modules.
                module.BeforeUpdate();
        }
        /// <summary>
        /// Executed after the Update.
        /// </summary>
        protected virtual void AfterUpdate()
        {
            foreach (PlayerModule module in Modules.Where(m => m.Enabled).OrderBy(m => m.ExecutionOrder)) // Call AfterUpdate on all enabled modules.
                module.AfterUpdate();
        }

        /// <summary>
        /// Mouse movement.
        /// </summary>
        protected void DoMouse()
        {
            bool locked = Controls.ControlsEnabled && Controls.MouseLocked; // Determine if the mouse is locked.
            Cursor.visible = !locked; // Set invisible mouse if locked, otherwise make it visible.
            Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None; // Set the mouse lock state.

            if (Movement.AllowMouseMove)
            {
                Vector2 rawDelta = new Vector2(GetMouseCurrentAxis(Controls.MouseXAxis), GetMouseCurrentAxis(Controls.MouseYAxis)) * Controls.MouseSensitivity * 5; // Raw delta of the mouse.
                TargetLookAngles += rawDelta; // Apply raw delta to target look angles.
                TargetLookAngles = new Vector2(TargetLookAngles.x, Mathf.Clamp(TargetLookAngles.y, -90f, 90f)); // Clamp target look angles Y values between -90...90 to avoid looking too much up or down.

                Vector2 newLookAngles = Vector2.Lerp(TargetLookAngles, Vector2.Lerp(LookAngles, TargetLookAngles, Time.deltaTime), Controls.MouseSmoothing); // Set the new look angles also taking into account mouse smoothing by doing a lerp between the target look angles and a very smoothly lerped target look angles.

                LookAnglesDelta = newLookAngles - LookAngles; // Calculate the delta, taking into account the mouse smoothing.
                LookAngles += LookAnglesDelta; // Apply the delta to our look angles.
            }

            Main.Camera.transform.localEulerAngles = new Vector3(-LookAngles.y, 0, 0); // Set the camera euler X axis to the negative of the Y look angles.
            transform.localEulerAngles = new Vector3(0, LookAngles.x, 0); // Set the player euler Y axis to the X look angles.
        }
        /// <summary>
        /// Character movement.
        /// </summary>
        protected void DoMovement()
        {
            IsStepOffsetting = false;

            if (IsGrounded) // If grounded
            {
                Vector3 targetNormalizedDesiredMovement = new Vector3(GetAxis(Controls.HorizontalAxis), 0, GetAxis(Controls.VerticalAxis)); // Raw axis input

                if (!Movement.AllowMovement) // If we don't allow movement, nullify axis input
                    targetNormalizedDesiredMovement = Vector3.zero;

                bool accelerate = Math.Abs(GetAxis(Controls.HorizontalAxis)) > 0f || Math.Abs(GetAxis(Controls.VerticalAxis)) > 0f; // Are we accelerating, or decelerating?
                float speedRate = accelerate ? Movement.Acceleration : Movement.Deceleration; // Choose speed rate depending on acceleration or deceleration

                if (targetNormalizedDesiredMovement.magnitude > 1f) // If the magnitude of our raw axis input is bigger than 1, normalize it (This avoids extra speed when moving diagonally)
                    targetNormalizedDesiredMovement.Normalize();

                NormalizedDesiredMovement = Vector3.Lerp(NormalizedDesiredMovement, targetNormalizedDesiredMovement, Time.fixedDeltaTime * speedRate); // Smooth the movement by the speed rate
                NormalizedDesiredWorldMovement = transform.TransformVector(NormalizedDesiredMovement); // Convert from local space to world space
                DesiredMovement = NormalizedDesiredMovement * Movement.Speed; // Multiply by the speed

                if (IsCrouching) // If crouching, apply smooth crouch speed multiplier
                    CurrentSpeedMultiplier = Mathf.Lerp(CurrentSpeedMultiplier, Movement.Crouch.SpeedMultiplier,
                        Time.fixedDeltaTime * Movement.Deceleration);
                else if (IsSprinting) // If crouching, apply smooth crouch speed multiplier
                    CurrentSpeedMultiplier = Mathf.Lerp(CurrentSpeedMultiplier, Movement.Sprint.SpeedMultiplier,
                        Time.fixedDeltaTime * Movement.Acceleration);
                else // Otherwise, apply smooth base speed multiplier
                    CurrentSpeedMultiplier = Mathf.Lerp(CurrentSpeedMultiplier, 1f, Time.fixedDeltaTime * Movement.Acceleration);

                Ray slopeRay = new Ray(transform.position + Vector3.up * Radius, Vector3.down);
                RaycastHit slopeHit;
                if (Physics.SphereCast(slopeRay, Radius, out slopeHit, Radius, CollisionLayers) && Physics.Raycast(transform.position, NormalizedDesiredWorldMovement, RadiusWithSkinWidth + 0.001f))
                {
                    float angle = Mathf.Clamp(Vector3.Angle(Vector3.up, slopeHit.normal), 0f, 90f);
                    float lerp = Mathf.InverseLerp(0, SlopeLimit, angle);
                    CurrentSlopeSpeedMultiplier = Mathf.Lerp(CurrentSlopeSpeedMultiplier, Movement.SlopeSpeedMultiplierCurve.Evaluate(lerp), Time.fixedDeltaTime * Movement.Acceleration);
                }
                else
                {
                    CurrentSlopeSpeedMultiplier = Mathf.Lerp(CurrentSlopeSpeedMultiplier, Movement.SlopeSpeedMultiplierCurve.Evaluate(0f), Time.fixedDeltaTime * Movement.Acceleration);
                }

                DesiredMovement *= CurrentSpeedMultiplier * CurrentSlopeSpeedMultiplier; // Apply speed multiplier to desired movement
                DesiredWorldMovement = transform.TransformVector(DesiredMovement); // Convert from local space to world space

                FinalMovement = DesiredWorldMovement; // Apply desired world movement to final movement
                if (ImpulseBuffer.sqrMagnitude > 0 || ForceBuffer.sqrMagnitude > 0) // Apply forces and impulses to final movement
                {
                    FinalMovement += ForceBuffer;
                    FinalMovement += ImpulseBuffer;
                    Debug.LogWarning("Impulse is " + ImpulseBuffer);
                }

                if (!IsJumping && KinematicMovement.sqrMagnitude <= 0 && (ImpulseBuffer.sqrMagnitude==0)) // Negative step offset for stairs and downwards slopes
                {
                    Ray r = new Ray(transform.position + Vector3.up * (Radius * SlopeRadiusMultiplier), Vector3.down);
                    if (Physics.SphereCast(r, Radius * SlopeRadiusMultiplier, StepOffset + SkinWidth, CollisionLayers))
                    {
                        FinalMovement += Vector3.down * (StepOffset / Time.fixedDeltaTime);
                        IsStepOffsetting = true;
                    }
                }

                if (IsJumping && !IsBouncing) { // Jump
                    FinalMovement += Vector3.up * Movement.Jump.Power;
                    //Debug.Log("jumping");

                   // StartCoroutine(PrintJumpHeight());
                }
            }
            else // Not grounded, in the air
            {
                NormalizedDesiredMovement = transform.InverseTransformVector(PlanarVelocity) / (Movement.Speed * CurrentSpeedMultiplier);
                FinalMovement = JustAired ? Velocity + OldKinematicMovement : Velocity;

                if (JustAired && !IsJumping && KinematicMovement.sqrMagnitude <= 0 && OldKinematicMovement.sqrMagnitude <= 0)
                {
                    FinalMovement.Set(FinalMovement.x, Physics.gravity.y * Time.fixedDeltaTime, FinalMovement.z);
                }

                if (Movement.AllowMovement && Movement.AirControl) // Air control
                {
                    if (Movement.SmartStrafing) // Smart strafing
                    {
                        Vector3 wishDir = transform.TransformVector(GetAxis("MoveX"), 0, GetAxis("MoveY"));
                        if (wishDir.magnitude > 1f)
                            wishDir.Normalize();

                        Vector3 finalStrafe = AirAccelerate(wishDir, PlanarVelocity, Movement.AirAccelerate);

                        FinalMovement += finalStrafe * Time.fixedDeltaTime; // Apply smart strafe
                    }

                    if (Movement.VerticalStrafing) // Vertical strafing
                    {
                        Vector3 vertical = transform.TransformVector(0, 0, GetAxis("MoveY")) * Movement.StrafingSpeed;
                        FinalMovement += vertical * Time.fixedDeltaTime;
                    }

                    if (Movement.HorizontalStrafing) // Horizontal strafing
                    {
                        Vector3 horizontal = transform.TransformVector(GetAxis("Movex"), 0, 0) * Movement.StrafingSpeed;
                        FinalMovement += horizontal * Time.fixedDeltaTime;
                    }
                }

                if (ForceBuffer.sqrMagnitude > 0 || ImpulseBuffer.sqrMagnitude > 0) // Apply forces and impulses in the air
                {
                    FinalMovement += ForceBuffer * Time.fixedDeltaTime;
                    FinalMovement += ImpulseBuffer;
                }

                if (OldIsGrounded && OldKinematicMovement.sqrMagnitude > 0 && OldIsStepOffsetting) // If we had negative kinematic step offset and we just aired, cancel it
                {
                    FinalMovement += -Vector3.down * (StepOffset / Time.fixedDeltaTime);
                }

                FinalMovement += Physics.gravity * Time.fixedDeltaTime; // Apply gravity
            }

            float oldCameraHeight = CurrentCameraHeight; // Last frame camera height
            CurrentHeight = Mathf.Lerp(CurrentHeight, IsCrouching ? Movement.Crouch.CrouchHeight : Main.Height, Time.fixedDeltaTime * Movement.Crouch.CrouchingSpeed); // Smooth current height updating.
            CurrentCameraHeight = Mathf.Lerp(CurrentCameraHeight, IsCrouching ? Movement.Crouch.CrouchCameraHeight : Main.CameraHeight, Time.fixedDeltaTime * Movement.Crouch.CrouchingSpeed); // Smooth current camera height updating.

            if (Movement.Crouch.AllowCrouchJump && IsFalling) // Apply crouch jump vertical position delta based on camera height delta (oldCameraHeight - CurrentCameraHeight)
            {
                float curHeightDelta = oldCameraHeight - CurrentCameraHeight;

                if (curHeightDelta > 0)
                {
                    transform.position += Vector3.up * curHeightDelta;
                }
                else if (curHeightDelta < 0 && !Physics.Raycast(transform.position, Vector3.down, Mathf.Abs(curHeightDelta), CollisionLayers)) // Also make sure we're not hitting the ground to not penetrate it if we are standing up in the air
                {
                    transform.position += Vector3.up * curHeightDelta;
                }
            }

            if (KinematicMovement.sqrMagnitude > 0f && !IsJumping)
            {
                transform.position += KinematicMovement * Time.fixedDeltaTime;

                Ray r = new Ray(transform.position + Vector3.up * (Radius * SlopeRadiusMultiplier), Vector3.down);
                if (IsGrounded && Physics.SphereCast(r, Radius * SlopeRadiusMultiplier, StepOffset + SkinWidth, CollisionLayers)) // Negative step offset to stay properly on moving kinematic platforms
                {
                    FinalMovement += Vector3.down * (StepOffset / Time.fixedDeltaTime);
                    IsStepOffsetting = true;
                }

                TargetLookAngles += Vector2.right * (KinematicAngles.y * Mathf.Rad2Deg) * Time.fixedDeltaTime; // Apply look angles offset on rotating kinematic movement (So that camera rotates along)
            }

            TotalVelocity = Velocity + KinematicMovement; // Update total velocity property
            ForceBuffer = ImpulseBuffer = Vector3.zero; // Reset force and impulse buffers as we have used them for this frame

            if (JustAired && OnAired != null) // On aired event trigger
                OnAired();

            if (JustLanded && OnLanded != null) // On landed event trigger
                OnLanded(OldVelocity);

            if (BeforeFinalMovement != null) // Before final movement event trigger
                BeforeFinalMovement(FinalMovement);

            OldIsGrounded = IsGrounded; // Set "old" variables to store them for next frame
            OldVelocity = Velocity;
            OldKinematicMovement = KinematicMovement;
            OldIsStepOffsetting = IsStepOffsetting;

            if (Collisions.ContinuousCollisionDetection) // Apply continuous collision detection
            {
                int passes = Collisions.ContinuousCollisionDetectionPasses; // Passes amount

                float strength = Time.fixedDeltaTime / Time.fixedDeltaTime * (Collisions.ContinuousCollisionDetectionStrength); // Strength of detection (How far we will move for the detection)
                float amountOfPhysicsUpdateRatio = Mathf.Clamp(Time.fixedDeltaTime / Time.fixedDeltaTime, 1f, float.MaxValue);

                for (int i = 0; i < passes; i++)
                {
                    for (int j = 0; j < continuousCollisionSweepVectors.Length; j++)
                    {
                        // Move character controller in such a way that we get back to the same position, but we have checked for collisions in the X and Z axis
                        Vector3 oldPos = transform.position;
                        CharacterController.Move(continuousCollisionSweepVectors[j] * strength);
                        Vector3 delta = transform.position - oldPos;
                        CharacterController.Move(-delta);
                    }

                    if (KinematicMovement.sqrMagnitude > 0 && new Vector3(KinematicMovement.x, 0, KinematicMovement.z).sqrMagnitude > 0)
                    {
                        CharacterController.Move(KinematicMovement.normalized * SkinWidth); // Depenetrate character
                        CharacterController.Move(KinematicMovement * amountOfPhysicsUpdateRatio * strength); // Apply kinematic movement collision checking only if there is any kinematic movement
                        CharacterController.Move(-KinematicMovement * amountOfPhysicsUpdateRatio * strength);
                        CharacterController.Move(-KinematicMovement.normalized * SkinWidth);
                    }
                }
            }

            ClampFinalMovement();

            KinematicMovement = KinematicAngles = Vector3.zero; // Reset kinematic movement and angles as we have used them for this frame
            CollidingColliders.Clear();
            ColliderHits.Clear();

            PreCharacterControllerUpdate ();

            CharacterController.Move(FinalMovement * Time.fixedDeltaTime); // Move the player

            PostCharacterControllerUpdate ();

            if (AfterFinalMovement != null) // After final movement event trigger
                AfterFinalMovement(FinalMovement);
        }

        private void PreCharacterControllerUpdate () {
            Vector3 center = transform.TransformPoint(CharacterController.center);
            Vector3 delta = (0.5f * CharacterController.height - CharacterController.radius) * Vector3.up;
            Vector3 bottom = center - delta;
            Vector3 top = bottom + delta;

            overlappingCollidersCount = Physics.OverlapCapsuleNonAlloc(bottom, top, CharacterController.radius, overlappingColliders);

            for (int i = 0; i < overlappingCollidersCount; i++) {
                Collider overlappingCollider = overlappingColliders[i];

                //if (overlappingCollider.gameObject.isStatic) {
                //    continue;
                //}

                ignoredColliders.Add(overlappingCollider);
                Physics.IgnoreCollision(CharacterController, overlappingCollider, true);
            }

        }

        private void PostCharacterControllerUpdate () {
            for (int i = 0; i < ignoredColliders.Count; i++) {
                Collider ignoredCollider = ignoredColliders[i];

                Physics.IgnoreCollision(CharacterController, ignoredCollider, false);
            }

            ignoredColliders.Clear();
        }

         void ClampFinalMovement()
        {

            //fixed issue with gravity being insane in opposite world from where player started. manually making sure negative velocity doesn't go nuts
            //if (FinalMovement.y < -14 && OldIsGrounded) {Debug.Log(CollisionLayers); FinalMovement = new Vector3(FinalMovement.x, -1f, FinalMovement.z); }
            //Debug.Log(FinalMovement.y);

            Vector3 planarFinalMovement = new Vector3(FinalMovement.x, 0, FinalMovement.z);

            if (planarFinalMovement.magnitude > Movement.MaxSpeed) // Check to see if the player is moving too fast in the horizontal plane
            {
                planarFinalMovement = Vector3.ClampMagnitude(planarFinalMovement, Movement.MaxSpeed);
                FinalMovement = new Vector3(planarFinalMovement.x, FinalMovement.y, planarFinalMovement.z); // Clamp the x and z values of FinalMovement to the maximum speed
            }

        }

        IEnumerator PrintJumpHeight() {
            float startingHeight = transform.position.y;
            float currentHeight = startingHeight;
            float previousHeight = currentHeight - 1f;

            while (previousHeight <= currentHeight) {
                yield return null; // skip a frame
                previousHeight = currentHeight;
                currentHeight = transform.position.y;
            }

            Debug.Log("Jump Height: " + (previousHeight - startingHeight));
        }

        /// <summary>
        /// Update the height of the <see cref="Player"/>.
        /// </summary>
        protected void UpdateHeight()
        {
            CharacterController.height = CurrentHeight; // Set the character controller height to the current height
            CharacterController.center = Vector3.up * (CurrentHeight / 2f); // Set the character controller center to the current height divided by 2 in the Y axis

            if (Main.Camera != null)
                Main.Camera.transform.localPosition = Vector3.up * CurrentCameraHeight; // Set the local position of the camera to the current camera height in the Y axis
        }

        /// <summary>
        /// The footstep timer interval left before emitting a footstep.
        /// </summary>
        protected float FootstepTimer { get; set; }
        /// <summary>
        /// Footstep updating.
        /// </summary>
        protected void DoFootsteps()
        {
            if (Footstep.Asset == null) // If footstep asset is null, don't bother doing the rest.
                return;

            if (FootstepTimer <= 0) // If footstep timer is due, emit footstep and reset the timer.
            {
                EmitFootstepSound();
                FootstepTimer += Footstep.Asset.WalkTimer;
            }
            else
            {
                if (IsGrounded) // If the player is grounded, calculate his magnitude and decrease the footstep timer accordingly (if sprinting, crouching, etc...)
                {
                    float maxMagnitude = Movement.Speed;
                    float mult = 1f;

                    if (IsCrouching)
                    {
                        maxMagnitude *= Movement.Crouch.SpeedMultiplier;
                        mult = Footstep.Asset.CrouchTimerMultiplier;
                    }
                    else if (IsSprinting)
                    {
                        maxMagnitude *= Movement.Sprint.SpeedMultiplier;
                        mult = Footstep.Asset.SprintTimerMultiplier;
                    }

                    FootstepTimer -= PlanarVelocity.magnitude / maxMagnitude * (Time.deltaTime / mult);
                }
            }

            if (JustAired && IsJumping) // If just aired and we are jumping, emit jump sound.
                EmitJumpSound();

            if (JustLanded) // If just landed, emit landing sound with the appropriate volume relative to the velocity.
            {
                float velocityMagnitude = OldVelocity.magnitude;
                EmitLandSound(velocityMagnitude);
                FootstepTimer = Footstep.Asset.WalkTimer;
            }
        }

        /// <summary>
        /// The kinematic movement of this <see cref="Player"/>, also known as the pushing/pulling force of other kinematic <see cref="Rigidbody"/>(ies).
        /// </summary>
        public Vector3 KinematicMovement { get; set; }
        /// <summary>
        /// The kinematic movement of this <see cref="Player"/> performed in the previous frame.
        /// </summary>
        public Vector3 OldKinematicMovement { get; protected set; }
        /// <summary>
        /// The kinematic angular movement, useful for rotating the <see cref="Player"/> as we rotate along a kinematic body.
        /// </summary>
        public Vector3 KinematicAngles { get; set; }
        /// <summary>
        /// The list of colliders that are colliding with this <see cref="Player"/> in this frame.
        /// </summary>
        public List<Collider> CollidingColliders { get; protected set; }
        /// <summary>
        /// The list of collider hit info of this <see cref="Player"/> in this frame.
        /// </summary>
        public List<ControllerColliderHit> ColliderHits { get; protected set; }
        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            Rigidbody body = hit.collider.attachedRigidbody;

            if (!CollidingColliders.Contains(hit.collider)) // If we didn't check the collider, add it to the list.
            {
                CollidingColliders.Add(hit.collider);
                ColliderHits.Add(hit);

                if (body != null && body.isKinematic) // If it contains rigidbody and is kinematic.
                {
                    Vector3 vel = body.GetPointVelocity(transform.position); // Get the velocity at the point of the player.

                    bool isWall = Mathf.Abs(Vector3.Dot(Vector3.up, hit.normal)) < 0.01f;

                    if (isWall) // Nullify Y kinematic velocity if wall.
                        vel = new Vector3(vel.x, 0, vel.z);

                    if (isWall && Vector3.Dot(vel.normalized, hit.normal.normalized) <= 0) // Nullify if rigidbody is moving away from us and is wall.
                        vel = Vector3.zero;

                    KinematicMovement += vel; // Add kinematic movement.
                    KinematicAngles += body.angularVelocity; // Add kinematic angular movement.
                }

                if (CollisionDetected != null) // Collision detected event trigger.
                    CollisionDetected(hit);
            }
        }

        /// <summary>
        /// Emits a footstep sound.
        /// </summary>
        public void EmitFootstepSound()
        {
            if (Footstep.GlobalVolume <= 0 || Footstep.Asset == null)  // If global volume or footstep asset is null, don't bother with the rest.
                return;

            FootstepAsset.Data data = CurrentFootstepData; // Get the corresponding footstep data
            if (data == null)
                throw new Exception("Footstep data was not found, are you sure you have any footsteps in your FootstepAsset or that you provided an existing footstep data name for the default footsteps?");

            AudioClip sound = data.GetRandomFootstep();  // Get the sound

            float volume = Footstep.Asset.WalkVolume;

            if (IsCrouching)
                volume = Footstep.Asset.CrouchVolume;
            else if (IsSprinting)
                volume = Footstep.Asset.SprintVolume;

            if (volume <= 0)
                return;

            float pitch = data.GetRandomPitch();  // Get random specified pitch

            // Play sound
            if (!Footstep.Is3D)
            {
                Audio.Play(sound, volume * Footstep.GlobalVolume, pitch);
            }
            else
            {
                if (Footstep.Range <= 0f)
                    Audio.Play3D(sound, transform, volume * Footstep.GlobalVolume, pitch);
                else
                    Audio.Play3D(sound, transform, volume * Footstep.GlobalVolume, pitch, Footstep.Range);
            }

            if (OnFootstep != null)
                OnFootstep(sound, volume, pitch);
        }
        /// <summary>
        /// Emits a jumping sound.
        /// </summary>
        public void EmitJumpSound()
        {
            if (Footstep.GlobalVolume <= 0 || Footstep.Asset == null)  // If global volume or footstep asset is null, don't bother with the rest.
                return;

            FootstepAsset.Data data = CurrentFootstepData; // Get the corresponding footstep data
            if (data == null)
                throw new Exception("Footstep data was not found, are you sure you have any footsteps in your FootstepAsset or that you provided an existing footstep data name for the default footsteps?");

            AudioClip sound = data.GetRandomJumping(); // Get the sound

            float pitch = data.GetRandomPitch();  // Get random specified pitch

            // Play sound
            if (!Footstep.Is3D)
            {
                Audio.Play(sound, Footstep.GlobalVolume, pitch);
            }
            else
            {
                if (Footstep.Range <= 0f)
                    Audio.Play3D(sound, transform, Footstep.GlobalVolume, pitch);
                else
                    Audio.Play3D(sound, transform, Footstep.GlobalVolume, pitch, Footstep.Range);
            }
        }
        /// <summary>
        /// Emits a landing sound.
        /// </summary>
        /// <param name="velocityMagnitude">The velocity magnitude.</param>
        public void EmitLandSound(float velocityMagnitude)
        {
            if (Footstep.GlobalVolume <= 0 || Footstep.Asset == null)  // If global volume or footstep asset is null, don't bother with the rest.
                return;

            FootstepAsset.Data data = CurrentFootstepData; // Get the corresponding footstep data
            if (data == null)
                throw new Exception("Footstep data was not found, are you sure you have any footsteps in your FootstepAsset or that you provided an existing footstep data name for the default footsteps?");

            AudioClip sound = data.GetRandomLanding();  // Get the sound

            float volume = Mathf.Clamp01(Mathf.InverseLerp(Footstep.Asset.LandingMinimumVelocity, Footstep.Asset.LandingMaximumVelocity, velocityMagnitude)) * Footstep.Asset.LandingVolumeMultiplier; // Set volume accordingly

            if (volume <= 0)
                return;

            float pitch = data.GetRandomPitch();  // Get random specified pitch

            // Play sound
            if (!Footstep.Is3D)
            {
                Audio.Play(sound, Footstep.GlobalVolume * volume, pitch);
            }
            else
            {
                if (Footstep.Range <= 0f)
                    Audio.Play3D(sound, transform, Footstep.GlobalVolume * volume, pitch);
                else
                    Audio.Play3D(sound, transform, Footstep.GlobalVolume * volume, pitch, Footstep.Range);
            }
        }

        /// <summary>
        /// Similar to <see cref="Input.GetAxis(string)"/> but also taking into account <see cref="ControlSettings.ControlsEnabled"/>.
        /// </summary>
        /// <param name="axisName">The name of the axis.</param>
        /// <returns>The value of the virtual axis identified by <see cref="axisName"/>.</returns>
        protected float GetAxis(string axisName)
        {
            return Controls.ControlsEnabled ? ControlManager.Instance.GetAxis(axisName) : 0f;
        }
        /// <summary>
        /// Similar to <see cref="Input.GetAxisRaw(string)"/> but also taking into account <see cref="ControlSettings.ControlsEnabled"/>.
        /// </summary>
        /// <param name="axisName">The name of the axis.</param>
        /// <returns>The value of the virtual axis identified by <see cref="axisName"/>.</returns>
        protected float GetAxisRaw(string axisName)
        {
            return Controls.ControlsEnabled ? ControlManager.Instance.GetAxis(axisName) : 0f;
        }
/*
        /// <summary>
        /// Returns the current axis value, based on whetever we use <see cref="ControlSettings.RawInput"/> and also taking into account <see cref="ControlSettings.ControlsEnabled"/>.
        /// </summary>
        /// <param name="axisName">The axis name.</param>
        /// <returns>The value of the virtual axis identified by <see cref="axisName"/>.</returns>
        protected float GetCurrentAxis(string axisName)
        {
            float ret = 0;
            if (axisName == "Vertical")
            {
                if (Input.GetKey(ControlManager.Instance.forward)) 
                {
                    ret += 1;
                }
                if (Input.GetKey(ControlManager.Instance.backward)) 
                {
                    ret -= 1;
                }
            }
            else if (axisName == "Horizontal")
            {
                if (Input.GetKey(ControlManager.Instance.left))
                {
                    ret -= 1;
                }

                if (Input.GetKey(ControlManager.Instance.right))
                {
                    ret += 1;
                }
            }

            return ret;
        }
*/

        float GetMouseCurrentAxis(string axisName)
        {
            return Controls.RawInput ? GetAxisRaw(axisName) : GetAxis(axisName);
        }
        
        /// <summary>
        /// Similar to <see cref="Input.GetButton(string)"/> but also taking into account <see cref="ControlSettings.ControlsEnabled"/>.
        /// </summary>
        /// <param name="buttonName">The name of the button.</param>
        /// <returns>Returns true while the virtual button identified by <see cref="buttonName"/> is held down.</returns>
        protected bool GetButton(string buttonName)
        {
            return Controls.ControlsEnabled && Input.GetButton(buttonName);
        }
        /// <summary>
        /// Similar to <see cref="Input.GetButtonDown(string)"/> but also taking into account <see cref="ControlSettings.ControlsEnabled"/>.
        /// </summary>
        /// <param name="buttonName">The name of the button.</param>
        /// <returns>Returns true during the frame the user pressed down the virtual button identified by <see cref="buttonName"/>.</returns>
        protected bool GetButtonDown(string buttonName)
        {
            return Controls.ControlsEnabled && Input.GetButtonDown(buttonName);
        }
        /// <summary>
        /// Similar to <see cref="Input.GetButtonUp(string)"/> but also taking into account <see cref="ControlSettings.ControlsEnabled"/>.
        /// </summary>
        /// <param name="buttonName">The name of the button.</param>
        /// <returns>Returns true the first frame the user releases the virtual button identified by <see cref="buttonName"/>.</returns>
        protected bool GetButtonUp(string buttonName)
        {
            return Controls.ControlsEnabled && Input.GetButtonUp(buttonName);
        }

        /// <summary>
        /// Applies air accelerate for a certain wished direction and the current velocity we're going at.
        /// </summary>
        /// <param name="wishDir">The wished direction.</param>
        /// <param name="velocity">The current velocity.</param>
        /// <param name="accelerate">The accelerate amount we want to apply.</param>
        /// <returns>The corrected air accelerated velocity.</returns>
        protected Vector3 AirAccelerate(Vector3 wishDir, Vector3 velocity, float accelerate)
        {
            float velocityDot = Vector3.Dot(wishDir, velocity.normalized);
            float lookDot = Vector3.Dot(transform.forward, velocity.normalized);
            lookDot = lookDot * lookDot * lookDot;
            Vector3 correctedDir = Vector3.Lerp(wishDir, transform.forward * 0.001f * accelerate, velocityDot);
            return Vector3.Lerp(correctedDir * accelerate, Vector3.zero, Mathf.Lerp(1, velocityDot, lookDot));
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(0, 0.5f, 1f, 1f);
            Extensions.GizmosDrawCapsule(transform.position + CharacterController.center, CharacterController.height, Radius, Radius + SkinWidth);
        }

        private void OnValidate()
        {
            if (CharacterController == null)
                CharacterController = GetComponent<CharacterController>();

            if (Main.Camera == null)
            {
                Transform cameraTransform = transform.Find("Camera");
                if (cameraTransform != null)
                    Main.Camera = transform.Find("Camera").GetComponent<Camera>() ?? Camera.main;
                else
                    Main.Camera = Camera.main;
            }

            CurrentHeight = Main.Height;
            CurrentCameraHeight = Main.CameraHeight;
            UpdateHeight();
        }

        public bool CheckJumpPressed()
        {
            return jumpPressed;
        }

        public void SetJumpPressed()
        {
            if (ControlManager.Instance.GetButtonDown("Jump")){ // Input.GetKeyDown(ControlManager.Instance.jump)) {
                jumpPressed = true;
            }
        }

        public void SetJumpPressedOff()
        {
            jumpPressed = false;
        }

        /// <summary>
        /// Base class for player settings.
        /// </summary>
        [Serializable]
        public abstract class Settings
        {
            /// <summary>
            /// The attached player to these settings.
            /// </summary>
            protected Player Player { get; private set; }

            public Settings(Player player)
            {
                Player = player;
            }
        }

        /// <summary>
        /// Main player settings.
        /// </summary>
        [Serializable]
        public class MainSettings : Settings
        {
            public MainSettings(Player player) : base(player) { }

            [SerializeField]
            [Tooltip("The camera attached to this Player.")]
            private Camera camera;
            /// <summary>
            /// The camera attached to this <see cref="Player"/>.
            /// </summary>
            public Camera Camera
            {
                get { return camera; }
                set { camera = value; }
            }

            [SerializeField]
            [Tooltip("The height of this Player.")]
            private float height = 1.7f;
            /// <summary>
            /// The height of this <see cref="Player"/>.
            /// </summary>
            public float Height
            {
                get { return height; }
                set
                {
                    height = value;
                }
            }

            [SerializeField]
            [Tooltip("The camera height of this Player.")]
            private float cameraHeight = 1.5f;
            /// <summary>
            /// The camera height of this <see cref="Player"/>.
            /// </summary>
            public float CameraHeight
            {
                get { return cameraHeight; }
                set
                {
                    camera.transform.localPosition = Vector3.up * cameraHeight;
                    cameraHeight = value;
                }
            }
        }

        /// <inheritdoc />
        /// <summary>
        /// Player input controls settings.
        /// </summary>
        [Serializable]
        public class ControlSettings : Settings
        {
            public ControlSettings(Player player) : base(player) { }

            [SerializeField]
            [Tooltip("Are input controls enabled?")]
            private bool controlsEnabled = true;
            /// <summary>
            /// Are input controls enabled?
            /// </summary>
            public bool ControlsEnabled
            {
                get { return controlsEnabled; }
                set { controlsEnabled = value; }
            }

            [Space]
            [SerializeField]
            [Tooltip("Should we use raw input for any axis?")]
            private bool rawInput = true;
            /// <summary>
            /// Should we use raw input for any axis?
            /// </summary>
            public bool RawInput
            {
                get { return rawInput; }
                set { rawInput = value; }
            }

            [Header("Mouse Input")]
            [SerializeField]
            [Tooltip("Is the mouse locked?")]
            private bool mouseLocked = true;
            /// <summary>
            /// Is the mouse locked?
            /// </summary>
            public bool MouseLocked
            {
                get { return mouseLocked; }
                set { mouseLocked = value; }
            }
            [SerializeField]
            [Tooltip("Axis name for the Mouse X axis.")]
            private string mouseXAxis = "Mouse X";
            /// <summary>
            /// Axis name for the Mouse X axis.
            /// </summary>
            public string MouseXAxis
            {
                get { return mouseXAxis; }
                set { mouseXAxis = value; }
            }
            [SerializeField]
            [Tooltip("Axis name for the Mouse Y axis.")]
            private string mouseYAxis = "Mouse Y";
            /// <summary>
            /// Axis name for the Mouse Y axis.
            /// </summary>
            public string MouseYAxis
            {
                get { return mouseYAxis; }
                set { mouseYAxis = value; }
            }
            [SerializeField]
            [Tooltip("The sensitivity of the mouse.")]
            private float mouseSensitivity = 1f;
            /// <summary>
            /// The sensitivity of the mouse.
            /// </summary>
            public float MouseSensitivity
            {
                get { return mouseSensitivity; }
                set { mouseSensitivity = value; }
            }
            [SerializeField]
            [Range(0f, 1f)]
            [Tooltip("Smoothness of the mouse movement.")]
            private float mouseSmoothing = 0f;
            /// <summary>
            /// Smoothness of the mouse movement.
            /// </summary>
            public float MouseSmoothing
            {
                get { return mouseSmoothing; }
                set { mouseSmoothing = Mathf.Clamp01(value); }
            }

            [Header("Button & Axis Input")]
            [SerializeField]
            [Tooltip("Axis name for the Horizontal axis.")]
            private string horizontalAxis = "Horizontal";
            /// <summary>
            /// Axis name for the Horizontal axis.
            /// </summary>
            public string HorizontalAxis
            {
                get { return horizontalAxis; }
                set { horizontalAxis = value; }
            }

            [SerializeField]
            [Tooltip("Axis name for the Vertical axis.")]
            private string verticalAxis = "Vertical";
            /// <summary>
            /// Axis name for the Vertical axis.
            /// </summary>
            public string VerticalAxis
            {
                get { return verticalAxis; }
                set { verticalAxis = value; }
            }

            [SerializeField]
            [Tooltip("Button name for the Jump button.")]
            private string jumpButton = "Jump";
            /// <summary>
            /// Button name for the Jump button.
            /// </summary>
            public string JumpButton
            {
                get { return jumpButton; }
                set { jumpButton = value; }
            }

            [SerializeField]
            [Tooltip("Button name for the Sprint button.")]
            private string sprintButton = "Sprint";
            /// <summary>
            /// Button name for the Sprint button.
            /// </summary>
            public string SprintButton
            {
                get { return sprintButton; }
                set { sprintButton = value; }
            }

            [SerializeField]
            [Tooltip("Button name for the Crouch button.")]
            private string crouchButton = "Crouch";
            /// <summary>
            /// Button name for the Crouch button.
            /// </summary>
            public string CrouchButton
            {
                get { return crouchButton; }
                set { crouchButton = value; }
            }
        }

        /// <inheritdoc />
        /// <summary>
        /// Player collision settings.
        /// </summary>
        [Serializable]
        public class CollisionSettings : Settings
        {
            public CollisionSettings(Player player) : base(player) { }

            [SerializeField]
            [Tooltip("Enables or disables overlap recovery. Used to depenetrate character controllers from static objects when an overlap is detected.")]
            private bool enableOverlapRecovery = true;
            /// <summary>
            /// Enables or disables overlap recovery. Used to depenetrate character controllers from static objects when an overlap is detected.
            /// </summary>
            public bool EnableOverlapRecovery
            {
                get { return enableOverlapRecovery; }
                set { Player.CharacterController.enableOverlapRecovery = enableOverlapRecovery = value; }
            }
            [Space]
            [SerializeField]
            [Tooltip("If enabled, applies an extremely subtle noisy movement pass to the Player to allow for continuous collision detection in all cases.")]
            private bool continuousCollisionDetection = true;
            /// <summary>
            /// If enabled, applies an extremely subtle noisy movement pass to the Player to allow for continuous collision detection in all cases.
            /// </summary>
            public bool ContinuousCollisionDetection
            {
                get { return continuousCollisionDetection; }
                set { continuousCollisionDetection = value; }
            }
            [SerializeField, Range(0.0001f, 0.1f)]
            [Tooltip("The strength of the continuous detection movement pass. High value assures better collisions but more inaccurate movement. 0.01 is a good value in most cases.")]
            private float continuousCollisionDetectionStrength = 0.01f;
            /// <summary>
            /// The strength of the continuous detection movement pass. High value assures better collisions but more inaccurate movement. 0.01 is a good value in most cases.
            /// </summary>
            public float ContinuousCollisionDetectionStrength
            {
                get { return continuousCollisionDetectionStrength; }
                set { continuousCollisionDetectionStrength = Mathf.Clamp(value, 0.0001f, 0.1f); }
            }
            [SerializeField, Range(1, 8)]
            [Tooltip("The amount of continuous collision passes. High value assures better collisions but is more expensive to compute and character's skin width feels \"thicker\". 1 is the recommended value.")]
            private int continuousCollisionDetectionPasses = 1;
            /// <summary>
            /// The amount of continuous collision passes. High value assures better collisions but is more expensive to compute and character's skin width feels "thicker". 1 is the recommended value.
            /// </summary>
            public int ContinuousCollisionDetectionPasses
            {
                get { return continuousCollisionDetectionPasses; }
                set { continuousCollisionDetectionPasses = Mathf.Clamp(value, 1, 8); }
            }
        }

        /// <inheritdoc />
        /// <summary>
        /// Player movement settings.
        /// </summary>
        [Serializable]
        public class MovementSettings : Settings
        {
            public MovementSettings(Player player) : base(player) { }

            [SerializeField]
            [Tooltip("Should we allow mouse movement?")]
            private bool allowMouseMove = true;
            /// <summary>
            /// Should we allow mouse movement?
            /// </summary>
            public bool AllowMouseMove
            {
                get { return allowMouseMove; }
                set { allowMouseMove = value; }
            }
            [SerializeField]
            [Tooltip("Should we allow character movement?")]
            private bool allowMovement = true;
            /// <summary>
            /// Should we allow character movement?
            /// </summary>
            public bool AllowMovement
            {
                get { return allowMovement; }
                set { allowMovement = value; }
            }
            [Header("Speed & Acceleration")]
            [SerializeField]
            [Tooltip("The base walking speed.")]
            private float speed = 3.5f;
            /// <summary>
            /// The base walking speed.
            /// </summary>
            public float Speed
            {
                get { return speed; }
                set { speed = value; }
            }
            [SerializeField, Range(0.0001f, 1000f)]
            [Tooltip("The acceleration amount.")]
            private float acceleration = 8f;
            /// <summary>
            /// The acceleration amount.
            /// </summary>
            public float Acceleration
            {
                get { return acceleration; }
                set { acceleration = Mathf.Clamp(value, 0.0001f, 1000f); }
            }
            [SerializeField, Range(0.0001f, 1000f)]
            [Tooltip("The deceleration amount.")]
            private float deceleration = 8f;
            /// <summary>
            /// The deceleration amount.
            /// </summary>
            public float Deceleration
            {
                get { return deceleration; }
                set { deceleration = Mathf.Clamp(value, 0.0001f, 1000f); }
            }
            [SerializeField]
            [Tooltip("The slope speed multiplier curve, ranges from [0 .. 1] in the horizontal axis mapped to [0 .. Slope Limit], vertical axis is the speed multiplier to apply.")]
            private AnimationCurve slopeSpeedMultiplierCurve = new AnimationCurve(new Keyframe(0.5f, 1f), new Keyframe(1f, 0f));
            /// <summary>
            /// The slope speed multiplier curve, ranges from [0 .. 1] in the horizontal axis mapped to [0 .. <see cref="Player.SlopeLimit"/>], vertical axis is the speed multiplier to apply.
            /// </summary>
            public AnimationCurve SlopeSpeedMultiplierCurve
            {
                get { return slopeSpeedMultiplierCurve; }
                set { slopeSpeedMultiplierCurve = value; }
            }
            [SerializeField]
            private float maximumSpeed = 10f;
            /// <summary>
            /// The maximum speed the player may travel in the horizontal plane
            /// </summary>
            public float MaxSpeed
            {
                get { return maximumSpeed; }
                set { maximumSpeed = value; }
            }
            [Header("Air Control & Strafing")]
            [SerializeField]
            [Tooltip("Should we allow air control?")]
            private bool airControl = true;
            /// <summary>
            /// Should we allow air control?
            /// </summary>
            public bool AirControl
            {
                get { return airControl; }
                set { airControl = value; }
            }
            [SerializeField, Range(0f, 1000f)]
            [Tooltip("The air acceleration amount.")]
            private float airAccelerate = 10f;
            /// <summary>
            /// The air acceleration amount.
            /// </summary>
            public float AirAccelerate
            {
                get { return airAccelerate; }
                set { airAccelerate = Mathf.Clamp(value, 0f, 1000f); }
            }
            [SerializeField, Space]
            [Tooltip("Should we allow smart strafing? (Similar to Source Engine strafing)")]
            private bool smartStrafing = true;
            /// <summary>
            /// Should we allow smart strafing? (Similar to Source Engine strafing, works with <see cref="AirControl"/> and <see cref="AirAccelerate"/> only)
            /// </summary>
            public bool SmartStrafing
            {
                get { return smartStrafing; }
                set { smartStrafing = value; }
            }
            [SerializeField]
            [Tooltip("Should we allow vertical strafing? (Z local axis)")]
            private bool verticalStrafing = true;
            /// <summary>
            /// Should we allow vertical strafing? (Z local axis)
            /// </summary>
            public bool VerticalStrafing
            {
                get { return verticalStrafing; }
                set { verticalStrafing = value; }
            }
            [SerializeField]
            [Tooltip("Should we allow vertical strafing? (X local axis)")]
            private bool horizontalStrafing = true;
            /// <summary>
            /// Should we allow vertical strafing? (X local axis)
            /// </summary>
            public bool HorizontalStrafing
            {
                get { return horizontalStrafing; }
                set { horizontalStrafing = value; }
            }
            [SerializeField]
            [Tooltip("The strafing speed for horizontal and vertical strafing. (Smart strafing not included)")]
            private float strafingSpeed = 1f;
            /// <summary>
            /// The strafing speed for horizontal and vertical strafing. (Smart strafing not included)
            /// </summary>
            public float StrafingSpeed
            {
                get { return strafingSpeed; }
                set { strafingSpeed = value; }
            }

            [Header("Other movement")]
            [SerializeField]
            private JumpSettings jumpSettings;
            /// <summary>
            /// Jump settings.
            /// </summary>
            public JumpSettings Jump
            {
                get { return jumpSettings; }
                set { jumpSettings = value; }
            }
            [SerializeField]
            private SprintSettings sprintSettings;
            /// <summary>
            /// Sprint settings.
            /// </summary>
            public SprintSettings Sprint
            {
                get { return sprintSettings; }
                set { sprintSettings = value; }
            }
            [SerializeField]
            private CrouchSettings crouchSettings;
            /// <summary>
            /// Crouch settings.
            /// </summary>
            public CrouchSettings Crouch
            {
                get { return crouchSettings; }
                set { crouchSettings = value; }
            }

            [Serializable]
            public class JumpSettings : Settings
            {
                public JumpSettings(Player player) : base(player) { }

                [SerializeField]
                [Tooltip("Should we allow jumping?")]
                private bool allowJump = true;
                /// <summary>
                /// Should we allow jumping?
                /// </summary>
                public bool AllowJump
                {
                    get { return allowJump; }
                    set { allowJump = value; }
                }
                [SerializeField, Space]
                [Tooltip("Should we allow auto-jumping?")]
                private bool autoJump;
                /// <summary>
                /// Should we allow auto-jumping?
                /// </summary>
                public bool AutoJump
                {
                    get { return autoJump; }
                    set { autoJump = value; }
                }

                [SerializeField]
                [Tooltip("The power of the jump.")]
                private float power = 3f;
                /// <summary>
                /// The power of the jump.
                /// </summary>
                public float Power
                {
                    get { return power; }
                    set { power = value; }
                }
            }

            [Serializable]
            public class SprintSettings : Settings
            {
                public SprintSettings(Player player) : base(player) { }

                [SerializeField]
                [Tooltip("Should we allow sprinting?")]
                private bool allowSprint = true;
                /// <summary>
                /// Should we allow sprinting?
                /// </summary>
                public bool AllowSprint
                {
                    get { return allowSprint; }
                    set { allowSprint = value; }
                }
                [SerializeField, Space]
                [Tooltip("The speed multiplier of the base speed when sprinting.")]
                private float speedMultiplier = 2f;
                /// <summary>
                /// The speed multiplier of the base speed when sprinting.
                /// </summary>
                public float SpeedMultiplier
                {
                    get { return speedMultiplier; }
                    set { speedMultiplier = value; }
                }
                public float MaxSpeed
                {
                    get { return MaxSpeed; }
                    set { MaxSpeed = value; }
                }
            }

            [Serializable]
            public class CrouchSettings : Settings
            {
                public CrouchSettings(Player player) : base(player) { }

                [SerializeField]
                [Tooltip("Should we allow crouching?")]
                private bool allowCrouch = true;
                /// <summary>
                /// Should we allow crouching?
                /// </summary>
                public bool AllowCrouch
                {
                    get { return allowCrouch; }
                    set { allowCrouch = value; }
                }

                [SerializeField, Space]
                [Tooltip("Should we allow crouch-jumping?")]
                private bool allowCrouchJump;
                /// <summary>
                /// Should we allow crouch-jumping?
                /// </summary>
                public bool AllowCrouchJump
                {
                    get { return allowCrouchJump; }
                    set { allowCrouchJump = value; }
                }
                [SerializeField, Space]
                [Tooltip("The height of the player when crouching.")]
                private float crouchHeight = 0.8f;
                /// <summary>
                /// The height of the player when crouching.
                /// </summary>
                public float CrouchHeight
                {
                    get { return crouchHeight; }
                    set { crouchHeight = value; }
                }
                [SerializeField]
                [Tooltip("The camera height of the player when crouching.")]
                private float crouchCameraHeight = 0.6f;
                /// <summary>
                /// The camera height of the player when crouching.
                /// </summary>
                public float CrouchCameraHeight
                {
                    get { return crouchCameraHeight; }
                    set { crouchCameraHeight = value; }
                }
                [SerializeField, Space]
                [Tooltip("The speed of the crouching action.")]
                private float crouchingSpeed = 7.5f;
                /// <summary>
                /// The speed of the crouching action.
                /// </summary>
                public float CrouchingSpeed
                {
                    get { return crouchingSpeed; }
                    set { crouchingSpeed = value; }
                }
                [SerializeField]
                [Tooltip("The speed multiplier of the base speed when crouching.")]
                private float speedMultiplier = 0.75f;
                /// <summary>
                /// The speed multiplier of the base speed when crouching.
                /// </summary>
                public float SpeedMultiplier
                {
                    get { return speedMultiplier; }
                    set { speedMultiplier = value; }
                }
            }
        }

        /// <inheritdoc />
        /// <summary>
        /// Player footstep settings.
        /// </summary>
        [Serializable]
        public class FootstepSettings : Settings
        {
            public FootstepSettings(Player player) : base(player) { }

            [SerializeField]
            [Tooltip("The footstep asset to use.")]
            private FootstepAsset footstepAsset;
            /// <summary>
            /// The footstep asset to use.
            /// </summary>
            public FootstepAsset Asset
            {
                get { return footstepAsset; }
                set { footstepAsset = value; }
            }

            [SerializeField, Space]
            [Tooltip("Are the footsteps generated in 3D spatial blending?")]
            private bool is3D = true;
            /// <summary>
            /// Are the footsteps generated in 3D spatial blending?
            /// </summary>
            public bool Is3D
            {
                get { return is3D; }
                set { is3D = value; }
            }
            [SerializeField, Range(0f, 1f)]
            [Tooltip("The global volume of the footsteps.")]
            private float globalVolume = 1f;
            /// <summary>
            /// The global volume of the footsteps.
            /// </summary>
            public float GlobalVolume
            {
                get { return globalVolume; }
                set { globalVolume = Mathf.Clamp01(value); }
            }
            [SerializeField]
            [Tooltip("The audio range of the footsteps. Roll-off will be linear. Any value equal or less than zero disables the range and uses default logarithmic roll-off. If \"Is 3D\" is disabled, then this value is ignored.")]
            private float range = -1f;
            /// <summary>
            /// The audio range of the footsteps. Roll-off will be linear. Any value equal or less than zero disables the range and uses default logarithmic roll-off. If <see cref="Is3D"/> is disabled, then this value is ignored.
            /// </summary>
            public float Range
            {
                get { return range; }
                set { range = value; }
            }
        }
    }
}

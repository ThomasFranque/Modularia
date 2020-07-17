using System;
using Enhancements;
using UnityEngine;
using Weapons;


namespace Character
{
    public class PlayerControl : MonoBehaviour
    {
        private const float MAX_Y_VELOCITY = 35f;
        public static bool LockMovement { get; set; }

        [ReadOnly, SerializeField] private Vector3 _motion = default;

        [SerializeField] private float _walkingSpeed = 7.5f;
        [SerializeField] private float _runningSpeed = 11.5f;
        [SerializeField] private float _jumpSpeed = 8.0f;
        [SerializeField] private float _gravity = 20.0f;
        [SerializeField] private float _lookSpeed = 2.0f;
        [SerializeField] private float _lookXLimit = 45.0f;
        [SerializeField] private Camera playerCamera = default;

        private CharacterController _cc;
        private Gun _gun;
        private bool _lastGroundedState;
        private float _crouchAmount;
        float _rotationX = 0;

        #region Inputs
        public Vector3 Motion { get => _motion; set { _motion = value; } }
        private Vector2 _inputAxis;
        private Vector2 _mouseAxis;
        public float InputsMagnitude { get; private set; }
        public float Gravity => _gravity;
        public Vector2 InputAxis => _inputAxis;
        public Vector2 MouseAxis => _mouseAxis;
        public bool JumpInput { get; private set; }
        public bool SprintInput { get; private set; }
        public bool FocusInput { get; private set; }
        public bool FocusIntention { get; private set; }
        public bool UnFocusIntention { get; private set; }
        public bool AttackInput { get; private set; }
        public bool AttackIntention { get; private set; }
        public bool CrouchInput { get; private set; }
        public bool HopRightInput { get; private set; }
        public bool HopLeftInput { get; private set; }
        #endregion

        public Vector3 Velocity => _cc.velocity;
        public float JumpSpeed => _jumpSpeed;
        public bool Grounded => _cc.isGrounded;
        public bool Sprinting => SprintInput && Grounded;

        private void Awake()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            _cc = GetComponent<CharacterController>();
            _gun = GetComponentInChildren<Gun>();

            _inputAxis = Vector2.zero;
            _motion = Vector3.zero;
        }

        // Update is called once per frame
        void Update()
        {
            if (!LockMovement)
                GetInputsAndStates();
            else
                GetInputDefaults();
            MoveAndRotate();
            ListenersUpdate();
        }

        private void GetInputsAndStates()
        {
            _inputAxis.x = Input.GetAxis("Horizontal");
            _inputAxis.y = Input.GetAxis("Vertical");
            _mouseAxis.x = Input.GetAxis("Mouse X");
            _mouseAxis.y = Input.GetAxis("Mouse Y");

            AttackIntention = Input.GetButtonDown("Fire1");
            AttackInput = Input.GetButton("Fire1");
            JumpInput = Input.GetButton("Jump");
            FocusInput = Input.GetMouseButton(1);
            FocusIntention = Input.GetMouseButtonDown(1);
            UnFocusIntention = Input.GetMouseButtonUp(1);
            SprintInput = Input.GetKey(KeyCode.LeftShift);
            CrouchInput = Input.GetKey(KeyCode.LeftControl);
            HopRightInput = Input.GetKey(KeyCode.E);
            HopLeftInput = Input.GetKey(KeyCode.Q);
            InputsMagnitude = InputAxis.magnitude;
        }

        private void GetInputDefaults()
        {
            _inputAxis.x = default;
            _inputAxis.y = default;
            _mouseAxis.x = default;
            _mouseAxis.y = default;

            AttackIntention = default;
            AttackInput = default;
            JumpInput = default;
            FocusInput = default;
            FocusIntention = default;
            UnFocusIntention = default;
            SprintInput = default;
            CrouchInput = default;
            HopRightInput = default;
            HopLeftInput = default;
            InputsMagnitude = default;
        }

        private void MoveAndRotate()
        {
            Vector3 characterRotation = new Vector3(0, MouseAxis.x, 0);
            Rotate(characterRotation);

            // We are grounded, so recalculate move direction based on axes
            Vector3 forward = transform.TransformDirection(Vector3.forward);
            Vector3 right = transform.TransformDirection(Vector3.right);
            // Press Left Shift to run
            bool isRunning = Input.GetKey(KeyCode.LeftShift);
            float curSpeedX = (isRunning ? _runningSpeed : _walkingSpeed) * _inputAxis.y;
            float curSpeedY = (isRunning ? _runningSpeed : _walkingSpeed) * _inputAxis.x;
            curSpeedX += (PlayerEnhancementsHandler.Instance.GenericEnhancements.AllModules[4].Value * curSpeedX);
            curSpeedY += (PlayerEnhancementsHandler.Instance.GenericEnhancements.AllModules[4].Value * curSpeedY);
            float movementDirectionY = Motion.y;
            Vector3 moveDirection = (forward * curSpeedX) + (right * curSpeedY);

            if (JumpInput && Grounded)
            {
                moveDirection.y = JumpSpeed;
            }
            else if (Grounded)
            {
                moveDirection.y = -Gravity * Time.deltaTime;
            }
            else
            {
                moveDirection.y = movementDirectionY;
                moveDirection.y -= Gravity * Time.deltaTime;
            }

            // Apply gravity. Gravity is multiplied by deltaTime twice (once here, and once below
            // when the moveDirection is multiplied by deltaTime). This is because gravity should be applied
            // as an acceleration (ms^-2)

            _motion = moveDirection;
            // Move the controller
            MoveCharacterController(_motion * Time.deltaTime);

            // Player and Camera rotation
            _rotationX += -MouseAxis.y * _lookSpeed;
            _rotationX = Mathf.Clamp(_rotationX, -_lookXLimit, _lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(_rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, MouseAxis.x * _lookSpeed, 0);
        }

        private void ListenersUpdate()
        {
            if (!_lastGroundedState && Grounded) Landed();
            if (AttackInput) TriggerAttack();
            if (FocusIntention || UnFocusIntention) Focus(FocusIntention);

            _lastGroundedState = Grounded;

            void Landed()
            {
                OnLand?.Invoke();
            }

            void TriggerAttack()
            {
                _gun.Shoot();
            }

            void Focus(bool state)
            {
                if (state)
                    _gun.Focus();
                else
                    _gun.UnFocus();
            }
        }

        public void MoveCharacterController(Vector3 motion)
        {
            _cc.Move(motion);
        }

        public void SetRotation(Quaternion newRot)
        {
            transform.rotation = newRot;
        }
        public void Rotate(Vector3 rotation)
        {
            transform.Rotate(rotation);
        }

        public Action OnJump;
        public Action OnLand;
        public Action OnHop;
        public Action OnAttack;
        public Action OnStartJumpDescend;
    }
}
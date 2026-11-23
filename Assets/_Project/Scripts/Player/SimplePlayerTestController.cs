using UnityEngine;

namespace LagosStreetLeague.Player
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(CharacterController))]
    public class SimplePlayerTestController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform referenceCamera;
        [SerializeField] private Animator animator;

        [Header("Movement & Rotation")]
        [SerializeField, Min(0.1f)] private float jogSpeed = 5.0f;
        [SerializeField, Min(0.1f)] private float sprintSpeed = 8.2f;
        [SerializeField, Min(1f)] private float turnSpeedDegPerSec = 720f;
        [SerializeField, Range(0.01f, 0.5f)] private float moveDeadzone = 0.12f;
        [SerializeField] private float gravity = -20f;

        [Header("Mobile Joystick & Keyboard")]
        [SerializeField] private bool enableKeyboardDebug = true;
        [SerializeField, Range(0.5f, 1f)]
        private float joystickSprintThreshold = 0.92f;

        [Header("Animator Parameter Names")]
        [SerializeField] private string speedParamName = "MoveSpeed";
        [SerializeField] private string isSprintingParamName = "IsSprinting";
        [SerializeField] private string kickTriggerName = "Kick";
        [SerializeField, Min(0.01f)] private float speedDampTime = 0.10f;

        private CharacterController _characterController;
        private Vector2 _joystickInput;
        private bool _joystickSprintHeld;
        private float _verticalVelocity;

        private int _speedHash;
        private int _isSprintingHash;
        private int _kickHash;

        private bool _hasSpeedParam;
        private bool _hasSprintParam;
        private bool _hasKickParam;

        private void Awake()
        {
            _characterController = GetComponent<CharacterController>();

            if (animator == null)
                animator = GetComponentInChildren<Animator>();

            if (referenceCamera == null && Camera.main != null)
                referenceCamera = Camera.main.transform;

            CacheAnimatorHashes();
        }

        private void CacheAnimatorHashes()
        {
            _speedHash = Animator.StringToHash(speedParamName);
            _isSprintingHash = Animator.StringToHash(isSprintingParamName);
            _kickHash = Animator.StringToHash(kickTriggerName);

            if (animator == null)
                return;

            AnimatorControllerParameter[] parameters = animator.parameters;

            for (int i = 0; i < parameters.Length; i++)
            {
                int hash = parameters[i].nameHash;

                if (hash == _speedHash &&
                    parameters[i].type == AnimatorControllerParameterType.Float)
                {
                    _hasSpeedParam = true;
                }
                else if (hash == _isSprintingHash &&
                         parameters[i].type == AnimatorControllerParameterType.Bool)
                {
                    _hasSprintParam = true;
                }
                else if (hash == _kickHash &&
                         parameters[i].type == AnimatorControllerParameterType.Trigger)
                {
                    _hasKickParam = true;
                }
            }
        }

        public void SetJoystickInput(Vector2 input, bool sprint = false)
        {
            _joystickInput = Vector2.ClampMagnitude(input, 1f);
            _joystickSprintHeld = sprint;
        }

        public void TriggerKickAnimation()
        {
            if (animator != null && _hasKickParam)
                animator.SetTrigger(_kickHash);
        }

        private void Update()
        {
            float dt = Time.deltaTime;

            if (dt <= 0f)
                return;

            Vector2 rawInput = _joystickInput;
            bool sprintPressed = _joystickSprintHeld;

            if (enableKeyboardDebug)
            {
                float h = Input.GetAxisRaw("Horizontal");
                float v = Input.GetAxisRaw("Vertical");

                Vector2 keyInput = new Vector2(h, v);

                if (keyInput.sqrMagnitude > rawInput.sqrMagnitude)
                    rawInput = Vector2.ClampMagnitude(keyInput, 1f);

                if (Input.GetKey(KeyCode.LeftShift) ||
                    Input.GetKey(KeyCode.RightShift))
                {
                    sprintPressed = true;
                }

                if (Input.GetKeyDown(KeyCode.Space))
                    TriggerKickAnimation();
            }

            float magnitude = rawInput.magnitude;
            bool hasMoveInput = magnitude >= moveDeadzone;

            Vector3 moveDir = Vector3.zero;

            if (hasMoveInput)
            {
                Vector2 normalizedInput = rawInput / magnitude;
                moveDir = ComputeWorldMoveDirection(normalizedInput);

                if (moveDir.sqrMagnitude > 0.0001f)
                {
                    Quaternion targetRot = Quaternion.LookRotation(
                        moveDir, Vector3.up);

                    transform.rotation = Quaternion.RotateTowards(
                        transform.rotation,
                        targetRot,
                        turnSpeedDegPerSec * dt);
                }
            }

            bool isSprinting = hasMoveInput &&
                (sprintPressed || magnitude >= joystickSprintThreshold);

            float targetSpeed = 0f;

            if (hasMoveInput)
            {
                float speedScale = isSprinting
                    ? 1f
                    : Mathf.InverseLerp(moveDeadzone, 1f, magnitude);

                float baseSpeed = isSprinting ? sprintSpeed : jogSpeed;

                targetSpeed = baseSpeed *
                    Mathf.Clamp(speedScale, 0.35f, 1f);
            }

            if (_characterController.isGrounded && _verticalVelocity < 0f)
            {
                _verticalVelocity = -2f;
            }
            else
            {
                _verticalVelocity += gravity * dt;
            }

            Vector3 horizontalVelocity = moveDir * targetSpeed;

            Vector3 totalDisplacement =
                (horizontalVelocity + Vector3.up * _verticalVelocity) * dt;

            _characterController.Move(totalDisplacement);

            UpdateAnimator(
                horizontalVelocity.magnitude,
                isSprinting,
                dt);
        }

        private Vector3 ComputeWorldMoveDirection(Vector2 stickInput)
        {
            if (referenceCamera != null)
            {
                Vector3 camForward = referenceCamera.forward;
                camForward.y = 0f;

                if (camForward.sqrMagnitude < 0.0001f)
                    camForward = Vector3.forward;
                else
                    camForward.Normalize();

                Vector3 camRight = new Vector3(
                    camForward.z, 0f, -camForward.x);

                Vector3 dir =
                    camRight * stickInput.x +
                    camForward * stickInput.y;

                return dir.sqrMagnitude > 0.0001f
                    ? dir.normalized
                    : Vector3.zero;
            }

            return new Vector3(
                stickInput.x, 0f, stickInput.y).normalized;
        }

        private void UpdateAnimator(
            float currentHorizontalSpeed,
            bool isSprinting,
            float dt)
        {
            if (animator == null)
                return;

            if (_hasSpeedParam)
            {
                float normalizedSpeed = Mathf.Clamp01(
                    currentHorizontalSpeed /
                    Mathf.Max(0.01f, sprintSpeed));

                animator.SetFloat(
                    _speedHash,
                    normalizedSpeed,
                    speedDampTime,
                    dt);
            }

            if (_hasSprintParam)
                animator.SetBool(_isSprintingHash, isSprinting);
        }
    }
}

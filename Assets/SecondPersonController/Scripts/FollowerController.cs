using UnityEngine;

namespace SecondPersonController.Scripts
{
    [RequireComponent(typeof(CharacterController))]
    public class ObserverController : MonoBehaviour
    {
        #region Fields

        #region Public

        public Transform PlayerTransform;
        public float FollowDistance = 4f;

        public float SprintDistanceThreshold = 10f;
        public float MoveSpeed = 2.0f;
        public float SprintSpeed = 5.335f;
        public float RotationSmoothTime = 0.12f;
        public float SpeedChangeRate = 10.0f;

        public float Gravity = -15.0f;
        public float FallTimeout = 0.15f;
        public float GroundedOffset = -0.14f;
        public float GroundedRadius = 0.28f;
        public LayerMask GroundLayers;
        public bool Grounded = true;

        public AudioClip LandingAudioClip;
        public AudioClip[] FootstepAudioClips;
        [Range(0, 1)] public float FootstepAudioVolume = 0.5f;

        #endregion Public

        #region Private

        private float _speed;
        private float _animationBlend;
        private float _rotationVelocity;
        private float _verticalVelocity;
        private float _fallTimeoutDelta;
        private const float _terminalVelocity = 53.0f;
        private float _targetRotation;

        private Animator _animator;

        private CharacterController _controller;

        private int _animIDSpeed;
        private int _animIDGrounded;
        private int _animIDJump;
        private int _animIDFreeFall;
        private int _animIDMotionSpeed;

        #endregion Private

        #endregion Fields

        #region Unity events

        private void Start()
        {
            _controller = GetComponent<CharacterController>();
            _animator = GetComponent<Animator>();
            AssignAnimationIDs();
            _fallTimeoutDelta = FallTimeout;
        }

        private void Update()
        {
            GroundedCheck();
            HandleGravity();
            FollowPlayer();
        }

        #endregion Unity events

        #region Methods

        private void AssignAnimationIDs()
        {
            _animIDSpeed = Animator.StringToHash("Speed");
            _animIDGrounded = Animator.StringToHash("Grounded");
            _animIDJump = Animator.StringToHash("Jump");
            _animIDFreeFall = Animator.StringToHash("FreeFall");
            _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
        }

        private void GroundedCheck()
        {
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
            Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);

            if (_animator)
                _animator.SetBool(_animIDGrounded, Grounded);
        }

        private void HandleGravity()
        {
            if (Grounded)
            {
                _fallTimeoutDelta = FallTimeout;

                if (_verticalVelocity < 0.0f)
                    _verticalVelocity = -2f;

                if (_animator)
                {
                    _animator.SetBool(_animIDJump, false);
                    _animator.SetBool(_animIDFreeFall, false);
                }
            }
            else
            {
                _fallTimeoutDelta -= Time.deltaTime;
                if (_fallTimeoutDelta <= 0.0f && _animator)
                    _animator.SetBool(_animIDFreeFall, true);
            }

            if (_verticalVelocity < _terminalVelocity)
                _verticalVelocity += Gravity * Time.deltaTime;
        }

        private void FollowPlayer()
        {
            if (!PlayerTransform) return;

            Vector3 directionToPlayer = PlayerTransform.position - transform.position;
            directionToPlayer.y = 0;
            float distance = directionToPlayer.magnitude;

            float targetSpeed = MoveSpeed;
            if (distance > SprintDistanceThreshold)
                targetSpeed = SprintSpeed;

            bool shouldMove = distance > FollowDistance;

            if (shouldMove)
            {
                _speed = Mathf.Lerp(_speed, targetSpeed, Time.deltaTime * SpeedChangeRate);
                _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);
            }
            else
            {
                _speed = Mathf.Lerp(_speed, 0, Time.deltaTime * SpeedChangeRate);
                _animationBlend = Mathf.Lerp(_animationBlend, 0, Time.deltaTime * SpeedChangeRate);
            }

            Vector3 move = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;
            _controller.Move(move.normalized * (_speed * Time.deltaTime) + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);

            Vector3 moveDir = directionToPlayer.normalized;

            if (moveDir.sqrMagnitude > 0.01f)
            {
                _targetRotation = Mathf.Atan2(moveDir.x, moveDir.z) * Mathf.Rad2Deg;
                float rotation = Mathf.SmoothDampAngle(
                    transform.eulerAngles.y,
                    _targetRotation,
                    ref _rotationVelocity,
                    RotationSmoothTime
                );
                transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
            }

            if (_animator)
            {
                _animator.SetFloat(_animIDSpeed, _animationBlend);
                _animator.SetFloat(_animIDMotionSpeed, 1f);
            }
        }

        #endregion Methods

        #region Events

        private void OnFootstep(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f && FootstepAudioClips.Length > 0)
            {
                var index = Random.Range(0, FootstepAudioClips.Length);
                AudioSource.PlayClipAtPoint(FootstepAudioClips[index], transform.TransformPoint(_controller.center), FootstepAudioVolume);
            }
        }

        private void OnLand(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
                AudioSource.PlayClipAtPoint(LandingAudioClip, transform.TransformPoint(_controller.center), FootstepAudioVolume);
        }

        private void OnDrawGizmosSelected()
        {
            Color color = Grounded ? new Color(0, 1, 0, 0.35f) : new Color(1, 0, 0, 0.35f);
            Gizmos.color = color;
            Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z), GroundedRadius);
        }

        #endregion Events
    }
}
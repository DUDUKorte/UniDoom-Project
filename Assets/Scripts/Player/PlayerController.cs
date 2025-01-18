using System.Text;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    // Public vars
    [Header ("Movement")]
    public float speed;
    public float sprintSpeed;
    public float maxSpeedAccumulator = 15f;
    public float speedAccumulator = 2f;
    public bool holdSprint;
    [Space]
    public float acceleration = 0.05f;
    public float deceleration = 0.05f;
    public float sprintAcceleration = 0.025f;
    public float sprintDeceleration = 0.02f;
    
    [Header ("Jump/Air")]
    public float jumpForce;
    public float airControl = 0.3f;
    public float gravity = 12.0f;
    public float jumpAfterSlideImpulse = 10.0f;
    public bool canJump;
    
    [Header ("Crouching")]
    public float crouchSpeed = 0.3f;
    public float crouchAcceleration = 0.3f;
    public float crouchDeceleration = 0.3f;
    public float crouchHeight = 0.3f;
    public float slideSpeed = 15.0f;
    
    [Space]
    public Transform headTransform;
    public DebugMenuController debugMenuController;
    
    public float BaseSprintSpeed { get; set; }
    
    // Private vars
    
    // Movement
    private CharacterController _cc;
    private PlayerCamera playerCamera;
    private GameObject _FPCamera;
    private Vector3 _movement;
    private Vector3 _direction;
    private Vector2 _moveInput;
    public float _groundVelocity { get; private set; }
    private float _baseHeight;
    private bool _isMoving;
    private float _maxSpeed;
    private bool _isSprinting;
    private bool _isCrouching;
    public bool _slide { get; private set; }
    private float _cAcceleration;
    private float _cDeceleration;
    private bool _wasGrounded;
    private bool _wasJumping;
    private bool _wasCrouching;
    private Vector2 _slideDirection;
    
    // Headbob
    private float _bobFreq = 1.7f;
    private float _bobAmp = 0.06f;
    private float _tBob = 0.0f;
    public float _headYOfffset { get; private set; }

    // DEBUG Log
    public StringBuilder DebugTextLog;
    
    void Start()
    {
        LockCursor();
        
        // Set default values
        _cc = GetComponent<CharacterController>();
        _maxSpeed = speed;
        _cAcceleration = acceleration;
        _cDeceleration = deceleration;
        _baseHeight = _cc.height;
        BaseSprintSpeed = sprintSpeed;
        DebugTextLog = new StringBuilder();
        _headYOfffset = headTransform.localPosition.y;
    }

    void Update()
    {
        CalculateGravity();
        
        // Gravity Debug
        //Debug.Log("movement: " + _movement.y + " | velocity: " + _cc.velocity.y + " | grounded: " + _cc.isGrounded);
        
        CalculateMovement();

        // Movement Debug
        //Debug.Log("Velocity: " + _cc.velocity.magnitude + " | x/z vel: " + new Vector2(_cc.velocity.x, _cc.velocity.z).magnitude);
        
        // Verify if is croushing in air
        if (_wasCrouching && _cc.isGrounded)
        {
            _wasCrouching = false;
            SendMessage("OnStartCrouching");
        }
        
        // Verify if is sliding
        if (_slide)
        {
            Slide();
        }
        
        // Crouch SmoothDamp
        if (_isCrouching && !Mathf.Approximately(_cc.height, crouchHeight))
        {
            _cc.height = Mathf.Lerp(_cc.height, crouchHeight, 0.03f);
        }
        else if(!Mathf.Approximately(_cc.height, _baseHeight))
        {
            _cc.height = Mathf.Lerp(_cc.height, _baseHeight, _groundVelocity <= 0f ? 1f : 0.03f);
        }
        
        // Apply movement
        _cc.Move(_movement * Time.deltaTime);
        
        // Update groundVelocity value
        _groundVelocity = new Vector2(_cc.velocity.x, _cc.velocity.z).magnitude;
        
        // Head bob
        if (_cc.isGrounded && !_slide)
        {
            _tBob += Time.deltaTime * _groundVelocity;
            headTransform.transform.localPosition = HeadBob(_tBob);
        }
    }

    private void FixedUpdate()
    {
        DebugTextLog.Clear();
        
        DebugTextLog.Append(
            $"ACCEL: {_cAcceleration:F3} | DECEL: {_cDeceleration:F3} | MAXSPEED: {_maxSpeed:F3} | VEL: {_cc.velocity.magnitude:F3} | " +
            $"G_VEL: {_groundVelocity:F3} | CROUCH: {_isCrouching} | HEIGHT: {_cc.height:F3} | SLIDE {_slide} | SPRINT {sprintSpeed:F3} | " +
            $"SLIDE.x: {_slideDirection.x:F3} | SLIDE.z: {_slideDirection.y:F3} | m.x: {_movement.x:F3} | m.z: {_movement.z:F3} | " +
            $"AP: {Mathf.Abs(_slideDirection.x - _movement.x)}");
        
        //Debug.Log(DebugTextLog);
    }
    
    // Handle TBOB Animation
    private Vector3 HeadBob(float time)
    {
        Vector3 pos = Vector3.zero;
        pos.y = Mathf.Sin(time * _bobFreq) * _bobAmp;
        pos.x = Mathf.Clamp(Mathf.Cos(time * _bobFreq / 2f) * _bobAmp, -0.08f, 0.08f);
        pos.y += _headYOfffset; // Heigth Offset
        return pos;
    }
    
    /* ============================================= Mouse Iteractions =============================================== */
    public void FreeCursor()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void LockCursor()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void UpdateSensivity(float senseX, float senseY)
    {
        if(playerCamera == null){ return; }
        
        playerCamera.sensitivityX = senseX;
        playerCamera.sensitivityY = senseY;
        playerCamera.OnUpdateSensivity();
    }

    public void setPlayerCamera(PlayerCamera playerCamera)
    {
        this.playerCamera = playerCamera;
    }

    /* ====================================== Handle Movement and Acceleration ======================================= */
    private void CalculateMovement()
    {
        // Check movement
        // Get movement direction
        _direction = (headTransform.right * _moveInput.x + headTransform.forward * _moveInput.y).normalized;
        _direction.y = 0; // Prevent fly when looking up/down

        if (_slide) { return; }

        if (_groundVelocity <= 1f && _isSprinting && _cc.isGrounded)
        {
            sprintSpeed = BaseSprintSpeed;
            OnSetSprintSpeed();
        }
        
        if (_cc.isGrounded)
        {
            if (_isMoving)
            {
                // Acceleration
                _movement.x = Mathf.Lerp(_movement.x, _direction.x * _maxSpeed, _cAcceleration);
                _movement.z = Mathf.Lerp(_movement.z, _direction.z * _maxSpeed, _cAcceleration);
            }
            else
            {
                // Decceleration
                _movement.x = Mathf.Lerp(_movement.x, 0.0f, _cDeceleration);
                _movement.z = Mathf.Lerp(_movement.z, 0.0f, _cDeceleration);
            }
        }else
        {
            // Air Control
            _movement.x = Mathf.Lerp(_movement.x, _direction.x * _maxSpeed, airControl);
            _movement.z = Mathf.Lerp(_movement.z, _direction.z * _maxSpeed, airControl);
        }
        
        //Debug.Log("Direction: " + direction +"\nMovement: " + _movement);
    }

    /* =========================================== Handle Gravity and jump =========================================== */
    private void CalculateGravity()
    {
        if(_cc.velocity.y > jumpForce * 0.7)
        {
            _wasJumping = true;
        }
        
        // wasGrounded fix fall accumulation bug
        // wasJumping fix cant jump when grounded bug
        if (!_cc.isGrounded && _wasGrounded && !_wasJumping) // Reset fall accumulation only if falling from edges
        {
            _movement.y = 0f;
            _wasGrounded = false;
        }else if (!_cc.isGrounded) // Apply gravity
        {
            _movement.y -= gravity * Time.deltaTime;
        }
        else // When on ground reset variables if necessary
        {
            _wasGrounded = true;
            _wasJumping = _wasJumping ? false : _wasJumping;
        }
    }
    
    /* ========================================== Handle Speed Setters ================================================ */
    private void OnSetSprintSpeed()
    {
        _maxSpeed = sprintSpeed;
        _cAcceleration = sprintAcceleration;
        _cDeceleration = sprintDeceleration;
    }

    private void OnSetCrouchSpeed()
    {
        _maxSpeed = crouchSpeed;
        _cAcceleration = crouchAcceleration;
        _cDeceleration = crouchDeceleration;
        _isSprinting = false;
    }

    private void OnSetWalkSpeed()
    {
        _maxSpeed = speed;
        _cAcceleration = acceleration;
        _cDeceleration = deceleration;
    }
    
    // =================================================================================================================
    // Handle Actions ==================================================================================================
    private void OnMove(InputValue value)
    {
        _moveInput = value.Get<Vector2>();
        _isMoving = !(_moveInput == Vector2.zero);
    }

    private void OnJump(InputValue value)
    {
        if ((_cc.isGrounded && canJump) || _slide)
        {
            _wasGrounded = false;
            _wasJumping = false;
            _movement.y = jumpForce;
            
            if (_isCrouching)
            {
                if (_slide && _groundVelocity >= slideSpeed/2f)
                {
                    OnJumpFromSlide();
                }
                else
                {
                    sprintSpeed = BaseSprintSpeed;
                }
            
                SendMessage("OnStopCrouching");
            }
        }
    }

    private void OnSprint(InputValue value)
    {
        if(!_cc.isGrounded){ return; }
        
        if (value.Get<float>() != 0f)
        {
            if(_isCrouching){ SendMessage("OnStopCrouching"); }
            sprintSpeed = BaseSprintSpeed;
            
            if (!holdSprint)
            {
                _isSprinting = !_isSprinting;
                if (_isSprinting)
                {
                    OnSetSprintSpeed();
                }
                else
                {
                    OnSetWalkSpeed();
                }
                return;
            }
            
            // Start Running
            OnSetSprintSpeed();
        }
        else if (holdSprint)
        {
            // Stop running
            OnSetWalkSpeed();
        }
    }

/* ============================================= Handle Crouch and Slide ============================================= */
    private void OnCrouch(InputValue value)
    {
        if (!_cc.isGrounded)
        {
            _wasCrouching = true;
            return;
        }
        
        if (_isCrouching)
        {
            SendMessage("OnStopCrouching");
            return;
        }
        
        OnStartCrouching();
    }

    private void OnStartCrouching() // START CROUCHING
    {
        _isCrouching = true;
        
        // Handle slide movement
        if (_isSprinting && _groundVelocity >= BaseSprintSpeed * 0.2f)
        {
            _slide = true;
            // Gambiarra temporário TODO
            _movement.x = _direction.x * (slideSpeed + _groundVelocity);
            _movement.z = _direction.z * (slideSpeed + _groundVelocity);
            _slideDirection.x = crouchSpeed * _direction.x;
            _slideDirection.y = crouchSpeed * _direction.z;
        }
        else
        {
            _isSprinting = false;
            OnSetCrouchSpeed();
        }
    }
    
    private void OnStopCrouching() // STOP CROUCHING
    {
        _isCrouching = false;
        _slide = false;
        
        if (_isSprinting)
        {
            OnSetSprintSpeed();
        }
        else
        {
            OnSetWalkSpeed();
        }
    }

    private void Slide() // Handle Slide
    {
        // Deceleration
        _movement.x = Mathf.Lerp(_movement.x, _slideDirection.x, 0.003f);
        _movement.z = Mathf.Lerp(_movement.z, _slideDirection.y, 0.003f);
        
        if (Mathf.Abs(_movement.x - _slideDirection.x) <= 0.1f && Mathf.Abs(_movement.z - _slideDirection.y) <= 0.1f)
        {
            Debug.Log("SET CROUCH SPEED");
            _slide = false;
            OnSetCrouchSpeed();
        }
    }

    private void OnJumpFromSlide() // Handle Jump from slide and speed accumulation
    {
        // After slide acceleration
        _movement.x = _direction.x * (jumpAfterSlideImpulse + _groundVelocity);
        _movement.z = _direction.z * (jumpAfterSlideImpulse + _groundVelocity);
        
        // Speed increment from slide
        sprintSpeed += speedAccumulator;
        if (sprintSpeed > maxSpeedAccumulator)
        {
            sprintSpeed = maxSpeedAccumulator;
        }
    }
    
    // =================================================================================================================
    // Handle DEBUG Actions ==================================================================================================

    
    
}
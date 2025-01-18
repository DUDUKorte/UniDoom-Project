using UnityEngine;
using UnityEngine.InputSystem;
using Cursor = UnityEngine.Cursor;

public class DebugMenuController : MonoBehaviour
{
    public PlayerController player;
    
    private bool showDebugMenu = false;
    
    private float _padding;
    private float _boxHeigth;
    private float _boxWidth;
    private Vector2 _boxPos;
    private Vector2 _scrollPosition;
    
    float speed;
    float BaseSprintSpeed;
    float sprintSpeed;
    float maxSpeedAccumulator;
    float speedAccumulator;
    bool holdSprint;
    float acceleration;
    float deceleration;
    float sprintAcceleration;
    float sprintDeceleration;
    float jumpForce;
    float airControl;
    float gravity;
    bool canJump;
    float crouchSpeed;
    float crouchAcceleration;
    float crouchDeceleration;
    float crouchHeight;
    float slideSpeed;

    private void Start()
    {
        // Get original values
        //Original values
        speed = player.speed;
        BaseSprintSpeed = player.BaseSprintSpeed;
        sprintSpeed = player.sprintSpeed;
        maxSpeedAccumulator = player.maxSpeedAccumulator;
        speedAccumulator = player.speedAccumulator;
        holdSprint = player.holdSprint;
        acceleration = player.acceleration;
        deceleration = player.deceleration;
        sprintAcceleration = player.sprintAcceleration;
        sprintDeceleration = player.sprintDeceleration;
        jumpForce = player.jumpForce;
        airControl = player.airControl ;
        gravity = player.gravity;
        canJump = player.canJump;
        crouchSpeed = player.crouchSpeed;
        crouchAcceleration = player.crouchAcceleration;
        crouchDeceleration = player.crouchDeceleration;
        crouchHeight = player.crouchHeight;
        slideSpeed = player.slideSpeed;
    }
    
    public void OnToggleDebug(InputValue value)
    {
        showDebugMenu = !showDebugMenu;
    }

    public void OnToggleDebugMouse(InputValue value)
    {
        Cursor.visible = !Cursor.visible;
        Cursor.lockState = Cursor.lockState ==  CursorLockMode.Locked ? CursorLockMode.None : CursorLockMode.Locked;
    }
    
    public void OnGUI()
    {
        if(!showDebugMenu || player == null){ return; }
        
        // Variables getters
        /*player.speed 
        player.sprintSpeed
        player.maxSpeedAccumulator
        player.speedAccumulator
        player.holdSprint
        player.acceleration
        player.deceleration
        player.sprintAcceleration
        player.sprintDeceleration
        player.jumpForce
        player.airControl
        player.gravity
        player.canJump
        player.crouchSpeed
        player.crouchAcceleration
        player.crouchDeceleration
        player.crouchHeight
        player.slideSpeed*/
        
        // DEBUG Print
        float debugSize = 40f;
        Rect debugBox = new Rect(0, Screen.height - debugSize, Screen.width, debugSize);
        
        GUI.Box(debugBox, "");
        GUI.Label(debugBox, $"INFO: {player.DebugTextLog}");
        
        // GUI Settings reset
        _padding = 5;
        _boxHeigth = 20;
        _boxPos = new Vector2(_padding + 10, 10 + _padding);
        _boxWidth = (Screen.width * 0.25f) - (_padding * 2f);
        
        // Gui Start
        GUI.Box(new Rect(10, 10, Screen.width * 0.25f, Screen.height * 0.5f), "Debug Menu");
        
        // GUI Scroll
        _scrollPosition = GUI.BeginScrollView(new Rect(10, 10, Screen.width * 0.25f + 20, Screen.height * 0.5f), _scrollPosition, new Rect(10, 10, Screen.width * 0.25f - 2f,
            (Screen.height * 0.5f) * 3.2f));
        
        // Title Offset 
        _boxPos.y += _boxHeigth + _padding;
        
        player.speed = AddFloatSlider(player.speed, "speed (2*Shift to Apply)");
        _boxPos.y += _boxHeigth + _padding;
        player.BaseSprintSpeed = AddFloatSlider(player.BaseSprintSpeed, "BaseSprintSpeed");
        _boxPos.y += _boxHeigth + _padding;
        player.sprintSpeed = AddFloatSlider(player.sprintSpeed, "sprintSpeed");
        _boxPos.y += _boxHeigth + _padding;
        player.maxSpeedAccumulator = AddFloatSlider(player.maxSpeedAccumulator, "maxSpeedAccumulator");
        _boxPos.y += _boxHeigth + _padding;
        player.speedAccumulator = AddFloatSlider(player.speedAccumulator, "speedAccumulator");
        _boxPos.y += _boxHeigth + _padding;
        player.holdSprint = AddBoolOption(player.holdSprint, "holdSprint");
        _boxPos.y += _boxHeigth + _padding;
        player.acceleration = AddFloatSlider(player.acceleration, "acceleration");
        _boxPos.y += _boxHeigth + _padding;
        player.deceleration = AddFloatSlider(player.deceleration, "deceleration");
        _boxPos.y += _boxHeigth + _padding;
        player.sprintAcceleration = AddFloatSlider(player.sprintAcceleration, "sprintAcceleration");
        _boxPos.y += _boxHeigth + _padding;
        player.sprintDeceleration = AddFloatSlider(player.sprintDeceleration, "sprintDeceleration");
        _boxPos.y += _boxHeigth + _padding;
        player.jumpForce = AddFloatSlider(player.jumpForce, "jumpForce");
        _boxPos.y += _boxHeigth + _padding;
        player.airControl = AddFloatSlider(player.airControl, "airControl");
        _boxPos.y += _boxHeigth + _padding;
        player.gravity = AddFloatSlider(player.gravity, "gravity");
        _boxPos.y += _boxHeigth + _padding;
        player.canJump = AddBoolOption(player.canJump, "canJump");
        _boxPos.y += _boxHeigth + _padding;
        player.crouchSpeed = AddFloatSlider(player.crouchSpeed, "crouchSpeed");
        _boxPos.y += _boxHeigth + _padding;
        player.crouchAcceleration = AddFloatSlider(player.crouchAcceleration, "crouchAcceleration");
        _boxPos.y += _boxHeigth + _padding;
        player.crouchDeceleration = AddFloatSlider(player.crouchDeceleration, "crouchDeceleration");
        _boxPos.y += _boxHeigth + _padding;
        player.crouchHeight = AddFloatSlider(player.crouchHeight, "crouchHeight");
        _boxPos.y += _boxHeigth + _padding;
        player.slideSpeed = AddFloatSlider(player.slideSpeed, "slideSpeed");
        _boxPos.y += _boxHeigth + _padding;

        //GUI.Box(new Rect(_boxPos.x, _boxPos.y, _boxWidth, _boxHeigth), "Test");
        //GUI.TextArea(new Rect(_boxPos.x, _boxPos.y, _boxWidth, _boxHeigth), "TEXT 1");

        //GUI.Box(new Rect(_boxPos.x, _boxPos.y, _boxWidth, _boxHeigth), "Test 1");
        //_boxPos.y += _boxHeigth + _padding;

        //GUI.Box(new Rect(_boxPos.x, _boxPos.y, _boxWidth, _boxHeigth), "Test 2");
        GUI.EndScrollView();

        bool reset = GUI.Button(new Rect(10, Screen.height * 0.5f + 10f, Screen.width * 0.25f, 20), "Reset Values");

        if (reset)
        {
            ResetValues();
        }
    }

    private void ResetValues()
    {
        player.speed = speed;
        player.BaseSprintSpeed = BaseSprintSpeed;
        player.sprintSpeed = sprintSpeed;
        player.maxSpeedAccumulator = maxSpeedAccumulator;
        player.speedAccumulator = speedAccumulator;
        player.holdSprint = holdSprint;
        player.acceleration = acceleration;
        player.deceleration = deceleration;
        player.sprintAcceleration = sprintAcceleration;
        player.sprintDeceleration = sprintDeceleration;
        player.jumpForce = jumpForce;
        player.airControl = airControl ;
        player.gravity = gravity;
        player.canJump = canJump;
        player.crouchSpeed = crouchSpeed;
        player.crouchAcceleration = crouchAcceleration;
        player.crouchDeceleration = crouchDeceleration;
        player.crouchHeight = crouchHeight;
        player.slideSpeed = slideSpeed;
    }
    
    private float AddFloatSlider(float variable, string variableName)
    {
        GUI.Label(new Rect(_boxPos.x, _boxPos.y, _boxWidth, _boxHeigth), $"{variableName}: {variable:F3}");
        _boxPos.y += _boxHeigth + _padding;
        return GUI.HorizontalSlider(new Rect(_boxPos.x, _boxPos.y, _boxWidth, _boxHeigth), variable, -50f,
            100f);
    }
    
    private bool AddBoolOption(bool variable, string variableName)
    {
        GUI.Label(new Rect(_boxPos.x, _boxPos.y, _boxWidth, _boxHeigth), $"{variableName}: {variable}");
        _boxPos.y += _boxHeigth + _padding;
        return GUI.Toggle(new Rect(_boxPos.x, _boxPos.y, _boxWidth, _boxHeigth), variable, "Toggle");
    }
}

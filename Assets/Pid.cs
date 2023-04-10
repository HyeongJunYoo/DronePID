using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;
public class Pid : MonoBehaviour
{
    public GameObject ball;
    private Rigidbody _rigid;
    public float moveSpeed;
    public float rotateSpeed;
    public float rotateLimit;
    public bool isHovering;

    public Vector3 targetPos;
    public float posP, posI, posD;
    private Vector3 _posIntegral, _posLastError;
    
    public float rotP, rotI, rotD;
    private Vector3 _rotIntegral, _rotLastError;
    
    [Range(-180f, 180f)]
    public float targetRotationRoll;

    [Range(-180f, 180f)]
    public float targetRotationYaw;

    [Range(-180f, 180f)]
    public float targetRotationPitch;

    [Range(0f, 250f)]
    public float maxTorque = 30.0f;
    
    private DroneInputActions _droneControls;
    private InputAction _yaw;
    private InputAction _pitch;
    private InputAction _roll;
    private InputAction _power;
    private InputAction _hovering;

    private void Awake()
    {
        _droneControls = new DroneInputActions();
    }


    private void OnEnable()
    {
        _yaw = _droneControls.Drone.Yaw;
        _pitch = _droneControls.Drone.Pitch;
        _roll = _droneControls.Drone.Roll;
        _power = _droneControls.Drone.Power;
        _hovering = _droneControls.Drone.Hovering;
        
        _yaw.Enable();
        _pitch.Enable();
        _roll.Enable();
        _power.Enable();
        _hovering.Enable();
    }

    private void OnDisable()
    {
        _yaw.Disable();
        _pitch.Disable();
        _roll.Disable();
        _power.Disable();
        _hovering.Disable();

    }

    private void Start()
    {
        _rigid = transform.GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            const float size = 10f;
            var randPos = new Vector3(Random.Range(0, size) - 5f, 50, Random.Range(0, size) - 5f);
            Instantiate(ball, randPos + transform.position, ball.transform.rotation);
        }

        
        if (!isHovering)
        {
             OnPitch();
             OnRoll();
             OnPower();
        }
        OnYaw();
        OnHovering();
    }

    private void FixedUpdate()
    {
        PidPos();
        PidRotation();
    }

    private void PidPos()
    {
        var currentPos = transform.position;
        var error = targetPos - currentPos;

        _posIntegral += error * Time.fixedDeltaTime;
        var derivative = (error - _posLastError) / Time.fixedDeltaTime;
        var output = error * posP + _posIntegral * posI + derivative * posD;

        if (!isHovering)
        {
            targetPos = transform.position;
            _rigid.AddForce(new Vector3(0, output.y, 0));
        }
        else
        {
            _rigid.AddForce(output);
            // targetRotationPitch = output.x;
            // targetRotationRoll =  output.z;
        }
        
        _posLastError = error;
    }
    
    

    private void PidRotation()
    {
        var currentRotation = transform.rotation;
        var targetRotation = Quaternion.Euler(new Vector3(targetRotationRoll, targetRotationYaw, targetRotationPitch));
        
        var error = Quaternion.Inverse(currentRotation) * targetRotation;
        var errorEuler = error.eulerAngles;
        
        errorEuler.x = Mathf.DeltaAngle(0, errorEuler.x);
        errorEuler.y = Mathf.DeltaAngle(0, errorEuler.y);
        errorEuler.z = Mathf.DeltaAngle(0, errorEuler.z);
        
        _rotIntegral += errorEuler * Time.fixedDeltaTime;
        
        var pTerm = rotP * errorEuler;
        
        var iTerm = Vector3.ClampMagnitude(rotI * _rotIntegral, maxTorque);
        
        var dTerm = rotD * (errorEuler - _rotLastError) / Time.fixedDeltaTime;

        var controlTorque = Vector3.ClampMagnitude(pTerm + iTerm + dTerm, maxTorque);
        
        _rigid.AddTorque(currentRotation * controlTorque);
        _rotLastError = errorEuler;
    }

    public void OnPower()
    {
        targetPos.y += _power.ReadValue<float>() * Time.deltaTime * moveSpeed;
        //_rigid.AddRelativeForce(Vector3.up);
    }
    
    public void OnYaw()
    {
        //transform.Rotate(new Vector3(0, -_yaw.ReadValue<float>() * Time.deltaTime * rotateSpeed,0));
        targetRotationYaw += _yaw.ReadValue<float>() * Time.deltaTime * -rotateSpeed;
    }

    public void OnPitch()
    {
        targetRotationPitch = _pitch.ReadValue<float>() * -rotateLimit;
        //targetPos.x += _pitch.ReadValue<float>() * Time.deltaTime * moveSpeed;
    }
    
    public void OnRoll()
    {
        targetRotationRoll = _roll.ReadValue<float>() * rotateLimit;
        //targetPos.z += _roll.ReadValue<float>() * Time.deltaTime * moveSpeed;
    }
 
    public void OnHovering()
    {
        isHovering = _hovering.ReadValue<float>() > 0;
    }
}

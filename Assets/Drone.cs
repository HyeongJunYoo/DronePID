using System;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class Drone : MonoBehaviour
{
    public GameObject ball;
    public Transform plane;
    private Rigidbody _rigid;
    public float movePower;
    public float moveSpeed;
    public float rotateSpeed;
    public float rotateLimit;

    public Vector3 targetPos;
    public float posP, posI, posD;
    private Vector3 _posIntegral, _posLastError;
    
    public float rotP, rotI, rotD;
    private Vector3 _rotIntegral, _rotLastError;
    
    [Range(-180f, 180f)]
    public float targetRotationRoll;

    [Range(-90, 90)]
    public float targetRotationYaw;

    [Range(-180f, 180f)]
    public float targetRotationPitch;

    [Range(0f, 1f)]
    public float maxTorque;
    
    private DroneInputActions _droneControls;
    private InputAction _yaw;
    private InputAction _pitch;
    private InputAction _roll;
    private InputAction _power;
    private InputAction _hovering;

    public Propeller cwl;
    public Propeller cwr;
    public Propeller cCwl;
    public Propeller cCwr;

    private Transform _cwlTransform;
    private Transform _cwrTransform;
    private Transform _cCwlTransform;
    private Transform _cCwrTransform;

    private void Awake()
    {
        _droneControls = new DroneInputActions();
    }

    private void Start()
    {
        _rigid = transform.GetComponent<Rigidbody>();
        _cwlTransform = cwl.GetComponent<Transform>();
        _cwrTransform = cwr.GetComponent<Transform>();
        _cCwlTransform = cCwl.GetComponent<Transform>();
        _cCwrTransform = cCwr.GetComponent<Transform>();
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
    
    private void Update()
    {
        InstanceBall();
        OnPower();
        OnPitch();
        OnRoll();
        OnYaw();
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
        movePower = output.y;
        
        cwl.PowerControl(movePower);
        cCwr.PowerControl(movePower);
        cCwl.PowerControl(movePower);
        cwr.PowerControl(movePower);
        
        _rigid.AddForceAtPosition(cwl.Power * _cwlTransform.up, _cwlTransform.position);
        _rigid.AddForceAtPosition(cCwr.Power * _cCwrTransform.up, _cCwrTransform.position);
        _rigid.AddForceAtPosition(cCwl.Power * _cCwlTransform.up, _cCwlTransform.position);
        _rigid.AddForceAtPosition(cwr.Power * _cwrTransform.up, _cwrTransform.position);

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

        //Pitch
        //1번 2번 출력
        cwl.Pitch = -controlTorque.z * rotateSpeed;
        cCwr.Pitch = -controlTorque.z * rotateSpeed;
        
        //3번 4번 출력
        cCwl.Pitch = -controlTorque.z * -rotateSpeed;
        cwr.Pitch = -controlTorque.z * -rotateSpeed;
        
        //Roll
        //2번 4번 출력
        cCwr.Roll = controlTorque.x * -rotateSpeed;
        cwr.Roll = controlTorque.x * -rotateSpeed;
        
        //1번 3번 출력
        cwl.Roll = controlTorque.x * rotateSpeed;
        cCwl.Roll = controlTorque.x * rotateSpeed;
        
        //Yaw
        //2번 3번 출력
        cCwr.Yaw =  controlTorque.y * rotateSpeed;
        cCwl.Yaw =  controlTorque.y * rotateSpeed; 

        //1번 4번 출력
        cwl.Yaw =  controlTorque.y * -rotateSpeed;
        cwr.Yaw =  controlTorque.y * -rotateSpeed;

        //Torque 힘
        var cwlPower = cwl.Power + cwr.Power;
        var cCwlPower = cCwl.Power + cCwr.Power;
        var torquePower = cwlPower - cCwlPower;
        _rigid.AddRelativeTorque(transform.up * -(torquePower * rotateSpeed));
        
        _rotLastError = errorEuler;
    }

    public void OnPower()
    {
        targetPos.y += _power.ReadValue<float>() * moveSpeed * Time.deltaTime;
        targetPos.y = Mathf.Clamp(targetPos.y, plane.position.y, 100);
    }
    
    public void OnYaw()
    { 
        targetRotationYaw += _yaw.ReadValue<float>() * -rotateLimit * 2 * Time.deltaTime;
    }

    public void OnPitch()
    {
        targetRotationPitch = _pitch.ReadValue<float>() * rotateLimit;
    }
    
    public void OnRoll()
    {
        targetRotationRoll = _roll.ReadValue<float>() * rotateLimit;
    }

    private void InstanceBall()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            const float size = 2;
            var randPos = new Vector3(Random.Range(0, size) - size/2,  15, Random.Range(0, size) - size/2);
            Instantiate(ball, randPos + transform.position, ball.transform.rotation);
        }
    }
}

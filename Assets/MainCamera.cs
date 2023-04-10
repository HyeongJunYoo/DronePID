using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    public Transform target;
    private Transform _transform;
    public float smoothMoveSpeed;    
    public float smoothRotateSpeed;     

    private Vector3 _offset;

    private void Start()
    {
        _offset = transform.position - target.position;
        _transform = GetComponent<Transform>();
    }

    private void LateUpdate()
    {
        var desiredPosition = target.position + _offset;
        var smoothedPosition = Vector3.Lerp(_transform.position, desiredPosition, smoothMoveSpeed);
        transform.position = smoothedPosition;
        
        var smoothedRotation = Quaternion.Slerp(_transform.rotation, target.rotation, smoothRotateSpeed);
        if(Quaternion.Angle(_transform.rotation, target.rotation) > 1)
            _transform.rotation = smoothedRotation;
    }
}

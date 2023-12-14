using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : MonoBehaviour
{
    public Joystick _joystick;
    [Header("Speed")]
        [SerializeField] private float _speed;
        [SerializeField] private float _maxVelocityChanged;
    [Header("tilt")] 
        [SerializeField] private float tiltAmount;
    

    private Vector3 _velocityVector = Vector3.zero;
    private Rigidbody _rb;


    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        float _xMovementInput = _joystick.Horizontal;
        float _zMovementInput = _joystick.Vertical;

        Vector3 _movementHor = transform.right * _xMovementInput;
        Vector3 _movementVer = transform.forward * _zMovementInput;

        Vector3 _movementVelocityVector = (_movementVer + _movementHor).normalized * _speed;
        
        Move(_movementVelocityVector);
        
        transform.rotation = Quaternion.Euler(_joystick.Vertical * _speed * tiltAmount, 0, _joystick.Horizontal * -_speed * tiltAmount);
    }

    private void FixedUpdate()
    {
        if (_velocityVector != Vector3.zero)
        {
            Vector3 _velocity = _rb.velocity;
            Vector3 _velocityChanged = (_velocityVector - _velocity);

            _velocityChanged.x = Mathf.Clamp(_velocityChanged.x, -_maxVelocityChanged, _maxVelocityChanged);
            _velocityChanged.y = 0;
            _velocityChanged.z = Mathf.Clamp(_velocityChanged.z, -_maxVelocityChanged, _maxVelocityChanged);
            
            _rb.AddForce(_velocityChanged, ForceMode.Acceleration);
        }
    }

    private void Move(Vector3 movementVelocityVector)
    {
        _velocityVector = movementVelocityVector;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using UnityEngine;

public class SpinController : MonoBehaviour
{
    [SerializeField] private GameObject _playerGraphics;
    
    [Header("Spinner")] 
        [SerializeField] private float _spinSpeed;
        [SerializeField] private bool _doSpin;

        private void FixedUpdate()
        {
            if (_doSpin)
            {
                _playerGraphics.transform.Rotate(new Vector3(0, _spinSpeed * Time.deltaTime, 0));
            }
        }
}

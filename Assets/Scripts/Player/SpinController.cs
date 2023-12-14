using UnityEngine;

public class SpinController : MonoBehaviour
{
    [SerializeField] private GameObject _playerGraphics;
    
    [Header("Spinner")] 
        public float _spinSpeed;
        [SerializeField] private bool _doSpin;

        private void FixedUpdate()
        {
            if (_doSpin)
            {
                _playerGraphics.transform.Rotate(new Vector3(0, _spinSpeed * Time.deltaTime, 0));
            }
        }
}

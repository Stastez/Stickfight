using System;
using System.Security.Cryptography;
using UnityEngine;

namespace Level1
{
    public class Var2EnemyMovement : MonoBehaviour, IObserver<GameManager.GameManagerUpdate>
    {
        private GameObject _enemy;
        private float _wantedPosition;
        private PlayerScript _playerScript;
        private bool _isCurrentlyPaused;

        private void Start()
        {
            _enemy = GameObject.Find("Enemy");
            _wantedPosition = _enemy.transform.position.z;
            _playerScript = GameObject.Find("Player").GetComponent<PlayerScript>();
        }

        //Movement between 1.25f and -3.75f
        void FixedUpdate()
        {
            if (!_playerScript.isIntroDone || _isCurrentlyPaused) return;

            var currentPosition = _enemy.transform.position.z;

            if (IsPositionAboutRight(currentPosition, _wantedPosition, 0.1f))
            {
                _wantedPosition = -RandomNumberGenerator.GetInt32(750, 1175) / 100f;
                return;
            }

            MoveTowards(currentPosition, _wantedPosition, 0.0075f);
        }

        private bool IsPositionAboutRight(float currentPosition, float wantedPosition, float threshold)
        {
            return (Math.Abs(currentPosition - wantedPosition) <= threshold);
        }

        private void MoveTowards(float currentPosition, float wantedPosition, float velocity)
        {
            if (currentPosition > wantedPosition)
            {
                _enemy.transform.position += new Vector3(0, 0, -velocity);
            }
            else
            {
                _enemy.transform.position += new Vector3(0, 0, velocity);
            }
        }
        
        //Observer infrastructure
        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void OnNext(GameManager.GameManagerUpdate value)
        {
            _isCurrentlyPaused = value.isCurrentlyPaused;
        }
    }
}

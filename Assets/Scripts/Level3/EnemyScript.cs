using System;
using UnityEngine;
using DualPantoFramework;

namespace Level3
{
    public class EnemyScript : MonoBehaviour, IObserver<GameManager.GameManagerUpdate>
    {
        private PantoHandle _itHandle, _meHandle;
        private GameObject _enemy, _player;
        private Vector3 _oldPosition;
        private bool _isCurrentlyPaused;
        private float _weaponRot;
        public GameObject weapon;

        public EnemyBehaviourCoordinator.AttackStyle attackStyle;

        // Start is called before the first frame update
        async void Start()
        {
            FindGameObjects();
            _weaponRot = weapon.transform.eulerAngles.y;

            _oldPosition = _enemy.transform.position;

            InvokeRepeating(nameof(RotateWeapon), 0, 0.1f);
        }

        private void FixedUpdate()
        {
            if (_isCurrentlyPaused) return;

            PositionWeapon();
        }

        private void FindGameObjects()
        {
            _meHandle = GameObject.Find("Panto").GetComponent<UpperHandle>();
            _itHandle = GameObject.Find("Panto").GetComponent<LowerHandle>();
            _enemy = transform.gameObject;
            _player = GameObject.Find("Player");
        }

        private void RotateWeapon()
        {
            if (_isCurrentlyPaused) return;

            float weaponDistance = 0.75f;
            var rot = _enemy.transform.eulerAngles;
            
            switch (attackStyle)
            {
                case EnemyBehaviourCoordinator.AttackStyle.OnlyPoint:
                    break;
                case EnemyBehaviourCoordinator.AttackStyle.SpinAround:
                    _weaponRot += 4f;
                    break;
                case EnemyBehaviourCoordinator.AttackStyle.MatchRotation:
                    var handleRotation = _meHandle.GetRotation() - 90;
                    if (Mathf.Abs(_weaponRot - handleRotation) > 181)
                    {
                        if (_weaponRot < 1f)
                            _weaponRot += 360;
                        else
                            handleRotation += 360;
                    }
                    _weaponRot += (handleRotation - _weaponRot) / 20;
                    break;
            }
            
            _weaponRot %= 360f;
            weapon.transform.eulerAngles = new Vector3(0, _weaponRot, 0);
            _enemy.transform.eulerAngles = new Vector3(rot.x, _weaponRot, rot.z);
            weapon.transform.position = _enemy.transform.position - new Vector3(
                Mathf.Sin((_weaponRot + 90) / 360 * 2 * Mathf.PI) *
                weaponDistance, 0,
                Mathf.Cos((_weaponRot + 90) / 360 * 2 * Mathf.PI) *
                weaponDistance);


        }

        private void PositionWeapon()
        {
            var enemyPosition = _enemy.transform.position;

            weapon.transform.position += new Vector3(0, 0, enemyPosition.z - _oldPosition.z);

            _oldPosition = enemyPosition;
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

        public void OnDestroy()
        {
            Destroy(weapon);
            
        }
    }
}
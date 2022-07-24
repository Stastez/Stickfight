using System;
using System.Threading;
using DualPantoFramework;
using SpeechIO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Level1
{
    public class WeaponScript : MonoBehaviour, IObserver<GameManager.GameManagerUpdate>
    {
        private BoxCollider _ownCollider;
        private GameObject _player, _ownGameObject, _enemy, _enemyWeapon;
        private SpeechOut _speech;
        private AudioSource _audioSource;
        private PantoHandle _itHandle;
        private bool _isCurrentlyPaused;
        private bool _blocked = false;

        public AudioClip enemyKilled, enemyBlocked, victory;

        private void Start()
        {
            _ownGameObject = GameObject.Find("PlayerWeapon");
            _ownCollider = _ownGameObject.GetComponent<BoxCollider>();
            _enemy = GameObject.Find("Enemy");
            _speech = new SpeechOut();
            _enemyWeapon = GameObject.Find("EnemyWeapon");
            _audioSource = _ownGameObject.GetComponent<AudioSource>();
            _player = GameObject.Find("Player");
            _itHandle = GameObject.Find("Panto").GetComponent<LowerHandle>();
        }

        private void OnApplicationQuit()
        {
            _speech.Stop();
        }

        private async void OnCollisionEnter(Collision collision)
        {
            GameObject collidedGameObject = collision.collider.gameObject;

            if (_isCurrentlyPaused) return;
            if (collidedGameObject.Equals(_enemy) && !_blocked)
            {
                _audioSource.PlayOneShot(enemyKilled);
                Destroy(_enemy);
                Destroy(_enemyWeapon);
                Thread.Sleep((int)(enemyKilled.length * 1000));
                _audioSource.PlayOneShot(victory, 0.25f);
                await _itHandle.MoveToPosition(new Vector3(0, 0, 0));
                _speech.Speak("Ihr habt Euren Feind gestürzt!", lang: SpeechBase.LANGUAGE.GERMAN);

                SceneManager.LoadScene("Scenes/Level2");
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
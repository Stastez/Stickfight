using System;
using System.Threading;
using DualPantoFramework;
using SpeechIO;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Level3
{
    public class WeaponScript : MonoBehaviour, IObserver<GameManager.GameManagerUpdate>
    {
        private BoxCollider _ownCollider;
        private GameObject _player, _ownGameObject;
        private SpeechOut _speech;
        private AudioSource _audioSource;
        private PantoHandle _itHandle;

        public AudioClip enemyKilled, enemyBlocked, victory;
        private bool _blocked = false;
        private bool _isCurrentlyPaused;

        private void Start()
        {
            _ownGameObject = GameObject.Find("PlayerWeapon");
            _ownCollider = _ownGameObject.GetComponent<BoxCollider>();
            _speech = new SpeechOut();
            _audioSource = _ownGameObject.GetComponent<AudioSource>();
            _player = GameObject.Find("Player");
            _itHandle = GameObject.Find("Panto").GetComponent<LowerHandle>();
        }

        private void OnApplicationQuit()
        {
            _speech.Stop();
        }

        private void OnCollisionExit(Collision other)
        {
            if (other.collider.gameObject.CompareTag("EnemyWeapon"))
            {
                _blocked = false;
            }
        }

        private async void OnCollisionEnter(Collision collision)
        {
            GameObject collidedGameObject = collision.collider.gameObject;

            if (_isCurrentlyPaused) return;

            if (collidedGameObject.CompareTag("EnemyWeapon"))
            {
                _blocked = true;
                _audioSource.PlayOneShot(enemyBlocked);
            }
            else if (collidedGameObject.CompareTag("Enemy") && !_blocked)
            {
                _audioSource.PlayOneShot(enemyKilled);
                var gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
                var newArr = new GameObject[gameManager.enemies.Length-1];
                var count = 0;
                foreach (GameObject enemy in gameManager.enemies)
                {
                    if (enemy != collidedGameObject)
                    {
                        newArr[count] = enemy;
                        count++;
                    }
                }

                gameManager.enemies = newArr;
                Destroy(collidedGameObject);
                if (GameObject.Find("GameManager").GetComponent<GameManager>().enemies.Length == 0)
                {
                    if (SceneManager.GetActiveScene().name == "Level3")
                    {
                        _speech.Speak("Auch der dritte Gegner ist dank dir kaputt!");
                        _audioSource.PlayOneShot(victory);
                        SceneManager.LoadScene("Scenes/Level4");
                    }
                    else
                    {
                        await GameObject.Find("GameManager").GetComponent<GameManager>().WinGame();
                    }
                }
                    
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
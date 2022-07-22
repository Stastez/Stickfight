using System;
using Level2;
using SpeechIO;
using UnityEngine;

namespace Level2
{
    public class EnemyWeaponScript : MonoBehaviour, IObserver<GameManager.GameManagerUpdate>
    {
        private GameObject _player, _enemyWeapon;
        private PlayerScript _playerScript;
        private AudioSource _audio;
        private SpeechOut _speechOut;
        private SpeechIn _speechIn;
        private GameManager _gameManager;
        private bool currentlyInCollision;
        private bool isCurrentlyPaused;

        private void Start()
        {
            _player = GameObject.Find("Player");
            _enemyWeapon = GameObject.Find("EnemyWeapon");
            _playerScript = _player.GetComponent<PlayerScript>();
            _audio = _enemyWeapon.GetComponent<AudioSource>();
            _speechOut = new SpeechOut();
            //_speechIn = new SpeechIn();
            _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        }

        private async void OnCollisionEnter(Collision collision)
        {
            var otherGameobject = collision.transform.gameObject;

            if (otherGameobject != _player || currentlyInCollision || isCurrentlyPaused) return;

            currentlyInCollision = true;
            _audio.Play();
            _gameManager.StopGame();
            await _speechOut.Speak("Oh nein! Ihr wurdet ermeuchelt!", lang: SpeechBase.LANGUAGE.GERMAN);
            _gameManager.RestartGame(false);
        }

        //Observer infrastructure
        public void OnCompleted()
        {
            Debug.Log("[Observer] OnCompleted was called.");
        }

        public void OnError(Exception error)
        {
            Debug.Log("[Observer] OnError was called.");
        }

        public void OnNext(GameManager.GameManagerUpdate value)
        {
            isCurrentlyPaused = value.isCurrentlyPaused;
        }
    }
}

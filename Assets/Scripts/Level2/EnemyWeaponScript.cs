using System;
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
        private bool _currentlyInCollision;
        private bool _isCurrentlyPaused;

        private void Start()
        {
            _player = GameObject.Find("Player");
            _enemyWeapon = GameObject.Find("EnemyWeapon");
            _playerScript = _player.GetComponent<PlayerScript>();
            _audio = _enemyWeapon.GetComponent<AudioSource>();
            _speechOut = new SpeechOut();
            _speechIn = new SpeechIn(SelectGameOverOption);
            _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        }

        private async void OnCollisionEnter(Collision collision)
        {
            var otherGameObject = collision.transform.gameObject;

            if (otherGameObject != _player || _currentlyInCollision || _isCurrentlyPaused) return;

            _currentlyInCollision = true;
            _audio.Play();
            _gameManager.StopGame();
            await _speechOut.Speak("Oh nein! Ihr wurdet ermeuchelt!", lang: SpeechBase.LANGUAGE.GERMAN);

            //SpeechIn apparently only recognizes English
            await _speechOut.Speak("Um das Spiel zu beenden, sagt", lang: SpeechBase.LANGUAGE.GERMAN);
            await _speechOut.Speak("Quit");
            await _speechOut.Speak("Um das Spiel mit Einführung neu zu starten, sagt", lang: SpeechBase.LANGUAGE.GERMAN);
            await _speechOut.Speak("Intro");
            await _speechOut.Speak("um das Spiel ohne Einführung neu zu starten, sagt", lang: SpeechBase.LANGUAGE.GERMAN);
            await _speechOut.Speak("Restart");

            await _speechIn.Listen(new string[] {"Quit", "Intro", "Restart"});
        }

        private void SelectGameOverOption(string command)
        {
            if (command == "Quit")
            {
                Application.Quit();
            } else if (command == "Intro")
            {
                _gameManager.RestartGame(true);
            } else if (command == "Restart")
            {
                _gameManager.RestartGame(false);
            }
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
            _isCurrentlyPaused = value.isCurrentlyPaused;
        }
    }
}

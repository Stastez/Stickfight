using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DualPantoFramework;
using SpeechIO;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Level3
{
    public class GameManager : MonoBehaviour, IObservable<GameManager.GameManagerUpdate>
    {
        public class GameManagerUpdate
        {
            public bool isCurrentlyPaused, isGameOver;

            public GameManagerUpdate(bool isCurrentlyPaused, bool isGameOver)
            {
                this.isCurrentlyPaused = isCurrentlyPaused;
                this.isGameOver = isGameOver;
            }
        }
        
        private List<IObserver<GameManagerUpdate>> _observers;
        private bool _isCurrentlyPaused;
        private bool _isGameOver;

        private void Start()
        {
            _observers = new List<IObserver<GameManagerUpdate>>();
            SubscribeObservers();
        }

        private void SubscribeObservers()
        {
            Subscribe(GameObject.Find("Player").GetComponent<PlayerScript>());
            Subscribe(GameObject.Find("Enemy").GetComponent<EnemyScript>());
            Subscribe(GameObject.Find("EnemyWeapon").GetComponent<EnemyWeaponScript>());
            Subscribe(GameObject.Find("PlayerWeapon").GetComponent<WeaponScript>());
        }

        public void CreateWalls()
        {
            var wallCount = GameObject.Find("Walls").transform.childCount;
            var walls = new GameObject[wallCount];
            var colliders = new PantoBoxCollider[wallCount];

            for (var i = 0; i < wallCount; i++)
            {
                walls[i] = GameObject.Find("Walls").transform.GetChild(i).gameObject;
                colliders[i] = walls[i].GetComponent<PantoBoxCollider>();
                colliders[i].CreateObstacle();
                colliders[i].Enable();
            }
        }
        
        public void PauseGame()
        {
            _isCurrentlyPaused = true;
            UpdatePauseState();
            
            Debug.Log("Game was paused.");
        }
        
        public void ResumeGame()
        {
            _isCurrentlyPaused = false;
            UpdatePauseState();
            
            Debug.Log("Game was resumed.");
        }

        public void StopGame()
        {
            _isCurrentlyPaused = true;
            _isGameOver = true;
            UpdatePauseState();
            
            Debug.Log("GameOver was called.");
        }

        public void RestartGame(bool playIntro)
        {
            PlayerPrefs.SetInt("playIntro", playIntro ? 1 : 0);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        private void OnApplicationQuit()
        {
            PlayerPrefs.DeleteAll();
        }

        public async Task WinGame()
        {
            _isCurrentlyPaused = true;
            UpdatePauseState();
            SpeechOut speech = new SpeechOut();
            speech.SetLanguage(SpeechBase.LANGUAGE.GERMAN);
            await speech.Speak("Du hast alle Widersacher erledigt! Du hast gewonnen!", lang: SpeechBase.LANGUAGE.GERMAN);
        }

        //Observer infrastructure
        public IDisposable Subscribe(IObserver<GameManagerUpdate> observer)
        {
            if (! _observers.Contains(observer))
                _observers.Add(observer);
            return new Unsubscriber(_observers, observer);
        }
        
        private class Unsubscriber : IDisposable
        {
            private List<IObserver<GameManagerUpdate>>_observers;
            private IObserver<GameManagerUpdate> _observer;

            public Unsubscriber(List<IObserver<GameManagerUpdate>> observers, IObserver<GameManagerUpdate> observer)
            {
                this._observers = observers;
                this._observer = observer;
            }

            public void Dispose()
            {
                if (_observer != null && _observers.Contains(_observer))
                    _observers.Remove(_observer);
            }
        }

        private void UpdatePauseState()
        {
            foreach (var observer in _observers)
            {
                observer.OnNext(new GameManagerUpdate(_isCurrentlyPaused, _isGameOver));
            }
        }
    }
}

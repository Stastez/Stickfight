using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DualPantoFramework;
using SpeechIO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Level1
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

        private SpeechIn _speechIn;
        private SpeechOut _speechOut, _speechOutEnglish;
        private List<IObserver<GameManagerUpdate>> _observers;
        private bool _isCurrentlyPaused;
        private bool _isGameOver;
        
        private readonly string[] _commandDictionary = new []{ "pause", "help", "resume", "unpause", "continue", "stop", "restart", "reset", "quit", "exit" };

        private void Start()
        {
            _observers = new List<IObserver<GameManagerUpdate>>();
            SubscribeObservers();
            _speechIn = new SpeechIn(OnCommandRecognized);
            _speechIn.StartListening(_commandDictionary);
            _speechOut = new SpeechOut();
            _speechOut.SetLanguage(SpeechBase.LANGUAGE.GERMAN);
            _speechOutEnglish = new SpeechOut();
            _speechOutEnglish.SetLanguage(SpeechBase.LANGUAGE.ENGLISH);
        }
        
        private void OnApplicationQuit()
        {
            PlayerPrefs.DeleteAll();
            _speechIn.StopListening();
            _speechOut.Stop();
        }

        private void SubscribeObservers()
        {
            Subscribe(GameObject.Find("Player").GetComponent<PlayerScript>());
            Subscribe(GameObject.Find("PlayerWeapon").GetComponent<WeaponScript>());
            Subscribe(GameObject.Find("Enemy").GetComponent<Var2EnemyMovement>());
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
        
        public async Task PauseGame(bool playerControlled = false)
        {
            _isCurrentlyPaused = true;
            UpdatePauseState();

            if (playerControlled) await _speechOut.Speak("Das Spiel ist pausiert.");
            
            Debug.Log("Game was paused.");
        }
        
        public async Task ResumeGame(bool playerControlled = false)
        {
            _isCurrentlyPaused = false;
            UpdatePauseState();
            
            if (playerControlled) await _speechOut.Speak("Das Spiel geht weiter.");
            
            Debug.Log("Game was resumed.");
        }

        public async Task StopGame(bool playerControlled = false)
        {
            _isCurrentlyPaused = true;
            _isGameOver = true;
            UpdatePauseState();
            
            if (playerControlled) await _speechOut.Speak("Das Spiel verloren.");
            
            Debug.Log("GameOver was called.");
        }

        public async Task RestartGame(bool playIntro)
        {
            PlayerPrefs.SetInt("playIntro", playIntro ? 1 : 0);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

            await _speechOut.Speak("Das Spiel wurde zurückgesetzt.");
        }

        public async Task WinGame()
        {
            _isCurrentlyPaused = true;
            UpdatePauseState();

            await new SpeechOut().Speak("Ihr habt alle Eurer Widersacher erledigt! Ihr habt gewonnen!", lang: SpeechBase.LANGUAGE.GERMAN);
        }

        private async void OnCommandRecognized(string command)
        {
            _speechIn.PauseListening();
            
            Debug.Log("[SpeechIO] " + command);
            
            switch (command)
            {
                case "pause":
                    HandlePauseCommand();
                    break;
                case "help":
                    HandleHelpCommand();
                    break;
                case "resume" or "unpause" or "continue":
                    HandleResumeCommand();
                    break;
                case "stop" or "quit" or "exit":
                    HandleExitCommand();
                    break;
                case "restart" or "reset":
                    HandleRestartCommand();
                    break;
            }
            
            _speechIn.StartListening(_commandDictionary);
        }

        private async void HandlePauseCommand()
        {
            if (_isCurrentlyPaused)
            {
                await ResumeGame(true);
            }
            else
            {
                await PauseGame(true);
                await _speechOut.Speak("Sag");
                await _speechOutEnglish.Speak("HELP");
                await _speechOut.Speak("für Hilfe,");
                await _speechOutEnglish.Speak("RESUME");
                await _speechOut.Speak("um das Spiel fortzusetzen,");
                await _speechOutEnglish.Speak("RESTART");
                await _speechOut.Speak("um das Spiel neuzustarten oder");
                await _speechOutEnglish.Speak("QUIT");
                await _speechOut.Speak("um das Spiel zu beenden.");
            }
        }

        private async void HandleHelpCommand()
        {
            
        }

        private async void HandleResumeCommand()
        {
            if (!_isCurrentlyPaused) return;

            await ResumeGame(true);
        }

        private async void HandleExitCommand()
        {
            Application.Quit();
        }

        private async void HandleRestartCommand()
        {
            await RestartGame(true);
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

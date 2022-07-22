using System;
using System.Collections.Generic;
using DualPantoFramework;
using UnityEngine;

namespace Level2
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
        public bool isCurrentlyPaused;
        public bool isGameOver;

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
        
        public void PauseGame()
        {
            isCurrentlyPaused = true;
            UpdatePauseState();
            
            Debug.Log("Game was paused.");
        }
        
        public void ResumeGame()
        {
            isCurrentlyPaused = false;
            UpdatePauseState();
            
            Debug.Log("Game was resumed.");
        }

        public void StopGame()
        {
            
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
                observer.OnNext(new GameManagerUpdate(isCurrentlyPaused, isGameOver));
            }
        }
    }
}

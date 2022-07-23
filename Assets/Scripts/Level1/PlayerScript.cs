using System;
using UnityEngine;
using DualPantoFramework;
using UnityEngine.SceneManagement;

namespace Level1
{
    public class PlayerScript : MonoBehaviour, IObserver<GameManager.GameManagerUpdate>
    {
        private PantoHandle _meHandle;
        private GameObject _player, _weapon;
        private GameManager _gameManager;

        public bool playIntro;
        public bool isIntroDone;
        private bool _isCurrentlyPaused;

        // Start is called before the first frame update
        async void Start()
        {
            if (PlayerPrefs.HasKey("playIntro"))
            {
                playIntro = (PlayerPrefs.GetInt("playIntro") == 1);
            }
            
            await IntroductionHandler.Introduce(playIntro);

            _meHandle = GameObject.Find("Panto").GetComponent<UpperHandle>();
            _player = GameObject.Find("Player");
            _weapon = GameObject.Find("PlayerWeapon");
            _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

            isIntroDone = true;

            _gameManager.CreateWalls();
        }

        private void FixedUpdate()
        {
            if (!isIntroDone || _isCurrentlyPaused) return;

            var currentHandlePosition = _meHandle.HandlePosition(transform.position);

            currentHandlePosition.y = (SceneManager.GetActiveScene().name == "Level1") ? 0 : 1;

            _meHandle.Rotate(90);
            transform.position = currentHandlePosition;
            PositionWeapon();
        }

        private void Awake()
        {
            Application.targetFrameRate = 62;
        }
        
        private void PositionWeapon()
        {
            const float horizontalWeaponDistance = 0.75f;
            const float weaponHeight = 0f;

            _weapon.transform.position = _player.transform.position + new Vector3(horizontalWeaponDistance, weaponHeight, 0);
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

            if (_isCurrentlyPaused)
            {
                _meHandle.Freeze();
            }
            else
            {
                _meHandle.Free();
            }
        }
    }
}

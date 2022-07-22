using System;
using UnityEngine;
using DualPantoFramework;
using UnityEngine.SceneManagement;

namespace Level2
{
    public class PlayerScript : MonoBehaviour
    {
        public enum WeaponPosition
        {
            Up,
            Middle,
            Down
        }

        public enum WeaponSide
        {
            Left,
            Right
        }

        private PantoHandle _meHandle;
        private GameObject _player, _weapon;
        private IntroductionHandler _intro;

        public bool playIntro;
        public bool isIntroDone;
        public WeaponPosition weaponPosition;
        public WeaponSide weaponSide;

        // Start is called before the first frame update
        async void Start()
        {
            _meHandle = GameObject.Find("Panto").GetComponent<UpperHandle>();
            _player = GameObject.Find("Player");
            _weapon = GameObject.Find("PlayerWeapon");
            _intro = _player.GetComponent<IntroductionHandler>();

            await _intro.Introduce(playIntro);

            isIntroDone = true;

            InitializeWorld.CreateWalls();
        }

        private void FixedUpdate()
        {
            if (!isIntroDone) return;

            var currentHandlePosition = _meHandle.HandlePosition(transform.position);

            currentHandlePosition.y = (SceneManager.GetActiveScene().name == "Level1") ? 0 : 1;

            transform.position = currentHandlePosition;
            PositionWeapon();
        }

        private void Awake()
        {
            Application.targetFrameRate = 62;
        }

        private void PositionWeapon()
        {
            const float weaponDistance = 0.75f;
            var handleRotation = _meHandle.GetRotation();

            _weapon.transform.eulerAngles = new Vector3(0, handleRotation - 90, 0);
            _weapon.transform.position = _player.transform.position + new Vector3(
                Mathf.Sin(handleRotation / 360 * 2 * Mathf.PI) * weaponDistance, 0,
                Mathf.Cos(handleRotation / 360 * 2 * Mathf.PI) * weaponDistance);
        }
    }
}
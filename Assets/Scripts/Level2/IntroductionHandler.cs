using System;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using DualPantoFramework;
using SpeechIO;
using Unity.VisualScripting;
using UnityEngine;

namespace Level2
{
    public class IntroductionHandler : MonoBehaviour
    {
        public enum WiggleDirection
        {
            Up,
            Down,
            UpDown,
            Left,
            Right,
            LeftRight
        }

        private GameObject _panto, _player, _enemy;
        private Level _level;
        private PantoHandle _meHandle, _itHandle;
        private SpeechOut _speech;

        private bool _initialized;

        private void Start()
        {
            _panto = GameObject.Find("Panto");
            _player = GameObject.Find("Player");
            _enemy = GameObject.Find("Enemy");
            _level = _panto.GetComponent<Level>();
            _meHandle = _panto.GetComponent<UpperHandle>();
            _itHandle = _panto.GetComponent<LowerHandle>();
            _speech = new SpeechOut();

            _initialized = true;
        }

        private void OnApplicationQuit()
        {
            _speech.Stop();
        }

        public async Task Introduce(bool playIntro)
        {
            if (!_initialized) Start();

            if (playIntro) await PlayIntro();
            await _itHandle.SwitchTo(_enemy, 50f);

            await _meHandle.MoveToPosition(_player.transform.position);
            _meHandle.FreeRotation();
        }

        private async Task PlayIntro()
        {
            await _meHandle.MoveToPosition(_player.transform.position);

            _player.GetComponent<PlayerScript>().isIntroDone = true;

            _speech.Speak("You can now rotate your weapon freely in all directions:");

            _speech.Speak("But be careful! Your enemy will try to block your attempts");
            StartCoroutine(nameof(Rotate), _meHandle);

            Thread.Sleep(6000);
        }

        /**
         * Max movement speed is 1.5f
         */
        public async Task Wiggle(PantoHandle handle, GameObject reference, WiggleDirection direction, float intensity,
            float extent)
        {
            Vector3 originalPosition = handle.HandlePosition(reference.transform.position);

            async Task MoveHandle(Vector3 direction, int repetitions)
            {
                for (int i = 0; i < repetitions; i++)
                {
                    await handle.MoveToPosition(originalPosition + direction * extent, intensity);
                    await handle.MoveToPosition(originalPosition, handle.MaxMovementSpeed());
                }
            }

            switch (direction)
            {
                case WiggleDirection.Up:
                    await MoveHandle(new Vector3(0, 0, 1), 2);
                    break;
                case WiggleDirection.Down:
                    await MoveHandle(new Vector3(0, 0, -1), 2);
                    break;
                case WiggleDirection.Left:
                    await MoveHandle(new Vector3(-1, 0, 0), 2);
                    break;
                case WiggleDirection.UpDown:
                    await MoveHandle(new Vector3(0, 0, 1), 1);
                    await MoveHandle(new Vector3(0, 0, -1), 1);
                    break;
                case WiggleDirection.Right:
                    await MoveHandle(new Vector3(1, 0, 0), 2);
                    break;
                case WiggleDirection.LeftRight:
                    await MoveHandle(new Vector3(-1, 0, 0), 1);
                    await MoveHandle(new Vector3(1, 0, 0), 1);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
            }
        }

        private IEnumerator Rotate(PantoHandle handle)
        {
            for (float rotation = 0; rotation < 360; rotation += 2)
            {
                if (handle.isFrozen)
                    handle.Free();

                handle.Rotate(rotation);
                yield return new WaitForSeconds(0.003f);
                handle.FreeRotation();
            }
        }
    }
}
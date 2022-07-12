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
        }

        private async Task PlayIntro()
        {
            await _meHandle.MoveToPosition(_player.transform.position);

            _player.GetComponent<PlayerScript>().isIntroDone = true;

            _speech.Speak("You can now rotate your weapon freely in all directions:");
            StartCoroutine(nameof(Rotate), _meHandle);

            _speech.Speak("But be careful! Your enemy will try to block your attempts");
        }

        private IEnumerator Rotate(PantoHandle handle)
        {
            for (float rotation = 0; rotation < 360; rotation++)
            {
                if (handle.isFrozen)
                    handle.Free();

                handle.Rotate(rotation);
                yield return new WaitForSeconds(0.003f);
            }

            handle.FreeRotation();
        }
    }
}
using System;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using DualPantoFramework;
using SpeechIO;
using UnityEngine;

namespace Level3
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
        private PantoHandle _meHandle, _itHandle;
        private SpeechOut _speech;

        private bool _initialized;

        private void Start()
        {
            _panto = GameObject.Find("Panto");
            _player = GameObject.Find("Player");
            _enemy = GameObject.Find("Enemy");
            _meHandle = _panto.GetComponent<UpperHandle>();
            _itHandle = _panto.GetComponent<LowerHandle>();
            _speech = new SpeechOut();
            _speech.SetLanguage(SpeechBase.LANGUAGE.GERMAN);


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

            _meHandle.Freeze();

            StartCoroutine(nameof(Rotate), _meHandle);
            await _speech.Speak("Du kannst deine Waffe komplett um dich herum drehen:");

            _speech.Speak("Aber Vorsicht! Deine Widersacher werden versuchen, die Schläge zu parieren!");
            await _itHandle.SwitchTo(_enemy, 10f);

            await _speech.Speak("Versuch doch ihn von der Seite zu treffen!");

            _meHandle.Free();
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
using System;
using System.Threading.Tasks;
using DualPantoFramework;
using SpeechIO;
using UnityEngine;

namespace Level1
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

        private static GameObject _panto = GameObject.Find("Panto"),
            _player = GameObject.Find("Player"),
            _enemy = GameObject.Find("Enemy");

        private static Level _level = _panto.GetComponent<Level>();

        private static PantoHandle _meHandle = _panto.GetComponent<UpperHandle>(), _itHandle = _panto.GetComponent<LowerHandle>();

        private static SpeechOut _speech = new();

        public static async Task Introduce(bool playIntro)
        {
            if (playIntro) await PlayIntro();

            await _meHandle.MoveToPosition(_player.transform.position);
            await _itHandle.SwitchTo(_enemy, 20f);

            //TODO: Freeze rotation of end effector
        }

        private static async Task PlayIntro()
        {
            await _meHandle.MoveToPosition(_player.transform.position);

            _speech.Speak("Ihr habt ein Schwert!", lang: SpeechBase.LANGUAGE.GERMAN);
            await Wiggle(_meHandle, _player, WiggleDirection.LeftRight, 0.25f, 1f);

            await _itHandle.MoveToPosition(_enemy.transform.position);
            
            _speech.Speak("Stecht den da ab!", lang: SpeechBase.LANGUAGE.GERMAN);
            await Wiggle(_itHandle, _enemy, WiggleDirection.UpDown, 0.25f, 1f);

            await Wiggle(_meHandle, _player, WiggleDirection.Right, 0.25f, 1);
        }

        /**
     * Max movement speed is 1.5f
     */
        public static async Task Wiggle(PantoHandle handle, GameObject reference, WiggleDirection direction,
            float intensity, float extent)
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

        private void OnApplicationQuit()
        {
            _speech.Stop();
        }
    }
}

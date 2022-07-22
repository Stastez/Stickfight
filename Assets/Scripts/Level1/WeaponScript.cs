using System.Threading;
using DualPantoFramework;
using SpeechIO;
using UnityEngine;

namespace Level1
{
    public class WeaponScript : MonoBehaviour
    {
        private BoxCollider _ownCollider;
        private GameObject _player, _ownGameObject, _enemy, _enemyWeapon;
        private SpeechOut _speech;
        private AudioSource _audioSource;
        private PantoHandle _itHandle;

        public AudioClip enemyKilled, enemyBlocked, victory;

        private void Start()
        {
            _ownGameObject = GameObject.Find("PlayerWeapon");
            _ownCollider = _ownGameObject.GetComponent<BoxCollider>();
            _enemy = GameObject.Find("Enemy");
            _speech = new SpeechOut();
            _enemyWeapon = GameObject.Find("EnemyWeapon");
            _audioSource = _ownGameObject.GetComponent<AudioSource>();
            _player = GameObject.Find("Player");
            _itHandle = GameObject.Find("Panto").GetComponent<LowerHandle>();
        }

        private void OnApplicationQuit()
        {
            _speech.Stop();
        }

        private async void OnCollisionEnter(Collision collision)
        {
            GameObject collidedGameObject = collision.collider.gameObject;

            if (collidedGameObject.Equals(_enemy))
            {
                if (WasHitBlocked())
                {
                    _audioSource.PlayOneShot(enemyBlocked);
                    return;
                }

                _audioSource.PlayOneShot(enemyKilled);
                Destroy(_enemy);
                Destroy(_enemyWeapon);
                Thread.Sleep((int) (enemyKilled.length * 1000));
                _audioSource.PlayOneShot(victory, 0.25f);
                _speech.Speak("Ihr habt Euren Feind gestürzt!");
                await _itHandle.MoveToPosition(new Vector3(0, 0, 0));
            }
        }

        private bool WasHitBlocked()
        {
            PlayerScript _playerScript = _player.GetComponent<PlayerScript>();
            EnemyScript _enemyScript = _enemy.GetComponent<EnemyScript>();

            return (_enemyScript.weaponPosition == _playerScript.weaponPosition) &&
                   (_enemyScript.weaponSide != _playerScript.weaponSide);
        }
    }
}

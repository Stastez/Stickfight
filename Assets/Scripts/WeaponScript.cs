using System;
using System.Threading;
using SpeechIO;
using UnityEngine;

public class WeaponScript : MonoBehaviour
{
    private BoxCollider _ownCollider;
    private GameObject _player, _ownGameObject, _enemy, _enemyWeapon;
    private SpeechOut _speech;
    private AudioSource _audioSource;
    public AudioClip enemyKilled, enemyBlocked;

    private void Start()
    {
        _ownGameObject = GameObject.Find("PlayerWeapon");
        _ownCollider = _ownGameObject.GetComponent<BoxCollider>();
        _enemy = GameObject.Find("Enemy");
        _speech = new SpeechOut();
        _enemyWeapon = GameObject.Find("EnemyWeapon");
        _audioSource = _ownGameObject.GetComponent<AudioSource>();
        _player = GameObject.Find("Player");
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
            GameObject.Destroy(_enemy);
            GameObject.Destroy(_enemyWeapon);
            Thread.Sleep((int) (enemyKilled.length * 1000));
            await _speech.Speak("You have slain your enemy!");
        }
    }

    private bool WasHitBlocked()
    {
        PlayerScript _playerScript = _player.GetComponent<PlayerScript>();
        EnemyScript _enemyScript = _enemy.GetComponent<EnemyScript>();
        
        return (_enemyScript.weaponPosition == _playerScript.weaponPosition) && (_enemyScript.weaponSide != _playerScript.weaponSide);
    }
}

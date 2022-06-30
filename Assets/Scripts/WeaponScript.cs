using System;
using SpeechIO;
using UnityEngine;

public class WeaponScript : MonoBehaviour
{
    private BoxCollider _collider;
    private GameObject _enemy;
    private SpeechOut _speech;

    private void Start()
    {
        _collider = GameObject.Find("PlayerWeapon").GetComponent<BoxCollider>();
        _enemy = GameObject.Find("Enemy");
        _speech = new SpeechOut();
    }

    private async void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.gameObject.Equals(_enemy))
        {
            GameObject.Destroy(_enemy);
            await _speech.Speak("You have slain your enemy!");
        }
    }
}

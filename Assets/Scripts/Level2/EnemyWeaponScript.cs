using System;
using System.Collections;
using System.Collections.Generic;
using SpeechIO;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyWeaponScript : MonoBehaviour
{
    private GameObject _player, _enemyWeapon;
    private AudioSource _audio;
    private SpeechOut _speechOut;
    private SpeechIn _speechIn;
    private bool currentlyInCollision;

    private void Start()
    {
        _player = GameObject.Find("Player");
        _enemyWeapon = GameObject.Find("EnemyWeapon");
    }

    private void OnCollisionEnter(Collision collision)
    {
        var otherGameobject = collision.transform.gameObject;

        if (otherGameobject != _player && !currentlyInCollision) return;

        currentlyInCollision = true;
        _audio.Play();
        _speechOut.Speak("Oh nein! Ihr wurdet ermeuchelt!", lang: SpeechBase.LANGUAGE.GERMAN);
    }
}

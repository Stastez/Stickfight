using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DualPantoFramework;

public class PlayerScript : MonoBehaviour
{

    private PantoHandle _meHandle;
    private GameObject _player;
    
    // Start is called before the first frame update
    async void Start()
    {
        _meHandle = GameObject.Find("Panto").GetComponent<UpperHandle>();
        _player = GameObject.Find("Player");

        await _meHandle.SwitchTo(_player);
        _meHandle.Free();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        transform.position = _meHandle.HandlePosition(transform.position);
        
    }

    private void PositionWeapon()
    {
        
    }
}

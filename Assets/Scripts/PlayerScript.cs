using UnityEngine;
using DualPantoFramework;

public class PlayerScript : MonoBehaviour
{

    private PantoHandle _meHandle;
    private GameObject _player;
    private GameObject _weapon;

    private bool _isSwitchDone;
    
    // Start is called before the first frame update
    async void Start()
    {
        _meHandle = GameObject.Find("Panto").GetComponent<UpperHandle>();
        _player = GameObject.Find("Player");
        _weapon = GameObject.Find("PlayerWeapon");

        await _meHandle.SwitchTo(_player);
        _meHandle.Free();

        InitializeWorld.CreateWalls();
        
        _isSwitchDone = true;
    }

    private void FixedUpdate()
    {
        if (!_isSwitchDone) return;

        transform.position = _meHandle.HandlePosition(transform.position);
        PositionWeapon();
    }
    
    private void PositionWeapon()
    {
        const float horizontalWeaponDistance = 0.75f;
        const float verticalWeaponDistance = 0.33f;
        var handleRotation = _meHandle.GetRotation();

        const float weaponRotation = 25f;
        
        var up = new Vector3(0, -weaponRotation, 0);
        var middle = new Vector3(0, 0, 0);
        var down = new Vector3(0, weaponRotation, 0);
        
        switch (handleRotation)
        {
            case >= 0 and < 60:
                _weapon.transform.eulerAngles = up;
                _weapon.transform.position = _player.transform.position + new Vector3(horizontalWeaponDistance, 0.25f, verticalWeaponDistance);
                break;
            case >= 60 and < 120:
                _weapon.transform.eulerAngles = middle;
                _weapon.transform.position = _player.transform.position + new Vector3(horizontalWeaponDistance, 0.25f, 0);
                break;
            case >= 120 and < 180:
                _weapon.transform.eulerAngles = down;
                _weapon.transform.position = _player.transform.position + new Vector3(horizontalWeaponDistance, 0.25f, -verticalWeaponDistance);
                break;
            case >= 180 and < 240:
                _weapon.transform.eulerAngles = -down;
                _weapon.transform.position = _player.transform.position + new Vector3(-horizontalWeaponDistance, 0.25f, -verticalWeaponDistance);
                break;
            case >= 240 and < 300:
                _weapon.transform.eulerAngles = middle;
                _weapon.transform.position = _player.transform.position + new Vector3(-horizontalWeaponDistance, 0.25f, 0);
                break;
            case >= 300 and < 360:
                _weapon.transform.eulerAngles = -up;
                _weapon.transform.position = _player.transform.position + new Vector3(-horizontalWeaponDistance, 0.25f, verticalWeaponDistance);
                break;
        }
    }
}

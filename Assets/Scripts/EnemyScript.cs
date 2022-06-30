using UnityEngine;
using DualPantoFramework;

public class EnemyScript : MonoBehaviour
{
    private PantoHandle _itHandle, _meHandle;
    private GameObject _enemy, _player, _weapon;
    private Vector3 _oldPosition;
    private float _oldRotation;

    public PlayerScript.WeaponPosition weaponPosition;
    public PlayerScript.WeaponSide weaponSide;

    // Start is called before the first frame update
    async void Start()
    {
        FindGameObjects();

        _itHandle.Free();
        await _itHandle.SwitchTo(_enemy);
        
        InvokeRepeating(nameof(PositionWeapon), 0, 2);
    }

    private void FindGameObjects()
    {
        _meHandle = GameObject.Find("Panto").GetComponent<UpperHandle>();
        _itHandle = GameObject.Find("Panto").GetComponent<LowerHandle>();
        _enemy = GameObject.Find("Enemy");
        _player = GameObject.Find("Player");
        _weapon = GameObject.Find("EnemyWeapon");
    }

    private void PositionWeapon()
    {
        var playerIsLeftOfEnemy = _player.transform.position.x <= _enemy.transform.position.x;
        
        float horizontalWeaponDistance = (playerIsLeftOfEnemy) ? -0.75f : 0.75f ;
        float verticalWeaponDistance = 0.33f;
        const float weaponHeight = 0f;
        var handleRotation = _meHandle.GetRotation();
        float weaponRotation = (playerIsLeftOfEnemy) ? -25f : 25f;
        
        var up = new Vector3(0, -weaponRotation, 0);
        var middle = new Vector3(0, 0, 0);
        var down = new Vector3(0, weaponRotation, 0);

        weaponSide = playerIsLeftOfEnemy ? PlayerScript.WeaponSide.Left : PlayerScript.WeaponSide.Right;

        switch (handleRotation)
        {
            case (>= 0 and < 60) or (>= 300 and < 360):
                _weapon.transform.eulerAngles = up;
                _weapon.transform.position = _enemy.transform.position + new Vector3(horizontalWeaponDistance, weaponHeight, verticalWeaponDistance);

                weaponPosition = PlayerScript.WeaponPosition.Up;
                break;
            case (>= 60 and < 120) or (>= 240 and < 300):
                _weapon.transform.eulerAngles = middle;
                _weapon.transform.position = _enemy.transform.position + new Vector3(horizontalWeaponDistance, weaponHeight, 0);
                
                weaponPosition = PlayerScript.WeaponPosition.Middle;
                break;
            case (>= 120 and < 180) or (>= 180 and < 240):
                _weapon.transform.eulerAngles = down;
                _weapon.transform.position = _enemy.transform.position + new Vector3(horizontalWeaponDistance, weaponHeight, -verticalWeaponDistance);
                
                weaponPosition = PlayerScript.WeaponPosition.Down;
                break;
        }
    }
}
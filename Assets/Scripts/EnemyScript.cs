using UnityEngine;
using DualPantoFramework;

public class EnemyScript : MonoBehaviour
{
    private PantoHandle _itHandle, _meHandle;
    private GameObject _enemy, _player;
    private GameObject _weapon;

    // Start is called before the first frame update
    async void Start()
    {
        _meHandle = GameObject.Find("Panto").GetComponent<UpperHandle>();
        _itHandle = GameObject.Find("Panto").GetComponent<LowerHandle>();
        _enemy = GameObject.Find("Enemy");
        _player = GameObject.Find("Player");
        _weapon = GameObject.Find("EnemyWeapon");

        _itHandle.Free();
        await _itHandle.SwitchTo(_enemy);
    }

    private void FixedUpdate()
    {
        PositionWeapon();
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

        switch (handleRotation)
        {
            case (>= 0 and < 60) or (>= 300 and < 360):
                _weapon.transform.eulerAngles = up;
                _weapon.transform.position = _enemy.transform.position + new Vector3(horizontalWeaponDistance, weaponHeight, verticalWeaponDistance);
                break;
            case (>= 60 and < 120) or (>= 240 and < 300):
                _weapon.transform.eulerAngles = middle;
                _weapon.transform.position = _enemy.transform.position + new Vector3(horizontalWeaponDistance, weaponHeight, 0);
                break;
            case (>= 120 and < 180) or (>= 180 and < 240):
                _weapon.transform.eulerAngles = down;
                _weapon.transform.position = _enemy.transform.position + new Vector3(horizontalWeaponDistance, weaponHeight, -verticalWeaponDistance);
                break;
        }
    }
}
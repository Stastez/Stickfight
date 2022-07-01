using UnityEngine;
using DualPantoFramework;
using UnityEditor.SearchService;
using UnityEngine.SceneManagement;
using Scene = UnityEngine.SceneManagement.Scene;

public class PlayerScript : MonoBehaviour
{
    public enum WeaponPosition
    {
        Up,
        Middle,
        Down
    }

    public enum WeaponSide
    {
        Left,
        Right
    }
    
    private PantoHandle _meHandle;
    private GameObject _player, _weapon;

    public bool playIntro;
    public bool isIntroDone;
    public WeaponPosition weaponPosition;
    public WeaponSide weaponSide;

    // Start is called before the first frame update
    async void Start()
    {
        await IntroductionHandler.Introduce(playIntro);
        
        _meHandle = GameObject.Find("Panto").GetComponent<UpperHandle>();
        _player = GameObject.Find("Player");
        _weapon = GameObject.Find("PlayerWeapon");

        isIntroDone = true;
        
        InitializeWorld.CreateWalls();
    }

    private void FixedUpdate()
    {
        if (!isIntroDone) return;
        
        var currentHandlePosition = _meHandle.HandlePosition(transform.position);

        currentHandlePosition.y = (SceneManager.GetActiveScene().name == "Level1") ? 0 : 1;
        
        transform.position = currentHandlePosition;
        PositionWeapon();
    }
    
    private void PositionWeapon()
    {
        const float horizontalWeaponDistance = 0.75f;
        const float verticalWeaponDistance = 0.33f;
        const float weaponHeight = 0f;
        var handleRotation = _meHandle.GetRotation();

        const float weaponRotation = 25f;
        
        var up = new Vector3(0, -weaponRotation, 0);
        var middle = new Vector3(0, 0, 0);
        var down = new Vector3(0, weaponRotation, 0);
        
        switch (handleRotation)
        {
            case >= 0 and < 60:
                _weapon.transform.eulerAngles = up;
                _weapon.transform.position = _player.transform.position + new Vector3(horizontalWeaponDistance, weaponHeight, verticalWeaponDistance);
                
                weaponPosition = WeaponPosition.Up;
                weaponSide = WeaponSide.Right;
                break;
            case >= 60 and < 120:
                _weapon.transform.eulerAngles = middle;
                _weapon.transform.position = _player.transform.position + new Vector3(horizontalWeaponDistance, weaponHeight, 0);
                
                weaponPosition = WeaponPosition.Middle;
                weaponSide = WeaponSide.Right;
                break;
            case >= 120 and < 180:
                _weapon.transform.eulerAngles = down;
                _weapon.transform.position = _player.transform.position + new Vector3(horizontalWeaponDistance, weaponHeight, -verticalWeaponDistance);
                
                weaponPosition = WeaponPosition.Down;
                weaponSide = WeaponSide.Right;
                break;
            case >= 180 and < 240:
                _weapon.transform.eulerAngles = -down;
                _weapon.transform.position = _player.transform.position + new Vector3(-horizontalWeaponDistance, weaponHeight, -verticalWeaponDistance);
                
                weaponPosition = WeaponPosition.Down;
                weaponSide = WeaponSide.Left;
                break;
            case >= 240 and < 300:
                _weapon.transform.eulerAngles = middle;
                _weapon.transform.position = _player.transform.position + new Vector3(-horizontalWeaponDistance, weaponHeight, 0);
                
                weaponPosition = WeaponPosition.Middle;
                weaponSide = WeaponSide.Left;
                break;
            case >= 300 and < 360:
                _weapon.transform.eulerAngles = -up;
                _weapon.transform.position = _player.transform.position + new Vector3(-horizontalWeaponDistance, weaponHeight, verticalWeaponDistance);
                
                weaponPosition = WeaponPosition.Up;
                weaponSide = WeaponSide.Left;
                break;
        }
    }
}

using DualPantoFramework;
using UnityEngine;

public class InitializeWorld : MonoBehaviour
{

    private static int wallCount = GameObject.Find("Walls").transform.childCount;
    private static GameObject[] _walls = new GameObject[wallCount];
    private static PantoBoxCollider[] _colliders = new PantoBoxCollider[wallCount];
    
    public static void CreateWalls()
    {
        for (var i = 0; i < wallCount; i++)
        {
            _walls[i] = GameObject.Find("Walls").transform.GetChild(i).gameObject;
            _colliders[i] = _walls[i].GetComponent<PantoBoxCollider>();
            _colliders[i].CreateObstacle();
            _colliders[i].Enable();
        }
    }
}

using DualPantoFramework;
using UnityEngine;

namespace Level2
{
    public class InitializeWorld : MonoBehaviour
    {
        public static void CreateWalls()
        {
            var wallCount = GameObject.Find("Walls").transform.childCount;
            var walls = new GameObject[wallCount];
            var colliders = new PantoBoxCollider[wallCount];

            for (var i = 0; i < wallCount; i++)
            {
                walls[i] = GameObject.Find("Walls").transform.GetChild(i).gameObject;
                colliders[i] = walls[i].GetComponent<PantoBoxCollider>();
                colliders[i].CreateObstacle();
                colliders[i].Enable();
            }
        }
    }
}

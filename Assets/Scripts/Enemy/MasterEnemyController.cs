using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class MasterEnemyController : MonoBehaviour {

    [SerializeField]
    private List<EnemyUnityController> enemy_Units;
    EnemyUnityController enemgfdagfadgafd;

    [SerializeField]
    private float CallDistance = 20;

    void Start()
    {
        enemgfdagfadgafd = GetComponent<EnemyUnityController>();
        enemy_Units = new List<EnemyUnityController>(ExtraMethods.ReturnAllComponetsTypeInGameobjectsArray<EnemyUnityController>(GameObject.FindGameObjectsWithTag("Guard")));
    }
    public void AddEnemyController(EnemyUnityController enemy_Unit)
    {
        this.enemy_Units.Add(enemy_Unit);
        
    }
    public void CallNearEnemys(EnemyUnityController ThisController, Collider PlayerCollider)
    {
        EnemyUnityController[] arr = ExtraMethods.CheckWithinRangeOf<EnemyUnityController>(enemy_Units.ToArray(),
                                                          ThisController, CallDistance);

        for (int i = 0; i < arr.Length; i++)
        {
            print(arr[i].name);
        }
        for (int i = 0; i < arr.Length; i++)
        {
            if (!arr[i].PlayerDetected)
            {
                arr[i].CallGuard(PlayerCollider);
            }
        }

    }
}

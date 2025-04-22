using UnityEngine;
using System.Collections.Generic;
using System.Linq;


public class PoolManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public Enemy TargetEnemyPrefab;
    public List<Enemy> EnemyPoolList = new List<Enemy>();


    public Enemy GetEnemy()
    {
        Enemy targetEnemy = EnemyPoolList.FirstOrDefault(enemy => enemy.gameObject.activeSelf == false);

        // 위 코드와 아래 주석으로 된 영역은 같은 동작을 함
        //Enemy targetEnemy = null;
        //foreach(Enemy _enemy in EnemyPoolList)
        //{
        //    if(_enemy.gameObject.activeSelf == false)
        //    {
        //        targetEnemy = _enemy;
        //    }
        //}

        if (targetEnemy != null)
        {
            return targetEnemy;
        }

        // 이하 아래에 있는 것들은 targetEnemy == null 일 때
        Enemy newEnemy = Instantiate(TargetEnemyPrefab);
        EnemyPoolList.Add(newEnemy);

        return newEnemy;
    }

}

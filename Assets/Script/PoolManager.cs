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

        // �� �ڵ�� �Ʒ� �ּ����� �� ������ ���� ������ ��
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

        // ���� �Ʒ��� �ִ� �͵��� targetEnemy == null �� ��
        Enemy newEnemy = Instantiate(TargetEnemyPrefab);
        EnemyPoolList.Add(newEnemy);

        return newEnemy;
    }

}

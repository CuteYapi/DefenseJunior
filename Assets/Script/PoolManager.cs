using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class PoolManager : MonoBehaviour
{
    #region Enemy Pool

    public Enemy RefEnemy;
    public List<Enemy> EnemyPool = new List<Enemy>();

    public Enemy GetEnemy()
    {
        // 1. 풀에 사용 가능한 오브젝트가 있는지 찾기
        Enemy targetEnemy = EnemyPool.FirstOrDefault(enemy => enemy.gameObject.activeSelf == false);

        #region 1.에 대한 다른 방법
        // 1.에 대한 다른 방법
        //Enemy targetEnemy = null; // (=default)
        //foreach(Enemy item in EnemyPool)
        //{
        //    if(item.gameObject.activeSelf == false)
        //    {
        //        targetEnemy = item;
        //        break;
        //    }
        //}
        #endregion

        // 2-1. 사용 가능한 Enemy가 없을 경우
        if (targetEnemy == default) //(== null)
        {
            // 새로운 걸 만들어서 사용 가능하게 하기
            Enemy newEnemy = Instantiate(RefEnemy);
            EnemyPool.Add(newEnemy);

            return newEnemy;
        }
        // 2-2. 사용 가능한 Enemy가 있을 경우
        else // targetEnemy != default
        {
            return targetEnemy;
        }
    }

    public Enemy RefEnemy_1;
    public List<Enemy> EnemyPool_1 = new List<Enemy>();

    public Enemy GetEnemy_1()
    {
        // 1. 풀에 사용 가능한 오브젝트가 있는지 찾기
        //Enemy targetEnemy = EnemyPool_1.FirstOrDefault(enemy => enemy.gameObject.activeSelf == false);

        #region 1.에 대한 다른 방법
        Enemy targetEnemy = null;
        foreach (Enemy enemy in EnemyPool_1)
        {
            if (enemy.gameObject.activeSelf == false)
            {
                targetEnemy = enemy;
                break;
            }
        }
        #endregion

        // 2-1. 사용 가능한 Enemy가 없을 경우
        if (targetEnemy == default) //(== null)
        {
            // 새로운 걸 만들어서 사용 가능하게 하기
            Enemy newEnemy = Instantiate(RefEnemy_1);
            EnemyPool_1.Add(newEnemy);

            return newEnemy;
        }

        // 2-2. 사용 가능한 Enemy가 있을 경우
        return targetEnemy;
    }

    public Enemy RefEnemy_2;
    public List<Enemy> EnemyPool_2 = new List<Enemy>();

    public Enemy GetEnemy_2()
    {
        // 1. 풀에 사용 가능한 오브젝트가 있는지 찾기
        Enemy targetEnemy = EnemyPool_2.FirstOrDefault(enemy => enemy.gameObject.activeSelf == false);

        #region 1.에 대한 다른 방법

        #endregion

        // 2-1. 사용 가능한 Enemy가 없을 경우
        if (targetEnemy == default) //(== null)
        {
            // 새로운 걸 만들어서 사용 가능하게 하기
            Enemy newEnemy = Instantiate(RefEnemy_2);
            EnemyPool_2.Add(newEnemy);

            return newEnemy;
        }

        // 2-2. 사용 가능한 Enemy가 있을 경우
        return targetEnemy;
    }

    #endregion

    #region Fx Pool

    public ParticleSystem RefFx;
    public List<ParticleSystem> FxPool = new List<ParticleSystem>();
    public ParticleSystem GetFx()
    {
        // 1. 풀에 사용 가능한 오브젝트가 있는지 찾기
        ParticleSystem targetFx = FxPool.FirstOrDefault(Fx => Fx.gameObject.activeSelf == false);

        // 1.에 대한 다른 방법
       

        // 2-1. 사용 가능한 Fx가 없을 경우
        if (targetFx == default) //(== null)
        {
            // 새로운 걸 만들어서 사용 가능하게 하기
            ParticleSystem newFx = Instantiate(RefFx);
            FxPool.Add(newFx);

            return newFx;
        }
        // 2-2. 사용 가능한 Fx가 있을 경우
        else // targetEnemy != default
        {
            return targetFx;
        }
    }


    public ParticleSystem RefFx_1;
    public List<ParticleSystem> FxPool_1 = new List<ParticleSystem>();
    public ParticleSystem GetFx_1()
    {
        // 1. 풀에 사용 가능한 오브젝트가 있는지 찾기
        //ParticleSystem targetFx = FxPool_1.FirstOrDefault(Fx => Fx.gameObject.activeSelf == false);

        // 1.에 대한 다른 방법
        ParticleSystem targetFx = null; // (=default)
        foreach (ParticleSystem fx in FxPool_1)
        {
            if (fx.gameObject.activeSelf == false)
            {
                targetFx = fx;
                break;
            }
        }

        // 2-1. 사용 가능한 Fx가 없을 경우
        if (targetFx == default) //(== null)
        {
            // 새로운 걸 만들어서 사용 가능하게 하기
            ParticleSystem newFx = Instantiate(RefFx_1);
            FxPool_1.Add(newFx);

            return newFx;
        }
        // 2-2. 사용 가능한 Fx가 있을 경우
        else // targetEnemy != default
        {
            return targetFx;
        }
    }


    public ParticleSystem RefFx_2;
    public List<ParticleSystem> FxPool_2 = new List<ParticleSystem>();
    public ParticleSystem GetFx_2()
    {
        // 1. 풀에 사용 가능한 오브젝트가 있는지 찾기
        ParticleSystem targetFx = FxPool_2.FirstOrDefault(Fx => Fx.gameObject.activeSelf == false);

        // 1.에 대한 다른 방법


        // 2-1. 사용 가능한 Fx가 없을 경우
        if (targetFx == default) //(== null)
        {
            // 새로운 걸 만들어서 사용 가능하게 하기
            ParticleSystem newFx = Instantiate(RefFx_2);
            FxPool_2.Add(newFx);

            return newFx;
        }
        // 2-2. 사용 가능한 Fx가 있을 경우
        else // targetEnemy != default
        {
            return targetFx;
        }
    }

    #endregion
}

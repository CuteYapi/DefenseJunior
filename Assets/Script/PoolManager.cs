using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class PoolManager : MonoBehaviour
{
    #region Singleton

    public static PoolManager Pool { get; private set; }

    private void Awake()
    {
        Pool = this;
    }

    #endregion

    #region PlayerCharacter Pool

    public List<PlayerCharacter> RefPlayerCharacterList = new List<PlayerCharacter>();
    public Dictionary<PlayerCharacterType, List<PlayerCharacter>> PlayerCharacterPool = new Dictionary<PlayerCharacterType, List<PlayerCharacter>>();

    public PlayerCharacter GetPlayerCharacter(PlayerCharacterType type)
    {
        // 1. 풀에 리스트 자체가 없는 경우 리스트를 생성
        if (PlayerCharacterPool.TryGetValue(type, out List<PlayerCharacter> targetList) == false)
        {
            targetList = new List<PlayerCharacter>();
            PlayerCharacterPool.Add(type, targetList);
        }

        // 2. 풀에서 사용 가능 오브젝트 검색
        PlayerCharacter target = targetList.FirstOrDefault(playerCharacter => playerCharacter.gameObject.activeSelf == false);

        // 3. 없을 경우 새롭게 생성 후 리스트에 추가
        if (target == null)
        {
            target = Instantiate(RefPlayerCharacterList[(int)type], transform);
            target.name = $"{type}_{targetList.Count}";
            targetList.Add(target);
        }

        // 4. 최종 반환
        return target;
    }

    #endregion

    #region Enemy Pool

    public List<Enemy> RefEnemyList = new List<Enemy>(); // 각 타입별 참조용 Enemy 프리팹 리스트
    public Dictionary<EnemyType, List<Enemy>> EnemyPool = new Dictionary<EnemyType, List<Enemy>>();

    public Enemy GetEnemy(EnemyType type)
    {
        // 1. 풀에 리스트 자체가 없는 경우 리스트를 생성
        if (EnemyPool.TryGetValue(type, out List<Enemy> targetList) == false)
        {
            targetList = new List<Enemy>();
            EnemyPool.Add(type, targetList);
        }

        // 2. 풀에서 사용 가능 오브젝트 검색
        Enemy targetEnemy = targetList.FirstOrDefault(enemy => enemy.gameObject.activeSelf == false);

        // 3. 없을 경우 새롭게 생성 후 리스트에 추가
        if (targetEnemy == null)
        {
            targetEnemy = Instantiate(RefEnemyList[(int)type], transform);
            targetEnemy.name = $"{type}_{targetList.Count}";
            targetList.Add(targetEnemy);
        }

        // 4. 최종 반환
        return targetEnemy;
    }

    #endregion

    #region Fx Pool

    public List<ParticleSystem> RefFxList = new List<ParticleSystem>(); // 각 타입별 참조용 Fx 프리팹 리스트
    public Dictionary<FxType, List<ParticleSystem>> FxPool = new Dictionary<FxType, List<ParticleSystem>>();

    public ParticleSystem GetFx(FxType type)
    {
        // 1. 풀에 리스트 자체가 없는 경우 리스트를 생성
        if (FxPool.TryGetValue(type, out List<ParticleSystem> targetList) == false)
        {
            targetList = new List<ParticleSystem>();
            FxPool.Add(type, targetList);
        }

        // 2. 풀에서 사용 가능 오브젝트 검색
        ParticleSystem targetFx = targetList.FirstOrDefault(fx => fx.gameObject.activeSelf == false);

        // 3. 없을 경우 새롭게 생성 후 리스트에 추가
        if (targetFx == null)
        {
            targetFx = Instantiate(RefFxList[(int)type], transform);
            targetFx.name = $"{type}_{targetList.Count}";
            targetList.Add(targetFx);
        }

        // 4. 최종 반환
        return targetFx;
    }

    #endregion
}

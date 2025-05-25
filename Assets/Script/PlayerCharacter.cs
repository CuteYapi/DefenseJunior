using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerCharacter : MonoBehaviour
{
    [Header("Detection Settings")]
    [SerializeField] private float detectionRadius = 5f;
    [SerializeField] private LayerMask targetLayer;  // Inspector에서 공격할 대상의 레이어를 설정
    [SerializeField] private Color gizmoColor = Color.red;  // Gizmo 색상

    [Header("PlayerData")]
    public List<PlayerCharacterData> PlayerCharacterDataList = new List<PlayerCharacterData>();
    
    [SerializeField] private float attackDamage = 10f;
    [SerializeField] private float attackCooldown = 1f;
    public int BuildCost;

    [Header("Attack Settings")]
    [SerializeField] private float attackFXDuration = 1f; // 이펙트 지속 시간
    [SerializeField] private FxType attackFXType; // 이펙트 종류

    [Header("Upgrade System")]
    [SerializeField] private int currentLevel = 1; // 현재 레벨 (1부터 시작)
    private int currentDataIndex = 0; // 현재 사용 중인 데이터 인덱스

    private GameObject currentTarget;
    private float nextAttackTime;

    // 레벨 관련 프로퍼티
    public int CurrentLevel => currentLevel;
    public int MaxLevel => PlayerCharacterDataList.Count;
    public bool IsMaxLevel => currentLevel >= MaxLevel;

    public void Initialize(Vector3 spawnPosition)
    {
        gameObject.SetActive(true);
        transform.position = spawnPosition;

        // 초기 데이터 설정 (0번 인덱스)
        ApplyCharacterData(0);

        GameManager.Game.SpendGold(BuildCost);
    }

    public bool HasBuildableGold()
    {
        return GameManager.Game.CurrentGold >= BuildCost;
    }

    // 업그레이드 가능 여부 확인
    public bool CanUpgrade()
    {
        if (IsMaxLevel) return false;

        int nextDataIndex = currentDataIndex + 1;
        if (nextDataIndex >= PlayerCharacterDataList.Count) return false;

        int upgradeCost = GetUpgradeCost();
        return GameManager.Game.CurrentGold >= upgradeCost;
    }

    // 업그레이드 비용 계산
    public int GetUpgradeCost()
    {
        if (IsMaxLevel) return 0;

        int nextDataIndex = currentDataIndex + 1;
        if (nextDataIndex >= PlayerCharacterDataList.Count) return 0;

        return PlayerCharacterDataList[nextDataIndex].BuildCost;
    }

    public PlayerCharacterData GetPlayerCharacterData()
    {
        return PlayerCharacterDataList[currentDataIndex];
    }

    // 업그레이드 실행
    public bool Upgrade()
    {
        // 업그레이드 가능 여부 확인
        if (!CanUpgrade())
        {
            if (IsMaxLevel)
            {
                TextController.Text.SetMessageText(MessageType.MaxLevelReached);
            }
            else
            {
                TextController.Text.SetMessageText(MessageType.UpgradeNotPossible);
            }
            return false;
        }

        int nextDataIndex = currentDataIndex + 1;
        int upgradeCost = PlayerCharacterDataList[nextDataIndex].BuildCost;

        // 골드 확인 및 소모
        if (GameManager.Game.CurrentGold < upgradeCost)
        {
            TextController.Text.SetMessageText(MessageType.NotEnoughGoldForUpgrade);
            return false;
        }

        // 업그레이드 실행
        GameManager.Game.SpendGold(upgradeCost);
        currentDataIndex = nextDataIndex;
        currentLevel++;

        // 새로운 데이터 적용
        ApplyCharacterData(currentDataIndex);

        TextController.Text.SetMessageText(MessageType.UpgradeSuccess);
        return true;
    }

    // 캐릭터 데이터 적용
    private void ApplyCharacterData(int dataIndex)
    {
        if (dataIndex < 0 || dataIndex >= PlayerCharacterDataList.Count)
        {
            // 에러는 Debug.Log로 유지 (개발자용 정보)
            Debug.LogError($"잘못된 데이터 인덱스: {dataIndex}");
            return;
        }

        PlayerCharacterData data = PlayerCharacterDataList[dataIndex];
        attackDamage = data.AttackDamage;
        attackCooldown = data.AttackCooldown;
        BuildCost = data.BuildCost;

        currentDataIndex = dataIndex;
    }

    // 다음 레벨 정보 가져오기 (UI 표시용)
    public PlayerCharacterData GetNextLevelData()
    {
        if (IsMaxLevel) return null;

        int nextDataIndex = currentDataIndex + 1;
        if (nextDataIndex >= PlayerCharacterDataList.Count) return null;

        return PlayerCharacterDataList[nextDataIndex];
    }

    // 현재 레벨 정보 가져오기
    public PlayerCharacterData GetCurrentLevelData()
    {
        if (currentDataIndex >= PlayerCharacterDataList.Count) return null;
        return PlayerCharacterDataList[currentDataIndex];
    }

    private void Update()
    {
        // 현재 타겟이 없거나, 죽었거나, 범위를 벗어난 경우 새로운 타겟 찾기
        if (currentTarget == null ||
            !IsTargetAlive(currentTarget) ||
            !IsTargetInRange(currentTarget))
        {
            FindNewTarget();
        }

        // 타겟이 있고 공격 가능한 경우 공격
        if (currentTarget != null && Time.time >= nextAttackTime)
        {
            AttackTarget();
            nextAttackTime = Time.time + attackCooldown;
        }
    }

    // 타겟이 공격 범위 내에 있는지 확인하는 함수
    private bool IsTargetInRange(GameObject target)
    {
        if (target == null) return false;

        float distance = Vector3.Distance(transform.position, target.transform.position);
        return distance <= detectionRadius;
    }

    private void FindNewTarget()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRadius, targetLayer);
        float closestDistance = float.MaxValue;
        currentTarget = null;

        foreach (Collider collider in colliders)
        {
            GameObject target = collider.gameObject;

            // 자기 자신은 제외
            if (target == gameObject)
            {
                continue;
            }

            // 타겟이 살아있는지 확인
            if (!IsTargetAlive(target))
            {
                continue;
            }

            // 가장 가까운 타겟 선택
            float distance = Vector3.Distance(transform.position, target.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                currentTarget = target;
            }
        }
    }

    private bool IsTargetAlive(GameObject target)
    {
        // 타겟의 Enemy 컴포넌트를 확인
        var enemy = target.GetComponent<Enemy>();
        return enemy != null && enemy.IsAlive;
    }

    private void AttackTarget()
    {
        if (currentTarget == null) return;

        var enemy = currentTarget.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.Damaged(attackDamage);

            // 공격 이펙트 생성
            SpawnAttackFX(currentTarget.transform.position);

            // 공격 후 타겟이 죽었는지 확인
            if (!enemy.IsAlive)
            {
                currentTarget = null;
            }
        }
    }

    // 공격 이펙트를 생성하는 함수
    private void SpawnAttackFX(Vector3 position)
    {
        // 이펙트 가져오기 => 어떤 이펙트를 가져올지 구분하기
        ParticleSystem targetFx = PoolManager.Pool.GetFx(attackFXType);

        // 1. 이펙트를 대상 위치에 이펙트를 옮기고 재생하기
        targetFx.transform.position = position;
        targetFx.gameObject.SetActive(true);
        targetFx.Play();

        // 2. 이펙트 재생이 끝나면 제거하기
        StartCoroutine(SetOffFx(targetFx));
    }

    // 일정 시간 뒤 이펙트 제거하기
    private IEnumerator SetOffFx(ParticleSystem Fx)
    {
        yield return new WaitForSeconds(attackFXDuration);
        Fx.Stop();
        Fx.gameObject.SetActive(false);
    }

    // 디버그용으로 감지 범위를 에디터에서 시각화
    private void OnDrawGizmos()
    {
        // 항상 범위를 표시 (선택하지 않아도)
        Gizmos.color = gizmoColor;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        // 선을 더 잘 보이게 하기 위해 불투명도 높은 색상으로 선을 그림
        Gizmos.color = new Color(gizmoColor.r, gizmoColor.g, gizmoColor.b, 1f);
        DrawCircle(transform.position, detectionRadius, 32);

        // 현재 타겟이 있으면 타겟으로 선을 그림
        if (currentTarget != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, currentTarget.transform.position);
        }
    }

    // 원형 범위를 더 명확하게 표시하기 위한 헬퍼 메서드
    private void DrawCircle(Vector3 center, float radius, int segments)
    {
        float angle = 0f;
        Vector3 lastPoint = center + new Vector3(radius, 0, 0);
        Vector3 newPoint;

        for (int i = 0; i <= segments; i++)
        {
            angle += 360f / segments;
            newPoint = center + new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad) * radius, 0, Mathf.Sin(angle * Mathf.Deg2Rad) * radius);
            Gizmos.DrawLine(lastPoint, newPoint);
            lastPoint = newPoint;
        }
    }
}
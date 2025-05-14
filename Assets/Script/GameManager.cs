using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // 스테이지 관련 변수
    [Header("Stage Settings")]
    public List<SpawnData> SpawnDataList = new List<SpawnData>();
    [SerializeField] private int CurrentStageIndex;

    // 적 소환 관련 변수
    [Header("Enemy Spawn Settings")]
    [SerializeField] private Transform spawnPosition; // 적 소환 위치
    [SerializeField] private Transform targetPosition; // 적의 목표 위치
    [SerializeField] private float spawnTime = 2f; // 적 소환 간격
    private HashSet<Enemy> enemySet = new HashSet<Enemy>();

    // 플레이어 관련 변수
    [Header("Player Settings")]
    [SerializeField] private int playerLifePoint = 3; // 플레이어 생명력
    [SerializeField] private bool gameActive = true; // 게임 활성화 상태

    // UI 관련 변수 (필요시 추가)
    [Header("UI References")]
    [SerializeField] private UnityEngine.UI.Text lifePointText; // 생명력 표시 텍스트

    #region Singleton

    public static GameManager Game { get; private set; }

    private void Awake()
    {
        Game = this;
    }

    #endregion

    // 시작 시 호출
    private void Start()
    {
        // 초기 설정 확인
        if (spawnPosition == null || targetPosition == null)
        {
            Debug.LogError("GameManager에 필요한 참조가 설정되지 않았습니다!");
            return;
        }

        CurrentStageIndex = 0;

        // UI 업데이트
        UpdateLifePointUI();

        // 적 소환 코루틴 시작
        StartCoroutine(SpawnEnemyRoutine());
    }

    // 적 소환 코루틴
    private IEnumerator SpawnEnemyRoutine()
    {
        int spawnCount = 0;

        // 게임이 활성화된 동안 반복
        while (gameActive)
        {
            SpawnEnemy();
            spawnCount++;

            if(spawnCount >= SpawnDataList[CurrentStageIndex].Count)
            {
                CurrentStageIndex++;
                spawnCount = 0;
            }

            if (CurrentStageIndex >= SpawnDataList.Count)
            {
                CurrentStageIndex = 0;
            }

            yield return new WaitForSeconds(spawnTime);
        }
    }

    // 적 소환 메서드
    private void SpawnEnemy()
    {
        EnemyType enemyType = (EnemyType)SpawnDataList[CurrentStageIndex].Type;
        Enemy enemy = PoolManager.Pool.GetEnemy(enemyType);

        // 생성한 에너미 관리를 위해 Set에 저장
        enemySet.Add(enemy);

        // 타겟 위치 설정
        enemy.SetTargetPosition(targetPosition);

        // 생성 위치 설정
        enemy.transform.position = spawnPosition.position;

        // 상태 초기화
        enemy.StatusReset();

        // Enemy 도착 이벤트 등록 (OnEnemyReachedTarget 메서드를 호출하도록 설정)
        StartCoroutine(CheckEnemyReachedTarget(enemy, enemy.gameObject));
    }

    // 적이 목표에 도달했는지 확인하는 코루틴
    private IEnumerator CheckEnemyReachedTarget(Enemy enemy, GameObject enemyObject)
    {
        // 적이 살아있고, 목표 위치와 거리가 가까워질 때까지 대기
        while (enemy.IsAlive() && Vector3.Distance(enemyObject.transform.position, targetPosition.position) > 0.1f)
        {
            yield return null; // 다음 프레임까지 대기
        }

        // 적이 아직 살아있고 목표에 도달했다면
        if (enemy.IsAlive())
        {
            // 플레이어 생명력 감소
            DecreasePlayerLifePoint();

            // 적 비활성화 (Enemy 스크립트의 Dead 메서드는 private이므로 직접 호출할 수 없음)
            enemyObject.SetActive(false);
        }
    }

    // 플레이어 생명력 감소
    private void DecreasePlayerLifePoint()
    {
        playerLifePoint--;
        Debug.Log($"플레이어 생명력이 감소했습니다. 남은 생명력: {playerLifePoint}");

        // UI 업데이트
        UpdateLifePointUI();

        // 생명력이 0이 되면 게임 종료
        if (playerLifePoint <= 0)
        {
            GameOver();
        }
    }

    // UI 업데이트
    private void UpdateLifePointUI()
    {
        if (lifePointText != null)
        {
            lifePointText.text = $"Life: {playerLifePoint}";
        }
    }

    // 게임 종료
    private void GameOver()
    {
        gameActive = false;
        Debug.Log("게임 오버!");

        // 모든 적 비활성화
        foreach (Enemy enemy in enemySet)
        {
            enemy.Damaged(enemy.GetHp());
        }

        // 게임 오버 UI 표시 등 추가 기능 구현 (필요시)
    }

    // 게임 재시작 (필요시)
    public void RestartGame()
    {
        playerLifePoint = 3;
        gameActive = true;
        UpdateLifePointUI();

        // 기존 적 정리
        foreach (Enemy enemy in enemySet)
        {
            Destroy(enemy.gameObject);
        }

        // 적 소환 코루틴 재시작
        StartCoroutine(SpawnEnemyRoutine());

        Debug.Log("게임을 재시작합니다.");
    }
}
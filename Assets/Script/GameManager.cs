using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // UI 컴포넌트 접근을 위해 추가

public class GameManager : MonoBehaviour
{
    #region Singleton
    public static GameManager Game { get; private set; }

    private void Awake()
    {
        Game = this;
    }
    #endregion

    // 스테이지 관련 변수
    [Header("Stage Settings")]
    public List<SpawnData> SpawnDataList = new List<SpawnData>();

    private int _currentStageIndex;
    public int CurrentStageIndex
    {
        get => _currentStageIndex;
        set
        {
            _currentStageIndex = value;
            TextController.Text.SetStageText(CurrentStageIndex);
        }
    }

    // 적 소환 관련 변수
    [Header("Enemy Spawn Settings")]
    [SerializeField] private Transform spawnPosition; // 적 소환 위치
    [SerializeField] private Transform targetPosition; // 적의 목표 위치
    [SerializeField] private float spawnTime = 2f; // 적 소환 간격
    private HashSet<Enemy> enemySet = new HashSet<Enemy>();

    // 플레이어 관련 변수 (ResourceManager에서 통합)
    [Header("Player Resources")]
    // 보유한 골드
    private int _currentGold;
    public int CurrentGold
    {
        get => _currentGold;
        private set
        {
            if (value < 0)
            {
                value = 0;
                Debug.LogError("골드가 0 이하가 될 수는 없습니다!");
            }
            _currentGold = value;
            TextController.Text.SetGoldText(CurrentGold);
        }
    }

    // 보유한 생명력
    private int _currentLives;
    public int GetCurrentLives()
    {
        return _currentLives;
    }

    public void SetCurrentLives(int amount)
    {
        _currentLives = amount;
        TextController.Text.SetLifeText(GetCurrentLives());
    }

    [SerializeField] private int startGold; // 시작시 보유 골드
    [SerializeField] private int startLives; // 시작시 보유 생명력
    [SerializeField] private bool gameActive = true; // 게임 활성화 상태

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
        CurrentGold = startGold;
        SetCurrentLives(startLives);

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

            if (spawnCount >= SpawnDataList[CurrentStageIndex].Count)
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
        while (enemy.IsAlive && Vector3.Distance(enemyObject.transform.position, targetPosition.position) > 0.1f)
        {
            yield return null; // 다음 프레임까지 대기
        }

        // 적이 아직 살아있고 목표에 도달했다면
        if (enemy.IsAlive)
        {
            // 플레이어 생명력 감소
            TakeDamage(1);  // 한 명의 적당 1의 데미지

            // 적 비활성화
            enemyObject.SetActive(false);
        }
    }

    // ResourceManager에서 통합된 메서드들

    // 골드 소비 메서드
    public bool SpendGold(int amount)
    {
        if (CurrentGold >= amount)
        {
            CurrentGold -= amount;
            return true;
        }
        return false;
    }

    // 골드 획득 메서드
    public void AddGold(int amount)
    {
        CurrentGold += amount;
    }

    // 생명력 감소 메서드
    public void TakeDamage(int amount)
    {
        int newCurrentLives = GetCurrentLives() - amount;
        SetCurrentLives(newCurrentLives);

        // 생명력이 0이 되면 게임 종료
        if (GetCurrentLives() <= 0)
        {
            GameOver();
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
            enemy.Damaged(enemy.Hp);
        }

        // 게임 오버 UI 표시 등 추가 기능 구현 (필요시)
    }

    // 게임 재시작 (필요시)
    public void RestartGame()
    {
        // 자원 초기화
        CurrentGold = startGold;
        SetCurrentLives(startLives);
        CurrentStageIndex = 0;

        gameActive = true;

        // 기존 적 정리
        foreach (Enemy enemy in enemySet)
        {
            Destroy(enemy.gameObject);
        }
        enemySet.Clear();

        // 적 소환 코루틴 재시작
        StartCoroutine(SpawnEnemyRoutine());

        Debug.Log("게임을 재시작합니다.");
    }
}
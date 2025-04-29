using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // �������� ���� ����
    [Header("Stage Settings")]
    public List<SpawnData> SpawnDataList = new List<SpawnData>();
    [SerializeField] private int CurrentStageIndex;

    // �� ��ȯ ���� ����
    [Header("Enemy Spawn Settings")]
    [SerializeField] private GameObject enemyPrefab; // ��ȯ�� �� ������
    [SerializeField] private Transform spawnPosition; // �� ��ȯ ��ġ
    [SerializeField] private Transform targetPosition; // ���� ��ǥ ��ġ
    [SerializeField] private float spawnTime = 2f; // �� ��ȯ ����

    // �÷��̾� ���� ����
    [Header("Player Settings")]
    [SerializeField] private int playerLifePoint = 3; // �÷��̾� �����
    [SerializeField] private bool gameActive = true; // ���� Ȱ��ȭ ����

    // UI ���� ���� (�ʿ�� �߰�)
    [Header("UI References")]
    [SerializeField] private UnityEngine.UI.Text lifePointText; // ����� ǥ�� �ؽ�Ʈ

    // PoolManager
    [Header("PoolManager")]
    public PoolManager Pool;
    

    // ���� �� ȣ��
    private void Start()
    {
        // �ʱ� ���� Ȯ��
        if (enemyPrefab == null || spawnPosition == null || targetPosition == null)
        {
            Debug.LogError("GameManager�� �ʿ��� ������ �������� �ʾҽ��ϴ�!");
            return;
        }

        CurrentStageIndex = 0;

        // UI ������Ʈ
        UpdateLifePointUI();

        // �� ��ȯ �ڷ�ƾ ����
        StartCoroutine(SpawnEnemyRoutine());
    }

    // �� ��ȯ �ڷ�ƾ
    private IEnumerator SpawnEnemyRoutine()
    {
        int spawnCount = 0;

        // ������ Ȱ��ȭ�� ���� �ݺ�
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

    // �� ��ȯ �޼���
    private void SpawnEnemy()
    {
        Enemy enemyScript = null;
        switch (SpawnDataList[CurrentStageIndex].Type)
        {
            case 1:
                enemyScript = Pool.GetEnemy();
                break;
            case 2:
                enemyScript = Pool.GetEnemy_1();
                break;
            case 3:
                enemyScript = Pool.GetEnemy_2();
                break;
            default:
                Debug.LogError("�ùٸ��� �ʴ� Enemy Ÿ���� �ԷµǾ����ϴ�!");
                goto case 1;
        }

        // Enemy ��ũ��Ʈ�� �ִ��� Ȯ��
        if (enemyScript != null)
        {
            // Ÿ�� ��ġ ����
            enemyScript.SetTargetPosition(targetPosition);

            // ���� ��ġ ����
            enemyScript.transform.position = spawnPosition.position;
            
            // ���� �ʱ�ȭ
            enemyScript.StatusReset();

            // Enemy ���� �̺�Ʈ ��� (OnEnemyReachedTarget �޼��带 ȣ���ϵ��� ����)
            StartCoroutine(CheckEnemyReachedTarget(enemyScript, enemyScript.gameObject));
        }
        else
        {
            Debug.LogError("������ ���� Enemy ��ũ��Ʈ�� �����ϴ�!");
        }
    }

    // ���� ��ǥ�� �����ߴ��� Ȯ���ϴ� �ڷ�ƾ
    private IEnumerator CheckEnemyReachedTarget(Enemy enemy, GameObject enemyObject)
    {
        // ���� ����ְ�, ��ǥ ��ġ�� �Ÿ��� ������� ������ ���
        while (enemy.IsAlive() && Vector3.Distance(enemyObject.transform.position, targetPosition.position) > 0.1f)
        {
            yield return null; // ���� �����ӱ��� ���
        }

        // ���� ���� ����ְ� ��ǥ�� �����ߴٸ�
        if (enemy.IsAlive())
        {
            // �÷��̾� ����� ����
            DecreasePlayerLifePoint();

            // �� ��Ȱ��ȭ (Enemy ��ũ��Ʈ�� Dead �޼���� private�̹Ƿ� ���� ȣ���� �� ����)
            enemyObject.SetActive(false);
        }
    }

    // �÷��̾� ����� ����
    private void DecreasePlayerLifePoint()
    {
        playerLifePoint--;
        Debug.Log($"�÷��̾� ������� �����߽��ϴ�. ���� �����: {playerLifePoint}");

        // UI ������Ʈ
        UpdateLifePointUI();

        // ������� 0�� �Ǹ� ���� ����
        if (playerLifePoint <= 0)
        {
            GameOver();
        }
    }

    // UI ������Ʈ
    private void UpdateLifePointUI()
    {
        if (lifePointText != null)
        {
            lifePointText.text = $"Life: {playerLifePoint}";
        }
    }

    // ���� ����
    private void GameOver()
    {
        gameActive = false;
        Debug.Log("���� ����!");

        // ��� �� ��Ȱ��ȭ (������)
        Enemy[] allEnemies = FindObjectsOfType<Enemy>();
        foreach (Enemy enemy in allEnemies)
        {
            enemy.Damaged(enemy.GetHp());
        }

        // ���� ���� UI ǥ�� �� �߰� ��� ���� (�ʿ��)
    }

    // ���� ����� (�ʿ��)
    public void RestartGame()
    {
        playerLifePoint = 3;
        gameActive = true;
        UpdateLifePointUI();

        // ���� �� ����
        Enemy[] allEnemies = FindObjectsOfType<Enemy>();
        foreach (Enemy enemy in allEnemies)
        {
            Destroy(enemy.gameObject);
        }

        // �� ��ȯ �ڷ�ƾ �����
        StartCoroutine(SpawnEnemyRoutine());

        Debug.Log("������ ������մϴ�.");
    }
}
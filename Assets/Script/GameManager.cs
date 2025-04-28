using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
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

    // ���� �� ȣ��
    private void Start()
    {
        // �ʱ� ���� Ȯ��
        if (enemyPrefab == null || spawnPosition == null || targetPosition == null)
        {
            Debug.LogError("GameManager�� �ʿ��� ������ �������� �ʾҽ��ϴ�!");
            return;
        }

        // UI ������Ʈ
        UpdateLifePointUI();

        // �� ��ȯ �ڷ�ƾ ����
        StartCoroutine(SpawnEnemyRoutine());
    }

    // �� ��ȯ �ڷ�ƾ
    private IEnumerator SpawnEnemyRoutine()
    {
        // ������ Ȱ��ȭ�� ���� �ݺ�
        while (gameActive)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(spawnTime);
        }
    }

    // �� ��ȯ �޼���
    private void SpawnEnemy()
    {
        // �� ������ ����
        GameObject newEnemy = Instantiate(enemyPrefab, spawnPosition.position, Quaternion.identity);

        // Enemy ��ũ��Ʈ ���� ��������
        Enemy enemyScript = newEnemy.GetComponent<Enemy>();

        // Enemy ��ũ��Ʈ�� �ִ��� Ȯ��
        if (enemyScript != null)
        {
            // Ÿ�� ��ġ ����
            enemyScript.SetTargetPosition(targetPosition);

            // Enemy ���� �̺�Ʈ ��� (OnEnemyReachedTarget �޼��带 ȣ���ϵ��� ����)
            StartCoroutine(CheckEnemyReachedTarget(enemyScript, newEnemy));
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
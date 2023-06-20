using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] private GameObject m_enemyPrefab;

    [SerializeField] private EnemyData m_enemyDataNormal;
    [SerializeField] private EnemyData m_enemyDataSpeeder;

    [SerializeField] private Transform[] m_respawnPosition = new Transform[4];

    [SerializeField] private List<GameObject> m_enemies = new List<GameObject>();

    public UnityEvent<string> OnEnemyRespawn;

    private void Awake()
    {
        OnEnemyRespawn.AddListener(OnEnemyRespawnHandler);
    }

    public void CreateEnemy()
    {
        var l_respawnPosition = m_respawnPosition[UnityEngine.Random.Range(0, m_respawnPosition.Length)];

        var newEnemy = Instantiate(m_enemyPrefab, l_respawnPosition.position, Quaternion.identity);

        OnEnemyRespawn?.Invoke(name);
        m_enemies.Add(newEnemy);

        EnemyController enemyController = newEnemy.GetComponent<EnemyController>();

        var l_random = UnityEngine.Random.Range(0, 2);
        if (l_random <= 0.9)
        {
            enemyController.InitEnemy(m_enemyDataSpeeder, GameManager.GetInstance().GetPlayerControl().gameObject);
        }
        else
        {
            enemyController.InitEnemy(m_enemyDataNormal, GameManager.GetInstance().GetPlayerControl().gameObject);
        }
    }

    // events

    private void OnEnemyRespawnHandler(string p_origin)
    {
        Debug.Log($"EnemyRespawn event. Called by {p_origin}. Executed in {name}");
    }
}

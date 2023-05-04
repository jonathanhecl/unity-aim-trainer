using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private PlayerController m_playerControl;
    [SerializeField] private GameObject m_enemyPrefab;

    public bool m_inmortalPlayer = false;

    private static GameManager m_instance;

    private void Awake()
    {
        if (m_instance == null)
        {
            m_instance = this;
        } else
        {
            Destroy(gameObject);
        }
    }

    public static GameManager GetInstance()
    {
        return m_instance;
    }

    public void NewEnemy()
    {
        var obj = Instantiate(m_enemyPrefab, new Vector3(-130.0f, 0.0f, 0.0f), Quaternion.identity);

        EnemyController enemyController = obj.GetComponent<EnemyController>();
        enemyController.m_targetControl = m_playerControl.gameObject;
    }

    public void HandleGameOver()
    {
        Debug.Log("Game Over");
    }
}

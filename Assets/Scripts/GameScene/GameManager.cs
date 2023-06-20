using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private PlayerController m_playerControl;
    [SerializeField] private GameObject m_enemyPrefab;

    [SerializeField] private EnemyData m_enemyDataNormal;
    [SerializeField] private EnemyData m_enemyDataSpeeder;

    [SerializeField] private Transform[] m_respawnPosition = new Transform[4];

    [SerializeField] private List<GameObject> m_enemies = new List<GameObject>();

    // Volumes

    [SerializeField] private Volume m_volumeGlobal;
    [SerializeField] private VolumeProfile m_volumePostProcess;
    [SerializeField] private VolumeProfile m_volumeDamage;

    // UI

    [SerializeField] private TMP_Text m_scoreText;
    [SerializeField] private Slider m_playerHPSlider;

    [SerializeField] private Image m_potionImage;
    [SerializeField] private float m_intervalPotion = 1.0f;

    [SerializeField] private Image m_swordImage;
    [SerializeField] private float m_intervalSword = 1.0f;

    [SerializeField] private Image m_spellImage;
    [SerializeField] private float m_intervalSpell = 1.0f;

    // score

    private int m_score = 0;

    // intervals

    private float m_timePotion = 1.0f;
    private float m_timeSword = 1.0f;
    private float m_timeSpell = 1.0f;

    // events

    public UnityEvent<string> OnEnemyRespawn;
    public UnityEvent<string> OnChangeMap;

    private static GameManager m_instance;

    private void Awake()
    {
        if (m_instance == null)
        {
            m_instance = this;
            OnEnemyRespawn.AddListener(OnEnemyRespawnHandler);
            OnChangeMap.AddListener(OnChangeMapHandler);
        } else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        Debug.Assert(m_scoreText);
        Debug.Assert(m_playerHPSlider);

        m_timePotion = m_intervalPotion;
        m_timeSword = m_intervalSword;
        m_timeSpell = m_intervalSpell;
        m_volumeGlobal.profile = m_volumePostProcess;

        ResetScore();

        CreateEnemy();
    }

    public static GameManager GetInstance()
    {
        return m_instance;
    }

    public PlayerController GetPlayerControl()
    {
        return m_playerControl;
    }

    public bool IsPlayerAlive()
    {
        return m_playerControl.IsAlive();
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
            enemyController.InitEnemy(m_enemyDataSpeeder, m_playerControl.gameObject);
        }
        else
        {
            enemyController.InitEnemy(m_enemyDataNormal, m_playerControl.gameObject);
        }
    }

    // intervals

    public bool UsePotion()
    {
        if (m_timePotion < m_intervalPotion)
        {
            return false;
        }

        m_timePotion = 0.0f;
        return true;
    }

    public bool UseSword()
    {
        if (m_timeSword < m_intervalSword)
        {
            return false;
        }

        m_timeSword = 0.0f;
        return true;
    }

    public bool CanUseSpell()
    {
        return m_timeSpell >= m_intervalSpell;
    }

    public bool LoadSpell()
    {
        if (!CanUseSpell())
        {
            return false;
        }

        m_timeSpell = 0.0f;

        return true;
    }

    // UI

    private void Update()
    {
        // Potion
        if (m_timePotion < m_intervalPotion)
        {
            m_timePotion += Time.deltaTime;
            if (m_timePotion >= m_intervalPotion)
            {
                m_timePotion = m_intervalPotion;
            }
            RefreshPotion();
        }

        // Sword
        if (m_timeSword < m_intervalSword)
        {
            m_timeSword += Time.deltaTime;
            if (m_timeSword >= m_intervalSword)
            {
                m_timeSword = m_intervalSword;
            }
            RefreshSword();
        }

        // Spell
        if (m_timeSpell < m_intervalSpell)
        {
            m_timeSpell += Time.deltaTime;
            if (m_timeSpell >= m_intervalSpell)
            {
                m_timeSpell = m_intervalSpell;
            }
            RefreshSpell();
        }

    }

    public bool CanChangeMap()
    {
        if (m_score > 10)
        {
            return true;
        }
        return false;
    }

    public void ChangeMap()
    {
        var l_scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
        if (l_scene.name == "GameScene")
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("GameSceneLava");
        }
        else
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");
        }
        m_score = 0;
    }

    public void HandleGameOver()
    {
        Debug.Log("Game Over");
    }

    public void ResetScore()
    {
        m_score = 0;
        RefreshScore();
    }

    public void AddScore(int p_score)
    {
        m_score += p_score;
        RefreshScore();
    }

    private void RefreshScore()
    {
        m_scoreText.text = "Score: " + m_score.ToString();
    }

    public void ResetPlayerHP(float maxHP)
    {
        m_playerHPSlider.gameObject.SetActive(true);
        RefreshPlayerHP(maxHP, maxHP);
    }

    public void RefreshPlayerHP(float p_minHP, float p_maxHP)
    {
        m_playerHPSlider.value = (p_minHP * 1 / p_maxHP);
        if (p_minHP <= 0)
        {
            m_playerHPSlider.gameObject.SetActive(false);
            HandleGameOver();
        } else if (p_minHP < p_maxHP)
        {
            if (m_volumeGlobal.profile != m_volumeDamage)
            {
                m_volumeGlobal.profile = m_volumeDamage;
            }
            m_volumeDamage.TryGet(out Vignette _vignette);
            m_volumeDamage.TryGet(out ColorCurves _colorcurves);
            _vignette.intensity.value = 1 - (p_minHP * 1 / p_maxHP);
            if (p_minHP < p_maxHP / 2)
            {
                _colorcurves.active = true;
            }
            else
            {
                _colorcurves.active = false;
            }

        }
        else
        {
            if (m_volumeGlobal.profile != m_volumePostProcess)
            {
                m_volumeGlobal.profile = m_volumePostProcess;
            }
        }
    }

    private void RefreshPotion()
    {
        m_potionImage.fillAmount = (m_timePotion * 1 / m_intervalPotion);
    }

    private void RefreshSword()
    {
        m_swordImage.fillAmount = (m_timeSword * 1 / m_intervalSword);
    }

    private void RefreshSpell()
    {
        m_spellImage.fillAmount = (m_timeSpell * 1 / m_intervalSpell);
    }

    // events

    private void OnEnemyRespawnHandler(string p_origin)
    {
        Debug.Log($"EnemyRespawn event. Called by {p_origin}. Executed in {name}");
    }

    private void OnChangeMapHandler(string p_origin)
    {
        Debug.Log($"OnChangeMap event. Called by {p_origin}. Executed in {name}");
    }
}

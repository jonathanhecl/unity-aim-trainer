using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public struct SpellInfo
{
    public GameManager.SpellLoaded m_spellType;
    public string m_name;
    public float m_time;
    public float m_interval;
    public Image m_image;
}

public class GameManager : MonoBehaviour
{
    public UnityEvent<string> OnEnemyRespawn;
    public UnityEvent<string> OnPlayerMove;
    public UnityEvent<string> OnPlayerAttack;
    public UnityEvent<string> OnPlayerDie;

    [SerializeField] private Volume m_volumeGlobal;
    [SerializeField] private VolumeProfile m_volumePostProcess;
    [SerializeField] private VolumeProfile m_volumeDamage;

    [SerializeField] private PlayerController m_playerControl;
    [SerializeField] private GameObject m_enemyPrefab;
    [SerializeField] private EnemyData m_enemyDataNormal;
    [SerializeField] private EnemyData m_enemyDataSpeeder;

    [SerializeField] private Transform[] m_respawnPosition = new Transform[4];

    [SerializeField] private List<GameObject> m_enemies = new List<GameObject>();
    [SerializeField] private TMP_Text m_scoreText;
    [SerializeField] private Slider m_playerHPSlider;

    [SerializeField] private Image m_potionImage;
    [SerializeField] private float m_intervalPotion = 1.0f;

    [SerializeField] private Image m_swordImage;
    [SerializeField] private float m_intervalSword = 1.0f;

    //[SerializeField] private GameObject m_spellsContent;
    //[SerializeField] private GameObject m_spellPrefab;
    public List<SpellInfo> m_spellsList = new List<SpellInfo>();
    [SerializeField] private Image m_spellImage;
    [SerializeField] private float m_intervalSpell = 1.0f;

    [SerializeField] private float m_maxHP = 200.0f;

    private int m_score = 0;
    private float m_playerHP = 0;

    private float m_timePotion = 1.0f;
    private float m_timeSword = 1.0f;
    private float m_timeSpell = 1.0f;

    public bool m_inmortalPlayer = false;
    private SpellLoaded m_spellLoaded = 0;
    
    public enum SpellLoaded { 
        None, 
        Attack, 
        Cure, 
        Paralysis };

    private static GameManager m_instance;

    private void Awake()
    {
        if (m_instance == null)
        {
            m_instance = this;
            OnEnemyRespawn.AddListener(OnEnemyRespawnHandler);
            OnPlayerMove.AddListener(OnPlayerMoveHandler);
            OnPlayerAttack.AddListener(OnPlayerAttackHandler);
            OnPlayerDie.AddListener(OnPlayerDieHandler);
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
        ResetPlayerHP();

        CreateEnemy();
    }

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

    public static GameManager GetInstance()
    {
        return m_instance;
    }

    public PlayerController GetPlayerControl()
    {
        return m_playerControl;
    }

    public void SetSpell(SpellLoaded spellType)
    {
        if (!CanUseSpell())
        {
           return;
        }

        m_spellLoaded = spellType;
        m_timeSpell = 0.0f;

        Debug.Log("Spell " + spellType.ToString() + " loaded");
    }

    public bool CanUseSpell()
    {
        return m_timeSpell >= m_intervalSpell;
    }

    public SpellLoaded UseSpell()
    {
        var l_prevSpell = m_spellLoaded;

        if (m_spellLoaded == SpellLoaded.None)
        {
            return l_prevSpell;
        }

        switch (m_spellLoaded)
        {
            case SpellLoaded.Attack:
                m_playerControl.m_playerCharacter.GetComponent<Animator>().SetTrigger("MagicAttack");
                m_playerControl.m_audioSource.PlayOneShot(m_playerControl.m_audioSpellAttack);
                break;
            case SpellLoaded.Cure:
                //m_playerControl.m_audioSource.PlayOneShot(m_playerControl.m_audioSpellCure);
                break;
            case SpellLoaded.Paralysis:
                //m_playerControl.m_audioSource.PlayOneShot(m_playerControl.m_audioSpellParalysis);
                break;
        }

        OnPlayerAttack?.Invoke(name);
        m_spellLoaded = SpellLoaded.None;

        return l_prevSpell;
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

    public void ResetPlayerHP()
    {
        m_playerHP = m_maxHP;
        m_playerHPSlider.gameObject.SetActive(true);
        RefreshPlayerHP();
    }

    public float GetPlayerHP()
    {
        return m_playerHP;
    }

    public void HandlePlayerDamage(float p_damage)
    {
        if (m_inmortalPlayer)
        {
            return;
        }
        m_playerHP -= p_damage;

        if (m_playerHP <= 0)
        {
            m_playerHPSlider.gameObject.SetActive(false);
            HandleGameOver();
        } else if (m_playerHP > m_maxHP)
        {
            m_playerHP = m_maxHP;
        }
        RefreshPlayerHP();
    }

    private void RefreshPlayerHP()
    {
        m_playerHPSlider.value = (m_playerHP * 1 / m_maxHP);
        if (m_playerHP < m_maxHP)
        {
            if (m_volumeGlobal.profile != m_volumeDamage)
            {
                m_volumeGlobal.profile = m_volumeDamage;
            }
            m_volumeDamage.TryGet(out Vignette _vignette);
            m_volumeDamage.TryGet(out ColorCurves _colorcurves);
            _vignette.intensity.value = 1 - (m_playerHP * 1 / m_maxHP);
            if (m_playerHP < m_maxHP / 2)
            {
                _colorcurves.active = true;
            } else
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

    public bool UsePotion()
    {
        if (m_timePotion < m_intervalPotion)
        {
            return false;
        }

        m_timePotion = 0.0f;
        return true;
    }

    private void RefreshPotion()
    {
        m_potionImage.fillAmount = (m_timePotion * 1 / m_intervalPotion) ;
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

    private void RefreshSword()
    {
        m_swordImage.fillAmount = (m_timeSword * 1 / m_intervalSword);
    }

    private void RefreshSpell()
    {
        m_spellImage.fillAmount = (m_timeSpell * 1 / m_intervalSpell);
    }

    // Events

    private void OnEnemyRespawnHandler(string p_origin)
    {
        Debug.Log($"EnemyRespawn event. Called by {p_origin}. Executed in {name}");
    }

    private void OnPlayerMoveHandler(string p_origin)
    {
        Debug.Log($"PlayerMove event. Called by {p_origin}. Executed in {name}");
    }

    private void OnPlayerAttackHandler(string p_origin)
    {
        Debug.Log($"PlayerAttack event. Called by {p_origin}. Executed in {name}");
    }

    private void OnPlayerDieHandler(string p_origin)
    {
        Debug.Log($"PlayerDie event. Called by {p_origin}. Executed in {name}");
    }
}

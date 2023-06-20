using System;
using System.Collections.Generic;
using TMPro;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

struct Timer
{
    public int minutes;
    public int second;
    public int milisecond;

    public Timer(int m, int s, int ms)
    {
        minutes = m;
        second = s;
        milisecond = ms;
    }
}

public class GameManager : MonoBehaviour
{
    [SerializeField] private EnemyManager m_enemyManager;
    [SerializeField] private PlayerController m_playerControl;

    // Volumes

    [SerializeField] private Volume m_volumeGlobal;
    [SerializeField] private VolumeProfile m_volumePostProcess;
    [SerializeField] private VolumeProfile m_volumeDamage;

    // UI

    [SerializeField] private TMP_Text m_scoreText;
    [SerializeField] private TMP_Text m_timerText;
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

    private float m_started;
    private float m_timePotion = 1.0f;
    private float m_timeSword = 1.0f;
    private float m_timeSpell = 1.0f;

    // events

    public UnityEvent<string> OnChangeMap;

    private static GameManager m_instance;

    private void Awake()
    {
        if (m_instance == null)
        {
            m_instance = this;
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

        m_started = Time.time;

        ResetScore();

        m_enemyManager.CreateEnemy();
    }

    public static GameManager GetInstance()
    {
        return m_instance;
    }

    public PlayerController GetPlayerControl()
    {
        return m_playerControl;
    }

    public EnemyManager GetEnemyManager()
    {
        return m_enemyManager;
    }

    public bool IsPlayerAlive()
    {
        return m_playerControl.IsAlive();
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
        RefreshTimer();

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

        if (Input.GetKeyDown(KeyCode.X))
        {
            m_enemyManager.CreateEnemy();
        }
    }

    private Timer GetTimer(float p_init)
    {
        var l_since = Time.time - p_init;

        var l_minutes = Convert.ToInt32(Math.Floor(l_since / 60));
        var l_seconds = Convert.ToInt32(Math.Floor(l_since) - (l_minutes * 60));
        var l_miliseconds = Convert.ToInt32(Math.Floor((l_since - ((l_minutes * 60) + l_seconds)) * 1000));

        return new Timer(l_minutes, l_seconds, l_miliseconds);
    }

    private void RefreshTimer()
    {
        if (!m_playerControl.IsAlive())
        {
            return;
        }

        var l_timer = GetTimer(m_started);
        var l_minutes = "";
        var l_seconds = "";
        var l_miliseconds = "";


        if (l_timer.minutes.ToString().Length == 1)
        {
            l_minutes = "0" + l_timer.minutes.ToString();
        } else
        {
            l_minutes =  l_timer.minutes.ToString();
        }

        if (l_timer.second.ToString().Length == 1)
        {
            l_seconds = "0" + l_timer.second.ToString();
        }
        else
        {
            l_seconds = l_timer.second.ToString();
        }

        if (l_timer.milisecond.ToString().Length == 2)
        {
            l_miliseconds = "0" + l_timer.milisecond.ToString();
        } else if (l_timer.milisecond.ToString().Length == 1)
        {
            l_miliseconds = "00" + l_timer.milisecond.ToString();
        }
        else
        {
            l_miliseconds = l_timer.milisecond.ToString();
        }
        
        m_timerText.text = l_minutes + ":" + l_seconds + ":" + l_miliseconds;
    }

    public bool CanChangeMap()
    {
        if (m_score >= 10)
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

    private void OnChangeMapHandler(string p_origin)
    {
        Debug.Log($"OnChangeMap event. Called by {p_origin}. Executed in {name}");
    }
}

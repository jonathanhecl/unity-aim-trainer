using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerController : MonoBehaviour
{
    private GridLogic grid;

    public bool m_inmortalPlayer = false;
    [SerializeField] private float m_maxHP = 200.0f;
    private float m_playerHP = 0;

    private SpellLoaded m_spellLoaded = 0;

    private Vector3 m_direction = Vector3.zero;

    [SerializeField] private GameObject m_playerControl;
    [SerializeField] public GameObject m_playerCharacter;
    [SerializeField] private GameObject m_playerDamageArea;

    [SerializeField] private GameObject m_playerBloodEffect;
    [SerializeField] private GameObject m_playerHealthEffect;

    [SerializeField] public bool m_isMoving = false;

    public AudioSource m_audioSource;

    [SerializeField] public AudioClip m_audioAttackMiss;
    [SerializeField] public AudioClip m_audioAttackHit;
    [SerializeField] public AudioClip m_audioCure;
    [SerializeField] public AudioClip m_audioHurt;
    [SerializeField] public AudioClip m_audioDeath;
    [SerializeField] public AudioClip m_audioRevive;
    [SerializeField] public AudioClip m_audioSpellAttack;

    private Vector3 m_fixUp;

    public UnityEvent<string> OnPlayerMove;
    public UnityEvent<string> OnPlayerAttack;
    public UnityEvent<string> OnPlayerDie;

    private void Start()
    {
        grid = gameObject.AddComponent<GridLogic>();
        m_playerCharacter.transform.localPosition = Vector3.zero;
        m_audioSource = GetComponent<AudioSource>();
        m_direction = Vector3.zero;
        m_fixUp = (transform.up * 4);
    }

    private void Awake()
    {
        m_playerHP = m_maxHP;

        OnPlayerMove.AddListener(OnPlayerMoveHandler);
        OnPlayerAttack.AddListener(OnPlayerAttackHandler);
        OnPlayerDie.AddListener(OnPlayerDieHandler);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            GameManager.GetInstance().CreateEnemy();
        }

        if (m_playerHP <= 0)
        {
            HandleRevive();
            return;
        }

        HandleSpells();
        HandleCure();
        HandleAttack();
        if (m_isMoving) {
            return;
        } else
        {
            HandleMovement();
        }
    }

    public bool IsAlive()
    {
        return m_playerHP > 0;
    }

    private void CreateBlood()
    {
        var l_blood = Instantiate(m_playerBloodEffect);
        l_blood.transform.position = m_playerCharacter.transform.position + Vector3.up * 3;
    }

    private EnemyController IsEnemyInFront()
    {
        RaycastHit hit;
        if (Physics.Raycast((m_playerDamageArea.transform.position + m_fixUp), m_direction , out hit, 12))
        {
            if (hit.collider.gameObject.tag == "Enemy")
            {
                var enemy = hit.collider.gameObject.GetComponent<EnemyController>();
                if (enemy.IsAlive()) {
                    return enemy;
                }
            }
        }
  
        return null;
    }

    private void HandleRevive()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            m_playerHP = m_maxHP;
            m_audioSource.PlayOneShot(m_audioRevive);
            GameManager.GetInstance().ResetPlayerHP(m_maxHP);
            m_playerCharacter.GetComponent<Animator>().SetBool("Alive", true);
            GameManager.GetInstance().ResetScore();
        }
    }

    private void HandleSpells()
    {

        if (Input.GetKeyDown(KeyCode.H))
        {
            if (!GameManager.GetInstance().CanUseSpell())
            {
                return;
            }
            SetSpell(SpellLoaded.Attack);
        }
    }

    public void SetSpell(SpellLoaded spellType)
    {
        if (!GameManager.GetInstance().LoadSpell())
        {
            return;
        }

        m_spellLoaded = spellType;
        Debug.Log("Spell " + spellType.ToString() + " loaded");
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
                m_playerCharacter.GetComponent<Animator>().SetTrigger("MagicAttack");
                m_audioSource.PlayOneShot(m_audioSpellAttack);
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

    private void HandleCure()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            if (GameManager.GetInstance().UsePotion())
            {
                m_audioSource.PlayOneShot(m_audioCure);
                HandleHealth(50);
            }
        }
    }

    private void CreateHealth()
    {
        var l_health = Instantiate(m_playerHealthEffect);
        l_health.transform.position = m_playerCharacter.transform.position + Vector3.up * 3;
    }

    public void HandleHealth(float p_heal)
    {
        m_playerHP += p_heal;
        if (m_playerHP > m_maxHP)
        {
            m_playerHP = m_maxHP;
        }

        GameManager.GetInstance().RefreshPlayerHP(m_playerHP, m_maxHP);
        CreateHealth();
    }

    public void HandleHurt(float p_damage)
    {
        if (m_playerHP <= 0)
        {
            return;
        }

        if (!m_inmortalPlayer)
        {
            m_playerHP -= p_damage;
        }

        m_playerCharacter.transform.localPosition = Vector3.zero; 
        m_playerCharacter.GetComponent<Animator>().SetTrigger("Hit");
        if (m_playerHP <= 0)
        {
            m_audioSource.PlayOneShot(m_audioDeath);
            m_playerCharacter.GetComponent<Animator>().SetBool("Alive", false);
            OnPlayerDie?.Invoke(name);
        }
        else
        {
            m_audioSource.PlayOneShot(m_audioHurt);
        }

        if (m_playerHP < 0)
        {
            m_playerHP = 0;
        }

        GameManager.GetInstance().RefreshPlayerHP(m_playerHP, m_maxHP);
        CreateBlood();
    }

    private void HandleAttack()
    {
        Debug.DrawRay(m_playerDamageArea.transform.position + m_fixUp, m_direction *12 , Color.red, 0.3f);

        if (Input.GetKeyDown(KeyCode.LeftControl) ||
            Input.GetKeyDown(KeyCode.RightControl) )
        {
            if (!GameManager.GetInstance().UseSword()) {
                return;
            }

            OnPlayerAttack?.Invoke(name);

            m_playerCharacter.GetComponent<Animator>().SetTrigger("PhysicalAttack");

            var l_target = IsEnemyInFront();
            if (l_target != null)
            { 
                m_audioSource.PlayOneShot(m_audioAttackHit);
                l_target.HandleHurt(50);
            } else
            {
                m_audioSource.PlayOneShot(m_audioAttackMiss);
            }
        }
    }

    private void HandleMovement()
    {
        var l_vertical = Input.GetAxis("Vertical");
        var l_horizontal = Input.GetAxis("Horizontal");
        Vector3 l_direction;

        if (l_vertical != 0.0f)
        {
            if (l_vertical > 0.0f)
            {
                l_direction = transform.forward;
            } else
            {
                l_direction = -transform.forward; 
            }
        } else if (l_horizontal != 0.0f) { 
            if (l_horizontal > 0.0f)
            {
                l_direction = transform.right;
            } else
            {
                l_direction = -transform.right;
            }
        } else
        {
            return;
        }

        if (!grid.PlayerCanMove(m_playerControl, l_direction) || 
            IsEnemyInFront() != null)  // Can't move only rotate
        {
            m_playerCharacter.transform.rotation = Quaternion.LookRotation(l_direction);

            return;
        }

        OnPlayerMove?.Invoke(name);

        m_playerCharacter.GetComponent<Animator>().SetBool("Running",true);
        m_direction = l_direction;
        m_isMoving = true;
        m_playerDamageArea.transform.position = m_playerCharacter.transform.position + l_direction;
        StartCoroutine(grid.Movement(m_playerControl, m_playerCharacter, l_direction, 0.0f));
    }

    // events

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

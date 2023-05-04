using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    private GridLogic grid;
    private Vector3 m_direction = Vector3.zero;

    [SerializeField] private float m_maxHP = 200.0f;
    [SerializeField] private float m_currentHP = 200.0f;

    [SerializeField] private GameObject m_playerControl;
    [SerializeField] public GameObject m_playerCharacter;
    [SerializeField] private GameObject m_playerDamageArea;
    [SerializeField] private GameObject m_playerBlood;
    [SerializeField] public bool m_isMoving = false;

    public AudioSource m_audioSource;

    [SerializeField] public AudioClip m_audioAttackMiss;
    [SerializeField] public AudioClip m_audioAttackHit;
    [SerializeField] public AudioClip m_audioCure;
    [SerializeField] public AudioClip m_audioHurt;
    [SerializeField] public AudioClip m_audioDeath;
    [SerializeField] public AudioClip m_audioRevive;
    [SerializeField] public AudioClip m_audioSpellAttack;

    private float m_lastCure;
    private float m_lastAttack;
    private float m_lastSpell;

    private Vector3 m_fixUp;

    private void Start()
    {
        grid = gameObject.AddComponent<GridLogic>();
        m_playerCharacter.transform.localPosition = Vector3.zero;
        m_audioSource = GetComponent<AudioSource>();
        m_direction = Vector3.zero;
        m_fixUp = (transform.up * 4);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            GameManager.GetInstance().NewEnemy();
        }

        if (m_currentHP <= 0)
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

    private void OnDrawGizmosSelected()
    {
        if (m_playerControl == null || m_direction == Vector3.zero)
        {
            return;
        }
        Gizmos.color = Color.red;
        Gizmos.DrawLine(m_playerControl.transform.position + m_fixUp, 
            m_playerControl.transform.position + m_fixUp + m_direction * (grid.m_tileGridSize*1.7f));
        Gizmos.DrawWireSphere(m_playerControl.transform.position + m_fixUp, 5.0f);
    }

    private EnemyController IsEnemyInFront()
    {
        /*
        bool l_hit = Physics.SphereCast(m_playerControl.transform.position + m_fixUp,
            5.0f, m_playerControl.transform.position + m_fixUp + m_direction, 
            out RaycastHit l_hitInfo, (grid.m_tileGridSize * 1.7f), LayerMask.GetMask("Enemy"));
        if (l_hit)
        {
            return l_hitInfo.collider.GetComponent<EnemyController>();
        }
        */

        if (m_playerDamageArea.GetComponent<DamageAreaController>().m_enemyController != null)
        {
            return m_playerDamageArea.GetComponent<DamageAreaController>().m_enemyController;
        }

        return null;             
    }

    private void HandleRevive()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            m_audioSource.PlayOneShot(m_audioRevive);
            m_currentHP = m_maxHP;
            m_playerCharacter.GetComponent<Animator>().SetBool("Alive", true);
        }
    }

    private void HandleSpells()
    {
        if (Time.time - m_lastSpell < 1) // 1 seconds cooldown
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.H))
        { 
            m_lastSpell = Time.time;
            GameManager.GetInstance().SetSpell(GameManager.SpellLoaded.Attack);
            Debug.Log("Spell Attack Loaded");
        }
    }

    private void HandleCure()
    {
        if (Time.time - m_lastCure < 1) // 1 seconds cooldown
        {
            return;
        }
        if (Input.GetKeyDown(KeyCode.U))
        {
            m_audioSource.PlayOneShot(m_audioCure);
            m_lastCure = Time.time;
            m_currentHP += 50;
            if (m_currentHP > m_maxHP)
            {
                m_currentHP = m_maxHP;
            }

            Debug.Log("Player has " + m_currentHP + " HP");
        }
    }

    public void HandleHurt(float p_damage)
    {
        if (m_currentHP <= 0)
        {
            return;
        }
        if (!GameManager.GetInstance().m_inmortalPlayer) {
            m_currentHP -= p_damage;
            Debug.Log("Player has " + m_currentHP + " HP");
        }
        m_playerCharacter.transform.localPosition = Vector3.zero; 
        m_playerCharacter.GetComponent<Animator>().SetTrigger("Hit");
        if (m_currentHP <= 0)
        {
            m_audioSource.PlayOneShot(m_audioDeath);
            m_playerCharacter.GetComponent<Animator>().SetBool("Alive", false);
            GameManager.GetInstance().HandleGameOver();
        }
        else
        {
            m_audioSource.PlayOneShot(m_audioHurt);
            m_playerBlood.SetActive(true);
            StartCoroutine(WaitForBlood());
        }
    }

    private IEnumerator WaitForBlood()
    {
        yield return new WaitForSeconds(0.3f);
        m_playerBlood.SetActive(false);
    }

    private void HandleAttack()
    {
        if (Time.time - m_lastAttack < 1) // 1 seconds cooldown
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.LeftControl) ||
            Input.GetKeyDown(KeyCode.RightControl) )
        {

            m_lastAttack = Time.time;

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

        if (!grid.PlayerCanMove(m_playerControl, l_direction))  // Can't move only rotate
        {
            m_playerCharacter.transform.rotation = Quaternion.LookRotation(l_direction);

            return;
        }

        m_playerCharacter.GetComponent<Animator>().SetBool("Running",true);
        m_direction = l_direction;
        m_playerDamageArea.transform.position = m_playerCharacter.transform.position + l_direction * (grid.m_tileGridSize);
        m_isMoving = true;
        StartCoroutine(grid.Movement(m_playerControl, m_playerCharacter, l_direction, 0.0f));
    }
}

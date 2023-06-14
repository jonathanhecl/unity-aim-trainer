using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerController : MonoBehaviour
{
    private GridLogic grid;
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
            GameManager.GetInstance().CreateEnemy();
        }

        if (GameManager.GetInstance().GetPlayerHP() <= 0)
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

    private EnemyController IsEnemyInFront()
    {
        RaycastHit hit;
        if (Physics.Raycast(m_playerDamageArea.transform.position + m_fixUp, m_direction , out hit, 12))
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
            m_audioSource.PlayOneShot(m_audioRevive);
            GameManager.GetInstance().ResetPlayerHP();
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
            GameManager.GetInstance().SetSpell(GameManager.SpellLoaded.Attack);
        }
    }

    private void HandleCure()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            if (GameManager.GetInstance().UsePotion())
            {
                m_audioSource.PlayOneShot(m_audioCure);

                GameManager.GetInstance().HandlePlayerDamage(-50);

                CreateHealth();
            }
        }
    }

    private void CreateHealth()
    {
        var l_health = Instantiate(m_playerHealthEffect);
        l_health.transform.position = m_playerCharacter.transform.position + Vector3.up * 3;
    }

    public void HandleHurt(float p_damage)
    {
        if (GameManager.GetInstance().GetPlayerHP() <= 0)
        {
            return;
        }

        if (!GameManager.GetInstance().m_inmortalPlayer) {
            GameManager.GetInstance().HandlePlayerDamage(p_damage);
        }

        m_playerCharacter.transform.localPosition = Vector3.zero; 
        m_playerCharacter.GetComponent<Animator>().SetTrigger("Hit");
        if (GameManager.GetInstance().GetPlayerHP() <= 0)
        {
            m_audioSource.PlayOneShot(m_audioDeath);
            m_playerCharacter.GetComponent<Animator>().SetBool("Alive", false);
            GameManager.GetInstance().OnPlayerDie?.Invoke(name);
            GameManager.GetInstance().HandleGameOver();
        }
        else
        {
            m_audioSource.PlayOneShot(m_audioHurt);
        }

        CreateBlood();
    }

    private void CreateBlood()
    {
        var l_blood = Instantiate(m_playerBloodEffect);
        l_blood.transform.position = m_playerCharacter.transform.position + Vector3.up * 3;
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

            GameManager.GetInstance().OnPlayerAttack?.Invoke(name);

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

        GameManager.GetInstance().OnPlayerMove?.Invoke(name);

        m_playerCharacter.GetComponent<Animator>().SetBool("Running",true);
        m_direction = l_direction;
        m_playerDamageArea.transform.position = m_playerCharacter.transform.position + l_direction * (grid.m_tileGridSize);
        m_isMoving = true;
        StartCoroutine(grid.Movement(m_playerControl, m_playerCharacter, l_direction, 0.0f));
    }

}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyController : EnemyEntity
{
    private GridLogic grid;

    private EnemyData m_enemyData;

    private GameObject m_enemyControl;
    [SerializeField] private GameObject m_enemyBlood;
    [SerializeField] private GameObject m_enemyCharacter;

    private GameObject m_targetControl;
    private AudioSource m_audioSource;

    public void InitEnemy(EnemyData p_enemyData, GameObject p_target)
    {
        Debug.Assert(p_enemyData != null, "Enemy data is null");

        m_enemyData = p_enemyData;
        m_targetControl = p_target;

        grid = gameObject.AddComponent<GridLogic>();
        m_enemyControl = gameObject;
        m_enemyBlood.SetActive(false);

        m_enemyCharacter.transform.localPosition = Vector3.zero;
        m_audioSource = GetComponent<AudioSource>();

        m_maxHP = m_enemyData.maxHealth;
        m_currentHP = m_maxHP;

        m_enemyCharacter.transform.localScale = new Vector3(m_enemyData.size, m_enemyData.size, m_enemyData.size);

        m_isMoving = false;
    }

    void Update()
    {
        if (m_targetControl == null || m_currentHP <= 0 || m_isMoving)
        { 
            return;
        }

        MoveEnemy();
    }

    private void OnMouseDown()
    {
        if (m_currentHP <= 0)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            var l_spell = GameManager.GetInstance().UseSpell();
            if (l_spell == GameManager.SpellLoaded.Attack)
            {
                GameManager.GetInstance().SetSpell(GameManager.SpellLoaded.None);
                m_entropy++; // More entropy with spell attack
                HandleHurt(50.0f);
            }
        }
    }

    public void HandleHurt(float p_damage)
    {
        if (m_currentHP <= 0)
        {
            return;
        }

        m_currentHP -= p_damage;
        m_enemyCharacter.GetComponent<Animator>().SetTrigger("Hit");

        if (m_currentHP <= 0)
        {
            GameManager.GetInstance().AddScore(2);
            m_audioSource.PlayOneShot(m_enemyData.enemyBase.m_audioDeath);
            m_enemyCharacter.GetComponent<Animator>().SetBool("Alive", false);
            OnEnemyDie?.Invoke(name);
            // Create a new enemy
            GameManager.GetInstance().CreateEnemy();
        }
        else
        {
            GameManager.GetInstance().AddScore(1);
            m_audioSource.PlayOneShot(m_enemyData.enemyBase.m_audioHurt);
            m_entropy++;
        }

        m_enemyBlood.SetActive(true);
        StartCoroutine(WaitForBlood());
    }

    private IEnumerator WaitForBlood()
    {
        yield return new WaitForSeconds(0.3f);
        m_enemyBlood.SetActive(false);
    }

    private void AttackTarget()
    {
        if (GameManager.GetInstance().GetPlayerHP() <= 0)
        {
            return;
        }

        OnEnemyAttack?.Invoke(name);

        Vector3 l_difference = m_enemyControl.transform.localPosition - m_targetControl.transform.localPosition;
        Vector3 l_direction = Vector3.zero;

        l_difference = grid.PositionGridNormalize(l_difference);

        if (Math.Abs(l_difference.x) <= grid.m_tileGridSize && Math.Abs(l_difference.z) == 0)
        {
            if (l_difference.x < 0)
            {
                l_direction = transform.right;
            }
            else
            {
                l_direction = -transform.right;
            }
        }

        if (Math.Abs(l_difference.z) <= grid.m_tileGridSize && Math.Abs(l_difference.x) == 0)
        {
            if (l_difference.z < 0)
            {
                l_direction = transform.forward;
            }
            else
            {
                l_direction = -transform.forward;
            }
        }

        if (l_direction != Vector3.zero)
        {
            var l_originalRotation = m_enemyCharacter.transform.rotation;
            var l_nextRotation = Quaternion.LookRotation(l_direction);

            m_enemyCharacter.transform.rotation = Quaternion.Lerp(l_originalRotation, l_nextRotation, m_enemyData.speedDelay);
        } else
        {
            l_direction = -l_difference.normalized;

            m_isMoving = true;
            StartCoroutine(grid.Movement(m_enemyControl, m_enemyCharacter, l_direction, m_enemyData.speedDelay));
            return;
        }

        var l_target = m_targetControl.GetComponent<PlayerController>();

        if (l_target != null)
        {
            m_audioSource.PlayOneShot(m_enemyData.enemyBase.m_audioAttackHit);
            l_target.HandleHurt(m_enemyData.baseDamage);
        }

        l_direction = RandomMovement(-l_direction, l_direction);

        m_isMoving = true;
        StartCoroutine(grid.Movement(m_enemyControl, m_enemyCharacter, l_direction, m_enemyData.speedDelay));
    }

    private void MoveEnemy()
    {
        Vector3 l_difference = m_enemyControl.transform.localPosition - m_targetControl.transform.localPosition;
        Vector3 l_direction = new Vector3(0, 0, 0);

        l_difference = grid.PositionGridNormalize(l_difference);

        if (Math.Abs(l_difference.x) > Math.Abs(l_difference.z))
        {
            if (Math.Abs(l_difference.x) <= grid.m_tileGridSize &&
                !(Math.Abs(l_difference.x) < 1) &&
                Math.Abs(l_difference.z) == 0)
            {
                AttackTarget();
                return;
            }

            if (l_difference.x < 1)
            {
                l_direction = transform.right;
            } else 
            {
                l_direction = -transform.right;
            }
        } else 
        {
            if (Math.Abs(l_difference.z) <= grid.m_tileGridSize && 
                !(Math.Abs(l_difference.z) < 1) &&
                Math.Abs(l_difference.x) == 0)
            {
                AttackTarget();
                return;
            }

            if (l_difference.z < 1)
            {
                l_direction = transform.forward;
            } else 
            {
                l_direction = -transform.forward;
            }
        }

        OnEnemyMove?.Invoke(name);

        l_direction = RandomMovement(l_direction, -l_direction);

        m_isMoving = true;
        StartCoroutine(grid.Movement(m_enemyControl, m_enemyCharacter, l_direction, m_enemyData.speedDelay));
    }
}

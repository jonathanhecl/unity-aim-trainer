using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

public class EnemyController : MonoBehaviour
{
    private GridLogic grid = new GridLogic();
    private float m_entropy = 0.0f;
    private float m_distanceToMagic;

    [SerializeField] private float m_maxHP = 200.0f;
    [SerializeField] private float m_currentHP = 200.0f;

    [SerializeField] private GameObject m_enemyControl;
    [SerializeField] private GameObject m_enemyCharacter;
    [SerializeField] private GameObject m_enemyBlood;
    [SerializeField] private GameObject m_targetControl;
    [SerializeField] public bool m_isMoving = false;
    [SerializeField] private float m_speedMovementDelay = 0.2f;
    [SerializeField] private int m_tilesDistanceToMagic = 10;

    private void Start()
    {
        m_distanceToMagic = grid.m_tileGridSize * m_tilesDistanceToMagic;
        m_enemyCharacter.transform.localPosition = Vector3.zero;
    }

    void Update()
    {
        if (m_currentHP <= 0)
        {
            return;
        }

        if (m_isMoving)
        {
            return;
        }
        else
        {
           MoveEnemy();
        }
    }

    private void OnMouseDown()
    {
        if (Input.GetMouseButtonDown(0))
        {
            HandleHurt(70.0f);
        }
    }

    public void HandleHurt(float p_damage)
    {
        m_currentHP -= p_damage;
        if (m_currentHP <= 0)
        {
            m_enemyCharacter.GetComponent<Animator>().SetBool("Alive", false);
        }
        else
        {
            m_enemyBlood.SetActive(true);
            m_entropy++;
            StartCoroutine(WaitForBlood());
        }
    }

    private IEnumerator WaitForBlood()
    {
        yield return new WaitForSeconds(0.3f);

        m_enemyBlood.SetActive(false);
    }

    private void AttackTarget()
    {
        Vector3 l_difference = m_enemyControl.transform.localPosition - m_targetControl.transform.localPosition;
        Vector3 l_direction = new Vector3(0, 0, 0);

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

        var l_originalRotation = m_enemyCharacter.transform.rotation;
        var l_nextRotation = Quaternion.LookRotation(l_direction);

        m_enemyCharacter.transform.rotation = Quaternion.Lerp(l_originalRotation, l_nextRotation, m_speedMovementDelay);

        var l_target = m_targetControl.GetComponent<PlayerController>();
        l_target.HandleHurt(50);

        l_direction = RandomMovement(-l_direction, l_direction);

        m_isMoving = true;
        StartCoroutine(grid.Movement(m_enemyControl, m_enemyCharacter, l_direction, m_speedMovementDelay));
    }

    private Vector3 RandomMovement(Vector3 p_direction, Vector3 p_invalid)
    {
        var result = new Vector3();
        var num = UnityEngine.Random.Range(0.0f, 5.0f);

        if (num <= 3.5f && m_entropy == 0.0f)
        {
            result = p_direction;
        } else
        {
            num = UnityEngine.Random.Range(0.0f, 4.0f);

            switch (num)
            {
                case (> 3.0f):
                    result = transform.right;
                    break;
                case (> 2.0f):
                    result = transform.forward;
                    break;
                case (> 1.0f):
                    result = -transform.right;
                    break;
                default:
                    result = -transform.forward;
                    break;
            }
        }

        if (result == p_invalid)
        {
            result = p_direction;
        }

        if (m_entropy > 0.0f)
        {
            m_entropy--;
        }
       
        return result;
    }

    private void MoveEnemy()
    {
        Vector3 l_difference = m_enemyControl.transform.localPosition - m_targetControl.transform.localPosition;
        Vector3 l_direction = new Vector3(0, 0, 0);

        if (Math.Abs(l_difference.x) > Math.Abs(l_difference.z))
        {
            if (Math.Abs(l_difference.x) <= grid.m_tileGridSize && Math.Abs(l_difference.z) == 0)
            {
                AttackTarget();
                return;
            }

            if (l_difference.x < 0)
            {
                l_direction = transform.right;
            } else 
            {
                l_direction = -transform.right;
            }
        } else 
        {
            if (Math.Abs(l_difference.z) <= grid.m_tileGridSize && Math.Abs(l_difference.x) == 0)
            {
                AttackTarget();
                return;
            }

            if (l_difference.z < 0)
            {
                l_direction = transform.forward;
            } else 
            {
                l_direction = -transform.forward;
            }
        }

        l_direction = RandomMovement(l_direction, -l_direction);

        m_isMoving = true;
        StartCoroutine(grid.Movement(m_enemyControl, m_enemyCharacter, l_direction, m_speedMovementDelay));
    }
}

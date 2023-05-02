using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

public class EnemyController : MonoBehaviour
{
    private GridLogic grid = new GridLogic();
    private float m_distanceToMagic;

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
        HandleClick();

        if (m_isMoving)
        {
            return;
        }
        else
        {
            MoveEnemy();
        }
    }

    private void HandleClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit l_hit;
            Ray l_ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(l_ray, out l_hit))
            {
                HandleHurt();
            }
        }
    }

    public void HandleHurt()
    {
        m_enemyBlood.SetActive(true);
        StartCoroutine(WaitForBlood());
    }

    private IEnumerator WaitForBlood()
    {
        yield return new WaitForSeconds(0.5f);

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

        var target = m_targetControl.GetComponent<PlayerController>();
        target.HandleHurt();

        l_direction = RandomMovement(-l_direction, l_direction);

        m_isMoving = true;
        StartCoroutine(grid.Movement(m_enemyControl, m_enemyCharacter, l_direction, m_speedMovementDelay));
    }

    private Vector3 RandomMovement(Vector3 p_direction, Vector3 p_invalid)
    {
        var result = new Vector3();
        var num = UnityEngine.Random.Range(0.0f, 10.0f);

        Debug.Log(num);

        if (num <= 5.9f)
        {
            result = p_direction;
        } else
        {
            // calculate a random direction
            if (num>8.9f)
            {
                result = transform.right;
            } else if (num>7.9f)
            {
                result = transform.forward;
            } else if (num > 6.9f)
            { 
                result = -transform.right;
            } else { 
                result = -transform.forward;
            }
        }

        if (result == p_invalid)
        {
            result = p_direction;
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

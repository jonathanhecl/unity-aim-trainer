using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

public class EnemyController : MonoBehaviour
{
    private GridLogic grid = new GridLogic();

    [SerializeField] private GameObject m_enemyControl;
    [SerializeField] private GameObject m_enemyCharacter;
    [SerializeField] private GameObject m_targetControl;
    [SerializeField] public bool m_isMoving = false;
    [SerializeField] private float m_speedMovementDelay = 0.2f;
    private float m_distanceToFight;
    private Vector3 m_previousDirection;

    private void Start()
    {
        m_distanceToFight = grid.m_tileGridSize * 3;
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

    void HandleClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Clicked");

            RaycastHit l_hit;
            Ray l_ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(l_ray, out l_hit))
            {
                Debug.Log(l_hit.ToString());
            }
        }
    }   

    void MoveEnemy()
    {
        Vector3 l_difference = new Vector3(m_enemyCharacter.transform.localPosition.x - m_targetControl.transform.localPosition.x, 0, m_enemyCharacter.transform.localPosition.z - m_targetControl.transform.localPosition.z);
        Vector3 l_direction = new Vector3(0, 0, 0);

        //l_difference.Normalize();

        Debug.Log(l_difference  + " " +   m_distanceToFight);
        Debug.Log("Z " + (Math.Abs(l_difference.z) - m_distanceToFight));
        Debug.Log("X " + (Math.Abs(l_difference.x) - m_distanceToFight));


        if (l_direction == new Vector3(0, 0, 0))
        { 
            Debug.Log("está cerca");
            return;
        }

        if (l_direction == new Vector3(0,0,0)) // or invalid direction
        {
            return;
        }

        m_isMoving = true;

        m_previousDirection = l_direction;
        StartCoroutine(grid.Movement(m_enemyControl, m_enemyCharacter, l_direction, m_speedMovementDelay));
    }
}

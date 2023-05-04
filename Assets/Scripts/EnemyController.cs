using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

public class EnemyController : MonoBehaviour
{
    private GridLogic grid;
    private float m_entropy = 0.0f;
    //private float m_distanceToMagic;
    //private float m_paralysis = 0.0f;

    //[SerializeField] private float m_maxHP = 200.0f;
    [SerializeField] private float m_currentHP = 200.0f;

    [SerializeField] private GameObject m_enemyControl;
    [SerializeField] private GameObject m_enemyCharacter;
    [SerializeField] private GameObject m_enemyBlood;
    [SerializeField] public GameObject m_targetControl;
    [SerializeField] public bool m_isMoving = false;
    [SerializeField] private float m_speedMovementDelay = 0.2f;
    [SerializeField] private int m_tilesDistanceToMagic = 10;

    private AudioSource m_audioSource;

    [SerializeField] public AudioClip m_audioAttackHit;
    [SerializeField] public AudioClip m_audioHurt;
    [SerializeField] public AudioClip m_audioDeath;

    private void Start()
    {
        grid = gameObject.AddComponent<GridLogic>();
        //m_distanceToMagic = grid.m_tileGridSize * m_tilesDistanceToMagic;
        m_enemyCharacter.transform.localPosition = Vector3.zero;
        m_audioSource = GetComponent<AudioSource>();
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
        /*
        if (Input.GetMouseButtonDown(0))
        {
            // TODO: The sound from the spell if from the player, not the enemy
            HandleHurt(70.0f);
        }
        */
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
            m_audioSource.PlayOneShot(m_audioDeath);
            m_enemyCharacter.GetComponent<Animator>().SetBool("Alive", false);
        }
        else
        {
            m_audioSource.PlayOneShot(m_audioHurt);
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
        Vector3 l_direction = Vector3.zero;

        l_difference = grid.PositionGridNormalize(l_difference);

        if (Math.Abs(l_difference.x) < 1)
        {
            l_difference.x = 0;
        }

        if (Math.Abs(l_difference.z) < 1)
        {
            l_difference.z = 0;
        }

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

            m_enemyCharacter.transform.rotation = Quaternion.Lerp(l_originalRotation, l_nextRotation, m_speedMovementDelay);
        }

        var l_target = m_targetControl.GetComponent<PlayerController>();

        if (l_target != null)
        {
            m_audioSource.PlayOneShot(m_audioAttackHit);
            l_target.HandleHurt(50);
        }

        l_direction = RandomMovement(-l_direction, l_direction);

        Debug.Log("Attack->New direction: " + l_direction);

        m_isMoving = true;
        StartCoroutine(grid.Movement(m_enemyControl, m_enemyCharacter, l_direction, m_speedMovementDelay));
    }

    private Vector3 RandomMovement(Vector3 p_direction, Vector3 p_invalid)
    {
        return p_direction;

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

        l_difference = grid.PositionGridNormalize(l_difference);

        if (Math.Abs(l_difference.x) < 1)
        {
            l_difference.x = 0;
        }

        if (Math.Abs(l_difference.z) < 1)
        {
            l_difference.z = 0;
        }

        Debug.Log("Enemy diff" + l_difference);

        if (Math.Abs(l_difference.x) > Math.Abs(l_difference.z))
        {
            if (Math.Abs(l_difference.x) <= grid.m_tileGridSize &&
                !(Math.Abs(l_difference.x) < 1) &&
                Math.Abs(l_difference.z) == 0)
            {
                Debug.Log("Xabs: " + Math.Abs(l_difference.x));
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
                Debug.Log("Zabs: " + Math.Abs(l_difference.z));
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

        Debug.Log("New direction : " + l_direction);

        l_direction = RandomMovement(l_direction, -l_direction);

        m_isMoving = true;
        StartCoroutine(grid.Movement(m_enemyControl, m_enemyCharacter, l_direction, m_speedMovementDelay));
    }
}

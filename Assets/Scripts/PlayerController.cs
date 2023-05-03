using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private GridLogic grid = new GridLogic();

    [SerializeField] private float m_maxHP = 200.0f;
    [SerializeField] private float m_currentHP = 200.0f;

    [SerializeField] private GameObject m_playerControl;
    [SerializeField] private GameObject m_playerCharacter;
    [SerializeField] private GameObject m_playerBlood;
    [SerializeField] public bool m_isMoving = false;

    private float m_lastCure;
    private float m_lastAttack;

    private void Start()
    {
        m_playerCharacter.transform.localPosition = Vector3.zero;
    }

    void Update()
    {
        if (m_currentHP <= 0)
        {
            HandleRevive();
            return;
        }

        HandleCure();
        HandleAttack();
        if (m_isMoving) {
            return;
        } else
        {
            HandleMovement();
        }
    }

    private void HandleRevive()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            m_currentHP = m_maxHP;
            m_playerCharacter.GetComponent<Animator>().SetBool("Alive", true);
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
            m_lastCure = Time.time;
            m_currentHP += 50;
            if (m_currentHP > m_maxHP)
            {
                m_currentHP = m_maxHP;
            }
        }
    }

    public void HandleHurt(float p_damage)
    {
        //m_currentHP -= p_damage;
        m_playerCharacter.GetComponent<Animator>().SetTrigger("Hit");
        if (m_currentHP <= 0)
        {
            m_playerCharacter.GetComponent<Animator>().SetBool("Alive", false);
        }
        else
        {
            m_playerBlood.SetActive(true);
            StartCoroutine(WaitForBlood());
        }
    }

    private IEnumerator WaitForBlood()
    {
        yield return new WaitForSeconds(0.3f);
        m_playerBlood.SetActive(false);
    }

    private EnemyController IsEnemyIn(Vector3 p_direction)
    {
        var l_target = GameObject.FindGameObjectWithTag("Enemy").GetComponent<EnemyController>();
        if (l_target != null)
        {

            Vector3 l_difference = l_target.transform.position - m_playerControl.transform.position;
            float l_distance = l_difference.magnitude;
            Quaternion l_looking = m_playerCharacter.transform.localRotation.normalized;

            if (l_distance < 14)
            {
                // is front the player?

                bool l_hit = Physics.Raycast(m_playerControl.transform.position, l_looking * p_direction, out RaycastHit l_hitInfo, 14.0f, LayerMask.GetMask("Enemy"));
                if (!l_hit)
                {
                    return null;
                }

                Debug.Log(l_hitInfo.collider.name);

                Debug.Log("target" + l_distance + l_looking);

                return l_target;
            }
        }

        return null;
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

            var l_target = IsEnemyIn(Vector3.forward);
            if (l_target != null)
            { 
                l_target.HandleHurt(50);
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

        if (IsEnemyIn(l_direction))
        {
            return; // Can't move
        }

        m_playerCharacter.GetComponent<Animator>().SetBool("Running",true);
        m_isMoving = true;
        StartCoroutine(grid.Movement(m_playerControl, m_playerCharacter, l_direction, 0.0f));
    }
}

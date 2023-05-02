using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private GridLogic grid = new GridLogic();

    [SerializeField] private GameObject m_playerControl;
    [SerializeField] private GameObject m_playerCharacter;
    [SerializeField] private GameObject m_playerBlood;
    [SerializeField] public bool m_isMoving = false;

    private void Start()
    {
        m_playerCharacter.transform.localPosition = Vector3.zero;
    }

    void Update()
    {
        HandleAttack();
        if (m_isMoving) {
            return;
        } else
        {
            HandleMovement();
        }
    }

    public void HandleHurt()
    {
        m_playerBlood.SetActive(true);

        // wait
        StartCoroutine(WaitForBlood());
    }

    private IEnumerator WaitForBlood()
    {
        yield return new WaitForSeconds(0.5f);

        m_playerBlood.SetActive(false);
    }

    private void HandleAttack()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl) ||
            Input.GetKeyDown(KeyCode.RightControl) )
        {
            //var target = m_ene.GetComponent<EnemyController>();
            //target.HandleHurt();
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

        m_playerCharacter.GetComponent<Animator>().SetTrigger("Running");
        m_isMoving = true;
        StartCoroutine(grid.Movement(m_playerControl, m_playerCharacter, l_direction, 0.0f));
    }
}

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private GameObject m_player;
    [SerializeField] public bool m_isMoving = false;
    private GridLogic grid = new GridLogic();

    void Update()
    {
        if (m_isMoving) {
            return;
        } else
        {
            HandleKeyboard();
        }
    }

    private void HandleKeyboard()
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

        m_isMoving = true;
        StartCoroutine(grid.Movement(m_player, l_direction, 0.0f));
    }
}

using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

public class GridLogic : MonoBehaviour
{
    public readonly float m_timeToMove = 0.2f;
    public readonly float m_tileGridSize = 10.0f;

    public IEnumerator Movement(GameObject p_element, Vector3 p_direction, float p_extraDelay)
    {
        float l_elapsedTime = 0;

        var l_originalPosition = p_element.transform.localPosition;
        var l_nextPosition = l_originalPosition + (p_direction * m_tileGridSize);

        while (l_elapsedTime < (m_timeToMove + p_extraDelay))
        {
            p_element.transform.localPosition = Vector3.Lerp(l_originalPosition, l_nextPosition, (l_elapsedTime / (m_timeToMove+ p_extraDelay)));
            l_elapsedTime += Time.deltaTime;
            yield return null;
        }

        p_element.transform.localPosition = l_nextPosition;

        UpdateStateMovement(p_element, false);
    }

    private void UpdateStateMovement(GameObject p_element, bool p_isMoving)
    {
        if (p_element.TryGetComponent<PlayerController>(out var l_player))
        {
            l_player.m_isMoving = p_isMoving;
        }

        if (p_element.TryGetComponent<EnemyController>(out var l_enemy))
        {
            l_enemy.m_isMoving = p_isMoving;
        }
    }
}

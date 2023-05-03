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
    public readonly float m_limitPlayer = 80.0f;

    public bool PlayerCanMove(GameObject p_playerControl, Vector3 p_direction)
    {
        var l_originalPosition = p_playerControl.transform.localPosition;
        var l_nextPosition = l_originalPosition + (p_direction * m_tileGridSize);

        if (l_nextPosition.x <= -m_limitPlayer || l_nextPosition.x >= m_limitPlayer ||
            l_nextPosition.z <= -m_limitPlayer || l_nextPosition.z >= m_limitPlayer)
        {
            return false;
        }

        return true;
    }


    public IEnumerator Movement(GameObject p_control, GameObject p_character, Vector3 p_direction, float p_extraDelay)
    {
        float l_elapsedTime = 0;

        // control movement
        var l_originalControlPosition = p_control.transform.localPosition;
        var l_nextControlPosition = l_originalControlPosition + (p_direction * m_tileGridSize);

        // character rotation
        var l_originalRotation = p_character.transform.rotation;
        var l_nextRotation = Quaternion.LookRotation(p_direction);        

        while (l_elapsedTime < (m_timeToMove + p_extraDelay))
        {
            // Control
            p_control.transform.localPosition = Vector3.Lerp(l_originalControlPosition, l_nextControlPosition, (l_elapsedTime / (m_timeToMove + p_extraDelay)));

            // Character
            p_character.transform.rotation = Quaternion.Lerp(l_originalRotation, l_nextRotation, (l_elapsedTime / (m_timeToMove + p_extraDelay)));
            l_elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Final position
        p_control.transform.localPosition = l_nextControlPosition;
        p_character.transform.localPosition = Vector3.zero;

        UpdateControlStateMovement(p_control, p_character, false);
    }

    private void UpdateControlStateMovement(GameObject p_element, GameObject p_character, bool p_isMoving)
    {
        if (p_element.TryGetComponent<PlayerController>(out var l_player))
        {
            l_player.m_isMoving = p_isMoving;
            p_character.GetComponent<Animator>().SetBool("Running", p_isMoving);
        }

        if (p_element.TryGetComponent<EnemyController>(out var l_enemy))
        {
            l_enemy.m_isMoving = p_isMoving;
        }
    }
}

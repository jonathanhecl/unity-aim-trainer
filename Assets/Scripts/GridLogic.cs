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

    public IEnumerator Movement(GameObject p_control, GameObject p_character, Vector3 p_direction, float p_extraDelay)
    {
        float l_elapsedTime = 0;

        // control movement
        var l_originalControlPosition = p_control.transform.localPosition;
        var l_nextControlPosition = l_originalControlPosition + (p_direction * m_tileGridSize);

        // character movement
        var l_originalPosition = p_character.transform.position;
        var l_nextPosition = l_originalPosition + (p_direction * m_tileGridSize);

        // character rotation
        var l_originalRotation = p_character.transform.rotation;
        var l_nextRotation = Quaternion.LookRotation(p_direction);        

        while (l_elapsedTime < (m_timeToMove + p_extraDelay))
        {
            // Control
            p_control.transform.localPosition = Vector3.Lerp(l_originalControlPosition, l_nextControlPosition, (l_elapsedTime / (m_timeToMove + p_extraDelay)));

            // Character
            p_character.transform.position = Vector3.Lerp(l_originalPosition, l_nextPosition, (l_elapsedTime / (m_timeToMove+ p_extraDelay)));
            p_character.transform.rotation = Quaternion.Lerp(l_originalRotation, l_nextRotation, (l_elapsedTime / (m_timeToMove + p_extraDelay)));
            l_elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Final position
        p_character.transform.position = l_nextPosition;
        p_control.transform.localPosition = l_nextControlPosition;

        UpdateControlStateMovement(p_control, false);
    }

    private void UpdateControlStateMovement(GameObject p_element, bool p_isMoving)
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

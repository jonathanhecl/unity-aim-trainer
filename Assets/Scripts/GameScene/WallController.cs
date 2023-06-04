using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallController : MonoBehaviour
{
    private bool m_isPlayerNear = false;

    void Update()
    {
        IsThePlayerNear();
    }

    private void IsThePlayerNear()
    {
        var l_player = GameManager.GetInstance().GetPlayerControl().m_playerCharacter;
        var l_distance = Vector3.Distance(l_player.transform.position, transform.position);

        if (l_distance < 100f)
        {
            if (m_isPlayerNear == true)
            {
                return;
            }
            //var l_material = GetComponent<Renderer>().material;
            //l_material.color = Color.red;
            //l_material.EnableKeyword("_EMISSION");
            m_isPlayerNear = true; ;
        } else
        {
            if (m_isPlayerNear == false)
            {
                return;
            }
            // var l_material = GetComponent<Renderer>().material;
            //l_material.color = Color.black;
            //l_material.DisableKeyword("_EMISSION");
            m_isPlayerNear = false;
        }
    }

}

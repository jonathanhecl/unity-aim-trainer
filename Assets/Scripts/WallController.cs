using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallController : MonoBehaviour
{
    void Update()
    {
        Debug.Log("Update");
        IsThePlayerNear();
    }

    private void IsThePlayerNear()
    {
        var l_player = GameManager.GetInstance().GetPlayerControl().m_playerCharacter;
        var l_distance = Vector3.Distance(l_player.transform.position, transform.position);

        Debug.Log("Distance" + l_distance);

        if (l_distance < 5f)
        {
            this.gameObject.SetActive(true);
        } else
        {
            this.gameObject.SetActive(false);
        }
    }
}

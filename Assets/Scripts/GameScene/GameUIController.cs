using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUIController : MonoBehaviour
{
    [SerializeField] private GameObject m_cursor;

    void Update()
    {
        if (GameManager.GetInstance().GetPlayerControl().SpellLoaded() != SpellType.None)
        {
            m_cursor.SetActive(true);
            m_cursor.transform.position = Input.mousePosition + new Vector3(30, 0, 0);
        }
        else
        {
            m_cursor.SetActive(false);
        }
    }
}

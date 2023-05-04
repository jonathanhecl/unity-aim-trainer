using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageAreaController : MonoBehaviour
{
    public EnemyController m_enemyController = null;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            m_enemyController = other.gameObject.GetComponent<EnemyController>();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            m_enemyController = null;
        }
    }
}

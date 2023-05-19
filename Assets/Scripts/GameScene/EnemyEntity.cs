using UnityEngine;

public class EnemyEntity : MonoBehaviour
{
    protected float m_maxHP;
    protected float m_currentHP;

    protected float m_entropy = 0.0f;

    public bool m_isMoving;

    protected Vector3 RandomMovement(Vector3 p_direction, Vector3 p_invalid)
    {
        Vector3 result;

        var num = UnityEngine.Random.Range(0.0f, 5.0f);

        if (num <= 3.5f && m_entropy == 0.0f)
        {
            result = p_direction;
        }
        else
        {
            num = UnityEngine.Random.Range(0.0f, 4.0f);

            switch (num)
            {
                case (> 3.0f):
                    result = transform.right;
                    break;
                case (> 2.0f):
                    result = transform.forward;
                    break;
                case (> 1.0f):
                    result = -transform.right;
                    break;
                default:
                    result = -transform.forward;
                    break;
            }
        }

        if (result == p_invalid)
        {
            result = p_direction;
        }

        if (m_entropy > 0.0f)
        {
            m_entropy--;
        }

        return result;
    }
}

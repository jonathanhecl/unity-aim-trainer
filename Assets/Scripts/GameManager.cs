using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private PlayerController m_playerControl;
    [SerializeField] private GameObject m_enemyPrefab;

    public bool m_inmortalPlayer = false;
    private SpellLoaded m_spellLoaded = 0;
    
    public enum SpellLoaded { 
        None, 
        Attack, 
        Cure, 
        Paralysis };

    private static GameManager m_instance;

    private void Awake()
    {
        if (m_instance == null)
        {
            m_instance = this;
        } else
        {
            Destroy(gameObject);
        }
    }

    public static GameManager GetInstance()
    {
        return m_instance;
    }

    public void SetSpell(SpellLoaded spellType)
    {
        m_spellLoaded = spellType;
    }

    public SpellLoaded UseSpell()
    {
        var l_prevSpell = m_spellLoaded;

        if (m_spellLoaded == SpellLoaded.Attack)
        {
            m_playerControl.m_playerCharacter.GetComponent<Animator>().SetTrigger("MagicAttack");
            m_playerControl.m_audioSource.PlayOneShot(m_playerControl.m_audioSpellAttack);
        }

        m_spellLoaded = SpellLoaded.None;

        return l_prevSpell;
    }

    public void NewEnemy()
    {
        var obj = Instantiate(m_enemyPrefab, new Vector3(-130.0f, 0.0f, 0.0f), Quaternion.identity);

        EnemyController enemyController = obj.GetComponent<EnemyController>();
        enemyController.m_targetControl = m_playerControl.gameObject;
    }

    public void HandleGameOver()
    {
        Debug.Log("Game Over");
    }
}

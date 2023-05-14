using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private PlayerController m_playerControl;
    [SerializeField] private GameObject m_enemyPrefab;
    [SerializeField] private Transform[] m_respawnPosition = new Transform[4];
    [SerializeField] private List<GameObject> m_enemies = new List<GameObject>();

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

    private void Start()
    {
        CreateEnemy();
    }

    public static GameManager GetInstance()
    {
        return m_instance;
    }

    public PlayerController GetPlayerControl()
    {
        return m_playerControl;
    }

    public void SetSpell(SpellLoaded spellType)
    {
        m_spellLoaded = spellType;
    }

    public SpellLoaded UseSpell()
    {
        var l_prevSpell = m_spellLoaded;

        switch (m_spellLoaded)
        {
            case SpellLoaded.Attack:
                m_playerControl.m_playerCharacter.GetComponent<Animator>().SetTrigger("MagicAttack");
                m_playerControl.m_audioSource.PlayOneShot(m_playerControl.m_audioSpellAttack);
                break;
            case SpellLoaded.Cure:
                //m_playerControl.m_audioSource.PlayOneShot(m_playerControl.m_audioSpellCure);
                break;
            case SpellLoaded.Paralysis:
                //m_playerControl.m_audioSource.PlayOneShot(m_playerControl.m_audioSpellParalysis);
                break;
        }

        m_spellLoaded = SpellLoaded.None;

        return l_prevSpell;
    }

    public void CreateEnemy()
    {
        var l_respawnPosition = m_respawnPosition[Random.Range(0, m_respawnPosition.Length)];

        var newEnemy = Instantiate(m_enemyPrefab, l_respawnPosition.position, Quaternion.identity);

        m_enemies.Add(newEnemy);

        EnemyController enemyController = newEnemy.GetComponent<EnemyController>();
        enemyController.m_targetControl = m_playerControl.gameObject;
    }

    public void HandleGameOver()
    {
        Debug.Log("Game Over");
    }
}

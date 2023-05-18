using UnityEngine;

[CreateAssetMenu(menuName = "Data/EnemyBaseData")]
public class EnemyBaseData : ScriptableObject
{
    // Audio
    public AudioClip m_audioAttackHit;
    public AudioClip m_audioHurt;
    public AudioClip m_audioDeath;
}

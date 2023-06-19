using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public struct SpellInfo
{
    public SpellLoaded m_spellType;
    public string m_name;
    public float m_time;
    public float m_interval;
    public Image m_image;
}
public enum SpellLoaded
{
    None,
    Attack,
    Cure,
    Paralysis
};

public class Spells : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

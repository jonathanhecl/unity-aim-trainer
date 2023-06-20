using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorController : MonoBehaviour
{
    private void OnMouseDown()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var playerControl = GameManager.GetInstance().GetPlayerControl();
            var l_spell = playerControl.UseSpell();
            if (l_spell != SpellType.None)
            {
                Debug.Log("Spell missed");
            }
        }
    }
}

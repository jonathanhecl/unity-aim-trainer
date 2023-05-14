using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorController : MonoBehaviour
{
    private void OnMouseDown()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (GameManager.GetInstance().UseSpell() != GameManager.SpellLoaded.None)
            {
                Debug.Log("Spell missed");
            }
        }
    }
}

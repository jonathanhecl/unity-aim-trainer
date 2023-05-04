using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorController : MonoBehaviour
{
    private void OnMouseDown()
    {
        if (Input.GetMouseButtonDown(0))
        {
            GameManager.GetInstance().UseSpell(); // Miss
            Debug.Log("Spell missed");
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KorYmeLibrary.SaveSystem;

public class TestSave : MonoBehaviour
{
    private int _deathCount;
    private Vector3 _position;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            _deathCount++;
            Debug.Log("DeathCount : " + _deathCount);
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            _position = new Vector3(0, 0, _position.z + 1);
            Debug.Log("Position : " + _position);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrandeThrower : MonoBehaviour
{
    [SerializeField] PlayerController player;

    void Start()
    {
        
    }


    public void EquipGun()
    {
        player.SwitchToWeapon();
    }

}

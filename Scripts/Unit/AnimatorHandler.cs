using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorHandler : MonoBehaviour
{
    [SerializeField] internal Unit unit;


    public void SpawnObject(int ID)
    {
        SpawnManager.instance.SpawnObject(ID, transform.position, 9);
    }

    public void LaunchForceForward(float force)
    {
        unit.rb.velocity += new Vector3(force * unit.direction, 0, 0);
    }
    public void LaunchForceUp(float force)
    {
        unit.rb.velocity += new Vector3(0,force,0);
    }

    public void EndAttackAnim()
    {
        unit.attackAnim = 0;
    }


    public void ActivateAbility(int id)
    {
        transform.parent.parent.Find("Skills").GetChild(id).GetComponent<Skill>().CheckAbility();
    }

    public void DeactivateAbility(int id)
    {
        unit.skills[id].DeactivateAbility();
    }
}

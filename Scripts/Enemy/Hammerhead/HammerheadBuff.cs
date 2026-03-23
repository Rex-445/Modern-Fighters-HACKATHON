using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HammerheadBuff : MonoBehaviour
{
    public float throwForce;
    public float throwDamage;

    public Hitbox targetHitbox;

    Unit unit;

    private void Start()
    {
        unit = transform.parent.parent.GetComponent<Unit>();
    }

    public void Drag(string dragName)
    {
        targetHitbox.owner.toGrabAnim = dragName;
    }

    public void Release()
    {
        targetHitbox.owner.toGrabAnim = "";
    }


    public void Throw()
    {
        targetHitbox.Throw(throwForce, throwDamage);
        targetHitbox.owner.toGrabAnim = "";
    }

    /// <summary>
    /// Everytime Hammerhead uses an ability his armor count is refershed
    /// </summary>
    public void ResetArmorCount()
    {
        unit.hitArmorCount = unit.maxHitArmorCount;
    }
}

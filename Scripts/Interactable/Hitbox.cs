using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Hitbox : MonoBehaviour
{
    [Header("Combat")]
    public float damage;
    public int stunCount;
    public float critChance = 50f;

    [Header("Physics")]
    public float force;
    public float forceUp;

    [SerializeField] internal Unit owner;

    public enum DamageType { hit, blood, lightning , armor, fire}

    public enum HitboxAction { normal, grab }
    public HitboxAction hitAction = HitboxAction.normal;
    public Unit grabTarget;

    public DamageType damageType = DamageType.hit;

    public UnityEvent OnCollision;

    public float timer = .5f;
    float maxTime;

    private void Start()
    {
        maxTime = timer;
    }

    private void Update()
    {
        //If the Hitbox detects a collision then it should start the time for a combo chain
        try
        {
            if (owner.hasHit)
            {
                //Decrement
                if (timer > 0)
                    timer -= Time.deltaTime;

                //Once the limit is reached then reset everything
                if (timer < 0)
                {
                    timer = maxTime;
                    if (owner.isPlayer)
                        owner.anim.SetInteger("Attack Chain", 0);
                    owner.hasHit = false;
                }
            }
        } catch { }


        //Grabbing
        if (grabTarget != null)
        {
            grabTarget.transform.position = transform.position;
            grabTarget.transform.rotation = transform.rotation;

            if (owner.toGrabAnim == null)
            {
                owner.unitState = Unit.UnitState.none;
                grabTarget.unitState = Unit.UnitState.none;
                grabTarget = null;
            }
        }
    }


    [System.Obsolete]
    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Unit")
        {
            //Check if this is not a stray hitbox 
            //That is this hitbox has an owner
            if (owner != null)
            {

                //If this hit box does not hit its owner
                if (other.gameObject != owner.gameObject)
                {
                    if (other.GetComponent<Unit>().team != owner.team|| other.GetComponent<Unit>().isNone)
                    {
                        Unit enemy = other.GetComponent<Unit>();
                        //If this hitbox is not a grabbing type then simply deal damage
                        if (hitAction == HitboxAction.normal)
                        {
                            //IF there is already a grabbed target and that target is not the enemy you are carrying at the moment
                            if (grabTarget != null)
                            {
                                if (grabTarget != enemy)
                                    DealDamage(enemy);
                            }
                            //IF there is no grabbed target 
                            else
                                DealDamage(enemy);
                        }

                        //If this hitbox is a grabbing type then set the unit grabbed to be grabbed by this hitbox
                        if (hitAction == HitboxAction.grab)
                        {
                            //IF there is not a grab target then grab
                            if (grabTarget == null)
                                Grab(enemy);
                        }


                        //If the owner is not in skill then add resource to the resource bar
                        if (!owner.inSkill)
                        {
                            //Add Resource on hit
                            int random = Random.Range(1, 3);
                            owner.resourceValue += random;
                        }
                    }
                }
            }

            //Check if this is not a stray hitbox 
            //That is this hitbox does not have an owner
            if (owner == null)
            {
                float newDamage = damage;
                bool crit = IsCrit();


                //Double the Damage if this is a crit
                if (crit)
                    newDamage *= 2;

                //Proceed
                other.GetComponent<Unit>().TakeDamage(newDamage, (int)damageType, force, this.gameObject, forceUp, crit);
                timer = .5f;
                OnCollision.Invoke();
            }
        }
    }

    [System.Obsolete]
    private void DealDamage(Unit enemy)
    {
        float newDamage = damage;
        bool crit = IsCrit();

        try
        {
            GameObject skills = transform.parent.parent.Find("Skills").gameObject;
            Skill[] skillList = skills.GetComponentsInChildren<Skill>();
            foreach (Skill sl in skillList)
            {
                if (sl.inUse)
                    newDamage += sl.abilityDamage;
            }
        }
        catch { }


        //Double the Damage if this is a crit
        if (crit)
            newDamage *= 2;

        //Proceed
        enemy.TakeDamage(newDamage, (int)damageType, force, owner.gameObject, forceUp, crit);
        owner.hasHit = true; ;
        timer = .5f;
        OnCollision.Invoke();
    }

    public void Grab(Unit target)
    {
        if (grabTarget != null)
            return;

        //Check if the target is alive and grounded
        if (target.health <= 0 || target.anim.GetCurrentAnimatorStateInfo(0).IsName("GetUp"))
            return;

        //Check if the target is in an invulnerable skill state
        if (target.invincibilitySkill)
            return;

        grabTarget = target;
        grabTarget.unitState = Unit.UnitState.grabbed;
        owner.unitState = Unit.UnitState.grabbing;
    }

    public void Throw(float force, float throwDamage)
    {
        owner.unitState = Unit.UnitState.none;

        if (grabTarget != null)
        {
            grabTarget.TakeDamage(damage + throwDamage, (int)damageType, force, owner.gameObject, forceUp, true);
            grabTarget.unitState = Unit.UnitState.none;
            grabTarget = null;
        }
        else
        {

        }
    }


    [System.Obsolete]
    bool IsCrit()
    {
        //Always Crit if the Owner is in skill
        if (owner != null)
            if (owner.inSkill)
                return true;


        float crit = Random.RandomRange(0, 100);
        if (crit <= critChance)
            return true;

        return false;
    }
}

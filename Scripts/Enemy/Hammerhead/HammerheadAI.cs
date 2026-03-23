using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HammerheadAI : MonoBehaviour
{

    public float scale = 2;


    public Vector3 point;

    internal Unit target;

    [Header("AI")]
    public float waitToMove;
    internal float moveDelay;

    //This is to check if PonyTail has move to the required destination
    bool hasMovedToPoint = false;

    [Header("Combat")]
    public float attackTime;
    internal float maxAttackTime;
    public float attackWaitTime;
    internal float maxAttackWaitTime;
    public float moveToPlayerWaitTime;
    float moveWaitTime;

    //Fighting
    public bool canAttackPlayer;
    public float combatTime;
    internal float maxCombatTime;

    [Header("Ability")]
    public float abilityFreq;
    internal float maxAbilityFreq;


    Unit unit;

    void Start()
    {
        unit = GetComponent<Unit>();


        maxAbilityFreq = abilityFreq;
        maxAttackWaitTime = attackWaitTime;
        maxAttackTime = attackTime;
        maxCombatTime = combatTime;

        moveDelay = waitToMove;
    }

    // Update is called once per frame
    [System.Obsolete]
    void Update()
    {

        //If the target is null then ask the UnitManager for the Player
        if (target == null)
        {
            target = UnitManager.instance.player;
            // If the Target is STILL null then there is no player so do nothing
            return;
        }


        //If PonyTail or the Player is dead then do nothing
        if (target.health <= 0 || unit.health <= 0)
        {
            unit.attack = false;
            unit.horizontal = 0;
            unit.vertical = 0;
            return;
        }

        //Check if the player is alive and/or healthy
        bool healthyPlayer = target.health > 0;


        //IF there are no active garrage units and the player is not dead then "canAttackPlayer" should be true
        canAttackPlayer = healthyPlayer && combatTime > 0;


        //Necessary Engage and Disengage variables
        float distanceX = Mathf.Abs(transform.position.x - target.transform.position.x);
        bool knockedOut = target.anim.GetCurrentAnimatorStateInfo(0).IsName("GetUp") && target.anim.GetCurrentAnimatorStateInfo(0).IsName("Death");

        if (canAttackPlayer)
        {
            //If the player is knocked out then give them some space
            if (knockedOut)
            {
                if (distanceX < unit.attackRange + 2)
                    Evade();
                else
                {
                    unit.horizontal = 0;
                    unit.vertical = 0;
                }
            }

            //If the player is up and active then engage
            if (!knockedOut)
                EngagePlayer();

        }

        //If you can't attack the player then avoid them instead with some conditions
        if (!canAttackPlayer)
        {
            //Before Just "Evading" check if the player is too close to this unit.
            //If the player IS, then don't just "ignore" and keep roaming but rather fight the player for as long as...


            //..A: The Player is within a certain range and is not knocked out

            //If the "certain" range is '7' and the "engage" range is '5'
            if (distanceX < 4 && distanceX > 2)
            {
                waitToMove -= Time.deltaTime;
                if (waitToMove < 0)
                {
                    Evade();
                }
            }

            if (distanceX > 4)
            {
                waitToMove = moveDelay;
                unit.horizontal = 0;
                unit.vertical = 0;
            }


            //..B: The player is close but not "just too close" but "just" out of range
            //If this unit is still close to the player 
            if (distanceX < unit.attackRange + 1)
            {
                //If the player is not on the floor knocked out
                //Then engage
                if (!knockedOut)
                    EngagePlayer();

                //IF the player has been knocked out then give them some space
                if (knockedOut)
                    Evade();
                else
                {
                    unit.horizontal = 0;
                    unit.vertical = 0;
                }
            }
        }

        combatTime -= Time.deltaTime;
        abilityFreq -= Time.deltaTime;

        //Combat Time (think of it like a "rest" time for Hammerhead)
        if (combatTime <= -10)
            combatTime = maxCombatTime;

        //Combat Ability
        if (abilityFreq <= 0 && distanceX <= unit.attackRange)
        {
            abilityFreq = maxAbilityFreq;
            //If this ability can be activated then go for it else just wait until PonyTail is available
            if (unit.skills[0].CanActivate())
            {
                unit.ActivateSkill(0);
            }
        }
    }


    [System.Obsolete]
    private void EngagePlayer()
    {
        //If the target is out of range then calculate when to move to player
        if (Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(target.transform.position.x, target.transform.position.z)) > unit.attackRange)
        {
            unit.attack = false;
            moveWaitTime -= Time.deltaTime;

            if (moveWaitTime <= 0)
            {
                MoveToPlayer(target.transform.position);
            }
        }

        //Else Attack
        else
        {
            Vector3 newPoint = target.transform.position;
            float xDistance = Mathf.Sqrt(Mathf.Pow(transform.position.x - target.transform.position.x, 2));

            //Check if the unit is too close to its target
            if (xDistance < unit.attackRange * .8f)
            {
                moveWaitTime -= Time.deltaTime;

                if (moveWaitTime <= 0)
                {
                    //If So then focus on moving out of range instead
                    if (transform.position.x < newPoint.x)
                    {
                        unit.horizontal = -1;
                    }

                    if (transform.position.x > newPoint.x)
                    {
                        unit.horizontal = 1;
                    }
                }
            }

            //If the target is within range then calculate when to attack the player
            else
            {
                unit.horizontal = 0;
                unit.vertical = 0;
                moveWaitTime = moveToPlayerWaitTime;

                attackWaitTime -= Time.deltaTime;
                if (attackWaitTime <= 0)
                {
                    LookAtTarget();

                    //Piledriver
                    if (abilityFreq <= 0)
                    {
                        unit.ActivateSkill(1);
                        abilityFreq = maxAbilityFreq;
                    }

                    //Decrement attack
                    // If it is less than zero then attack for as long at it is supposed to
                    attackTime -= Time.deltaTime;
                    unit.attack = true;

                    //Then reset attack
                    if (attackTime <= 0)
                    {
                        unit.attack = false;
                        attackWaitTime = maxAttackWaitTime;
                        attackTime = maxAttackTime;
                    }
                }
            }
        }
    }


    private void Evade()
    {
        unit.attack = false;

        if (hasMovedToPoint == false)
        {
            MoveToPoint();
        }

        if (hasMovedToPoint == true)
        {
            waitToMove -= Time.deltaTime;
            if (waitToMove <= 0)
            {
                waitToMove = moveDelay;
                hasMovedToPoint = false;

                //First Check Which Side PonyTail is to Karrin/Player
                //If PonyTail is on the left side then she only roams on the left and the same for the right

                //Step One: Get the difference between both positions
                float x = target.transform.position.x;

                //Step Two: Check for the result
                // If less the 0: Left Side
                //Stay on the Left side
                if (transform.position.x < target.transform.position.x)
                {
                    x -= scale;
                }

                // If Greater then 0: Right Side
                //Stay on the Right side
                if (transform.position.x > target.transform.position.x)
                {
                    x += scale;
                }

                float z = target.transform.position.z + Random.Range(-2, 2);

                if (z > 6)
                    z = 5.5f;
                if (z < -3)
                    z = -2.5f;

                if (x < -8f)
                    x = -7;
                if (x > GameManager.instance.bound.transform.position.x)
                    x = Camera.main.transform.parent.GetComponent<CameraMovement>().right.transform.position.x + Camera.main.orthographicSize;

                // Then Roam as intended
                point = new Vector3(x, transform.position.y, z);
            }
        }
    }

    void MoveToPlayer(Vector3 newPoint)
    {
        //Moveing on the X Axis
        float distance = Mathf.Sqrt(Mathf.Pow(transform.position.x - target.transform.position.x, 2));

        //Check if the unity is out of attack range on the X Axis (Only)
        if (distance > unit.attackRange)
        {
            if (transform.position.x < newPoint.x)
            {
                unit.horizontal = 1;
            }

            if (transform.position.x > newPoint.x)
            {
                unit.horizontal = -1;
            }

        }

        float zDistance = Mathf.Abs(transform.position.z - target.transform.position.z);
        if (zDistance > .3f)
        {
            //Moveing on the X Axis
            if (transform.position.z < target.transform.position.z)
            {
                unit.vertical = 1;
            }

            if (transform.position.z > target.transform.position.z)
            {
                unit.vertical = -1;
            }
        }
        else
        {
            unit.vertical = 0;
        }
        LookAtTarget();
    }


    public void MoveToPoint()
    {
        float xDistance = Mathf.Abs(transform.position.x - point.x);
        float zDistance = Mathf.Abs(transform.position.z - point.z);
        if (xDistance > 1)
        {
            //Moveing on the X Axis
            if (transform.position.x < point.x)
            {
                unit.horizontal = 1;
            }

            if (transform.position.x > point.x)
            {
                unit.horizontal = -1;
            }

            unit.ChangeDirection((int)unit.horizontal);
        }
        else
        {
            unit.horizontal = 0;
        }


        if (zDistance > 2)
        {
            //Moving On the Z Axis
            if (transform.position.z < point.z)
            {
                unit.vertical = 1;
            }

            if (transform.position.z > point.z)
            {
                unit.vertical = -1;
            }
        }
        else

        {
            unit.vertical = 0;
        }


        if (xDistance < 2 && zDistance < 2)
        {
            hasMovedToPoint = true;
            unit.horizontal = 0;
            unit.vertical = 0;
        }

        //Focus the target if in alert range
        if (Mathf.Abs(transform.position.x - target.transform.position.x) <= 5)
            LookAtTarget();
    }


    void LookAtTarget()
    {
        //IF there is no target to look at then do nothing
        if (target == null)
            return;

        if (target.transform.position.x < transform.position.x)
        {
            unit.direction = -1;
        }

        if (target.transform.position.x > transform.position.x)
        {
            unit.direction = 1;
        }
    }

}

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

[System.Serializable]
public class ElectrisLevel
{
    //Electric Thrust
    /// <summary>
    ///  This is a Melee ability that is used to compensate for Electris's lack of close combat prowers
    ///  It's a simple thrust forward of electric energy that does damage in a line (decent damage and low cooldown)
    /// </summary>
    public float electricThrustFreq;
    internal float maxElectricThrustFreq;


    //Electric Rail
    /// <summary>
    ///  This is a seeking lightning bolt that teleports to its owner's target on spawn
    /// </summary>
    public float electricRailFreq;
    internal float maxElectricRailFreq;

    //Electric Burst
    /// <summary>
    ///  This is an AOE ability that creates a burst of Electric energy around Electris
    ///  It's used to upgrade her abilites damage overall and increace the effect of her next cast of "Electric Burst" (that is it just increase the AOE range)
    /// </summary>
    public float electricBurstFreq;
    internal float maxElectricBurstFreq;

    //Electric Storm
    /// <summary>
    ///  This is a wide range ability that does the same as "Electric Burst" but better
    ///  In addition to the burst of energy Electris calls down multiple lightning rails that strikes the ground (some lighting's spawns can Hone in on it's target)
    /// </summary>
    public float electricStormFreq;
    internal float maxElectricStormFreq;

    //Electric Rail
    /// <summary>
    ///  This is a stance where Electris swaps from "Melee" attacks (like basic attack and other melee abilities) to ranged attacks
    ///  When Electris enteres this stance the cooldown's on all her "..Freq" are reduced by x2 making it possible for her to cast (ranged) abilities more often
    /// </summary>
    public float evadeTime;
    internal float maxEvadeTime;
}


public class ElectrisAI : MonoBehaviour
{
    public float scale;

    public Vector3 point;

    public Unit target;

    [Header("AI")]
    public float waitToMove;
    public float moveDelay;
    bool hasMovedToPoint;

    //Level Up Variables
    [Header("Level Up")]
    public float[] levelUpHealthConditions;

    [Header("Combat")]
    public float attackTime;
    internal float maxAttackTime;
    public float attackWaitTime;
    internal float maxAttackWaitTime;
    public float moveToPlayerWaitTime;
    float moveWaitTime;

    public bool canAttackPlayer;

    [Header("Ability")]
    public List<ElectrisLevel> electrisAbilityLevel;

    [SerializeField]
    internal int levelIndex = -1;

    Unit unit;

    private void Start()
    {
        maxAttackWaitTime = attackWaitTime;
        maxAttackTime = attackTime;
        attackTime = 0;

        //Assign Ability Level Values
        AssignAbilityValues();



        //Health
        unit = GetComponent<Unit>();


        GameObject[] objects = GameObject.FindGameObjectsWithTag("Unit");
        foreach (GameObject go in objects)
        {
            if (go.GetComponent<Unit>().team != unit.team)
                target = go.GetComponent<Unit>();
        }


        float x = Random.Range(-3, 11);
        float z = Random.Range(-4, 9);

        if (z > 2.5f)
            z = 2.5f;
        if (z < -2)
            z = -2f;

        if (x < -8f)
            x = -7;
        if (x > Camera.main.transform.parent.GetComponent<CameraMovement>().right.transform.position.x)
            x = Camera.main.transform.parent.GetComponent<CameraMovement>().right.transform.position.x + Camera.main.orthographicSize;

        point = new Vector3(x, 0, z);

    }

    private void AssignAbilityValues()
    {
        levelIndex++;

        if (levelIndex >= electrisAbilityLevel.Count)
            return;

        //Electric Thrust
        electrisAbilityLevel[levelIndex].maxElectricThrustFreq = electrisAbilityLevel[levelIndex].electricThrustFreq;

        //Electric Rail
        electrisAbilityLevel[levelIndex].maxElectricRailFreq = electrisAbilityLevel[levelIndex].electricRailFreq;

        //Electris Burst
        electrisAbilityLevel[levelIndex].maxElectricBurstFreq = electrisAbilityLevel[levelIndex].electricBurstFreq;

        //Electric Storm
        electrisAbilityLevel[levelIndex].maxElectricStormFreq = electrisAbilityLevel[levelIndex].electricStormFreq;

        //Evade Time
        electrisAbilityLevel[levelIndex].maxEvadeTime = electrisAbilityLevel[levelIndex].evadeTime;

    }


    [System.Obsolete]
    // Update is called once per frame
    void Update()
    {
        if (target == null || unit.inSkill)
        {
            return;
        }


        //If the Player or Electris is dead then do nothing
        if (target.health <= 0 || unit.health <= 0)
        {
            unit.attack = false;
            unit.horizontal = 0;
            unit.vertical = 0;
            return;
        }

        UpdateAbilities();

        Movement();
        //UpdateLevelUpCounter();
    }

    private void Movement()
    {
        //IF there are no active garrage units and the player is not dead then "canAttackPlayer" should be true
        bool knockedOut = target.anim.GetCurrentAnimatorStateInfo(0).IsName("GetUp");


        //Necessary Engage and Disengage variables
        float distanceX = Mathf.Abs(transform.position.x - target.transform.position.x);


        //You can only attack the player if they are not knocked down to the ground
        canAttackPlayer = electrisAbilityLevel[levelIndex].evadeTime < 0;

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

            //If the "certain" range is '4'
            if (distanceX < 5 && distanceX > 2)
            {
                waitToMove -= Time.deltaTime;
                if (waitToMove <= 0)
                {
                    Evade();
                }
            }

            //Electric Rail
            electrisAbilityLevel[levelIndex].electricRailFreq -= Time.deltaTime;

            if (electrisAbilityLevel[levelIndex].electricRailFreq < 0)
            {
                if (unit.skills[3].CanActivate())
                    ElectricRail();
            }

            //If you are in a good distance to the player then use ranged abilities
            if (distanceX > 5)
            {
                LongRangeEngage();
            }


            //..B: The player is close but not "just too close" but "just" out of range
            //If this unit is still close to the player 
            if (distanceX < 2)
            {
                //If the player is not on the floor knocked out
                //Then engage
                if (!knockedOut)
                    EngagePlayer();

                //IF the player has been knocked out then give them some space
                if (knockedOut)
                    Evade();
            }
        }
    }

    void UpdateLevelUpCounter()
    {
        float perc = (int)(unit.health / unit.maxHealth);

        if (perc <= levelUpHealthConditions[2])
        {
            levelIndex = 2;
        }

        else if (perc <= levelUpHealthConditions[1])
        {
            levelIndex = 1;
        }

        else if (perc <= levelUpHealthConditions[0])
        {
            levelIndex = 0;
        }
    }


    private void LongRangeEngage()
    {
        //Electric Storm
        electrisAbilityLevel[levelIndex].electricStormFreq -= Time.deltaTime;

        //Electric Rail
        electrisAbilityLevel[levelIndex].electricRailFreq -= Time.deltaTime;

        //Electric Storm
        if (electrisAbilityLevel[levelIndex].electricStormFreq < 0)
        {
            //Activate Ability
            ElectricStorm();
        }

        //Engage the player because they have moved too far away
        EngagePlayer();
        waitToMove = moveDelay;
    }

    private void UpdateAbilities()
    {
        ///// Electric Thrust ////
        electrisAbilityLevel[levelIndex].electricThrustFreq -= Time.deltaTime;


        //// Electric Burst ////
        electrisAbilityLevel[levelIndex].electricBurstFreq -= Time.deltaTime;
        if (electrisAbilityLevel[levelIndex].electricBurstFreq < 0)
        {
            if (unit.skills[1].CanActivate())
                //Activate Ability
                ElectricBurst();
        }



        //Electric Storm
        electrisAbilityLevel[levelIndex].electricStormFreq -= Time.deltaTime;
        if (electrisAbilityLevel[levelIndex].electricStormFreq < 0)
        {
            //Activate Ability
            ElectricStorm();
        }

        //Evade Time
        electrisAbilityLevel[levelIndex].evadeTime -= Time.deltaTime;

        //If the evade time is less than -5 then go back to melee ranged attacks
        if (electrisAbilityLevel[levelIndex].evadeTime < -5)
            electrisAbilityLevel[levelIndex].evadeTime = electrisAbilityLevel[levelIndex].maxEvadeTime;

    }

    private void ElectricThrust()
    {
        unit.ActivateSkill(0);

        //Reset Timer based on the "Level Index"
        electrisAbilityLevel[levelIndex].electricThrustFreq = electrisAbilityLevel[levelIndex].maxElectricThrustFreq;
    }

    private void ElectricBurst()
    {
        unit.ActivateSkill(1);
        //Reset Timer based on the "Level Index"
        electrisAbilityLevel[levelIndex].electricBurstFreq = electrisAbilityLevel[levelIndex].maxElectricBurstFreq;
    }

    private void ElectricStorm()
    {
        unit.ActivateSkill(2);

        //Reset Timer based on the "Level Index"
        electrisAbilityLevel[levelIndex].electricStormFreq = electrisAbilityLevel[levelIndex].maxElectricStormFreq;
    }

    private void ElectricRail()
    {
        unit.ActivateSkill(3);

        //Reset Timer based on the "Level Index"
        electrisAbilityLevel[levelIndex].electricRailFreq = electrisAbilityLevel[levelIndex].maxElectricRailFreq;
    }


    private void EngagePlayer()
    {

        //If the target is out of range then calculate when to move to player
        if (Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(target.transform.position.x, target.transform.position.z)) > unit.attackRange)
        {
            moveWaitTime -= Time.deltaTime;

            if (moveWaitTime <= 0)
            {
                MoveToPlayer(target.transform.position);
            }

            electrisAbilityLevel[levelIndex].electricStormFreq -= Time.deltaTime;

            //Electric Storm
            if (electrisAbilityLevel[levelIndex].electricStormFreq < 0)
            {
                //If the player Z position is far from Electris
                if (Mathf.Abs(transform.position.z - target.transform.position.z) > 1.5f)
                    //Activate Ability
                    ElectricStorm();
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

                    /*IDEA
                    //EGO: Ego is the threat level of the unit that is being targeted by "this"
                    //Below are ego variables that add to 
                    int ego = 0;

                    bool lowHealth = ((unit.health / unit.maxHealth) * 100) < .45f;*/

                    //Check to activate "Electric Thrust"
                    if (electrisAbilityLevel[levelIndex].electricThrustFreq < 0)
                    {
                        //Activate Ability
                        ElectricThrust();
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

                if (z > 5)
                    z = 4.7f;
                if (z < -1)
                    z = -1f;

                if (x < -8f)
                    x = -7;
                if (x > GameManager.instance.bound.transform.position.x)
                    x = Camera.main.transform.parent.GetComponent<CameraMovement>().right.transform.position.x + Camera.main.orthographicSize;

                // Then Roam as intended
                point = new Vector3(x, transform.position.y, z);
            }
        }
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

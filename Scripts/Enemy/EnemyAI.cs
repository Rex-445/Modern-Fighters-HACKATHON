using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Cameras;

public class EnemyAI : MonoBehaviour
{
    public float scale = 2;


    public Vector3 point;

    public Unit target;

    [Header("AI")]
    public float waitToMove;
    public float moveDelay;
    bool hasMoved = false;

    [Header("Combat")]
    public float attackTime;
    internal float maxAttackTime;
    public float attackWaitTime;
    internal float maxAttackWaitTime;
    public float moveToPlayerWaitTime;
    float moveWaitTime;

    public bool canAttackPlayer;

    [Header("UI")]
    public SpriteRenderer alertSprite;
    public Sprite alert;
    public Sprite roaming;
    public DataManager dataManager;

    [Header("Call For Backup")]
    public GameObject[] backups;
    public float healthDiff = 50;
    public bool canCallBackup;

    [Header("Ability")]
    public float abilityFreq;
    internal float maxAbilityFreq;
    public AudioClip[] backupClips;
    public Animator backupAnimator;

    Vector3 newPoint;


    Unit unit;
    //Right
    internal float leftToRightExtention = 1.5f;
    //LEft
    internal float rightToLeftExtention = 1.3f;

    private void Start()
    {
        maxAttackWaitTime = attackWaitTime;
        maxAttackTime = attackTime;
        attackTime = 0;
        maxAbilityFreq = abilityFreq;
        unit = GetComponent<Unit>();
        dataManager = GameObject.FindGameObjectWithTag("UIManager").GetComponent<DataManager>();

        GameObject[] objects = GameObject.FindGameObjectsWithTag("Unit");
        foreach (GameObject go in objects)
        {
            if (go.GetComponent<Unit>().team != unit.team)
                target = go.GetComponent<Unit>();
        }


        try
        {
            StartCoroutine(StalledRequestAgro());
        }
        catch { }



        float x = Random.Range(-3, 11);
        float z = Random.Range(-4, 9);

        if (z > 9)
            z = 8.5f;
        if (z < -4)
            z = -3.5f;

        if (x < -8f)
            x = -7;
        if (x > Camera.main.transform.parent.GetComponent<CameraMovement>().right.transform.position.x)
            x = Camera.main.transform.parent.GetComponent<CameraMovement>().right.transform.position.x + Camera.main.orthographicSize;

        point = new Vector3(x, 0, z);

    }


    [System.Obsolete]
    // Update is called once per frame
    void Update()
    {
        if (target == null)
        {
            return;
        }

        //LookAtPlayer();
        if (target.health <= 0 || unit.health <= 0)
        {
            unit.attack = false;
            unit.horizontal = 0;
            unit.vertical = 0;
            return;
        }

        abilityFreq -= Time.deltaTime * 2;

        unit.attack = attackTime > 0;
        attackTime -= Time.deltaTime * 2;
        alertSprite.transform.localScale = transform.localScale;

        //Engage the Player
        if (canAttackPlayer)
        {
            if (Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(target.transform.position.x, target.transform.position.z)) < unit.attackRange)
                return;

            if (unit.anim.GetCurrentAnimatorStateInfo(0).IsName("Idle") || unit.anim.GetCurrentAnimatorStateInfo(0).IsName("Walk"))
                MoveToPlayer();
        }

        //Move Around and get busy if this enemy can't attack the player
        if (!canAttackPlayer)
        {
            alertSprite.sprite = roaming;
            unit.attack = false;
            if (hasMoved == false)
            {
                MoveToPoint();
            }

            if (hasMoved == true)
            {
                waitToMove -= Time.deltaTime * 2;
                if (waitToMove <= 0)
                {
                    waitToMove = moveDelay;
                    hasMoved = false;
                    float x = target.transform.position.x;
                    if (transform.position.x < target.transform.position.x)
                    {
                        x -= scale;
                    }

                    //Stay on the Right hand side
                    if (transform.position.x > target.transform.position.x)
                    {
                        x += scale;
                    }

                    float z = target.transform.position.z + Random.Range(-4, 9);

                    if (z > 9)
                        z = 8.5f;
                    if (z < -4)
                        z = -3.5f;

                    if (x < -8f)
                        x = -7;
                    if (x > Camera.main.transform.parent.GetComponent<CameraMovement>().right.transform.position.x)
                        x = Camera.main.transform.parent.GetComponent<CameraMovement>().right.transform.position.x + Camera.main.orthographicSize;

                    point = new Vector3(x, transform.position.y, z);
                }
            }
        }
    }

    public void MoveToPlayer()
    {
        alertSprite.sprite = alert;
        newPoint = target.GetComponent<EnemyAIHandler>().enemyPointLeft;
        Vector3 targetOffset = target.GetComponent<EnemyAIHandler>().offset;
        bool foundPoint = false;

        //If the Enemy is closer to the right than the left
        bool foundRightPoint = Vector3.Distance(transform.position, target.GetComponent<EnemyAIHandler>().enemyPointRight) < Vector3.Distance(transform.position, target.GetComponent<EnemyAIHandler>().enemyPointLeft);

        if (foundRightPoint == false)
        {
            //Left Side
            // If the target's Enemy AI Handler's Left is less than 2 (max amount) and it does not contain this enemy
            // Then Move towards that point
            if (target.GetComponent<EnemyAIHandler>().enemyLeft.Count < 2 && !target.GetComponent<EnemyAIHandler>().enemyLeft.Contains(this.gameObject))
            {
                foundPoint = true;
                if (unit.hDirection == 1 || unit.hDirection == 0)
                {
                    newPoint = target.transform.position - new Vector3(targetOffset.x - rightToLeftExtention, 0, targetOffset.z);
                }
                if (unit.hDirection == -1)
                {
                    newPoint = target.transform.position - new Vector3(targetOffset.x + rightToLeftExtention, 0, targetOffset.z);
                }
            }

            //If the EnemyAI is Contained
            if (target.GetComponent<EnemyAIHandler>().enemyLeft.Contains(this.gameObject))
            {
                foundPoint = true;
                if (unit.hDirection == 1 || unit.hDirection == 0)
                {
                    newPoint = target.transform.position - new Vector3(targetOffset.x - rightToLeftExtention, 0, targetOffset.z);
                }
                if (unit.hDirection == -1)
                {
                    newPoint = target.transform.position - new Vector3(targetOffset.x + rightToLeftExtention, 0, targetOffset.z);
                }
            }
        }



        //Right Side
        //If Not Contained
        if (!foundPoint && target.GetComponent<EnemyAIHandler>().enemyRight.Count < 2 && !target.GetComponent<EnemyAIHandler>().enemyRight.Contains(this.gameObject))
        {
            if (unit.hDirection == 1)
            {
                newPoint = target.transform.position + new Vector3(targetOffset.x + leftToRightExtention, 0, targetOffset.z);
            }
            if (unit.hDirection == -1 || unit.hDirection == 0)
            {
                newPoint = target.transform.position + new Vector3(targetOffset.x, 0, targetOffset.z);
            }
        }

        //If Contained
        if (!foundPoint && target.GetComponent<EnemyAIHandler>().enemyRight.Contains(this.gameObject))
        {
            if (unit.hDirection == 1)
            {
                newPoint = target.transform.position + new Vector3(targetOffset.x + leftToRightExtention, 0, targetOffset.z);
            }
            if (unit.hDirection == -1 || unit.hDirection == 0)
            {
                newPoint = target.transform.position;
            }
        }

        //If the target is out of range then calculate when to move to player
        if (Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(newPoint.x, newPoint.z)) > unit.attackRange)
        {
            moveWaitTime -= Time.deltaTime * 2;

            if (moveWaitTime <= 0)
            {
                MoveToPlayer(newPoint);
            }
        }
    }

    private void LateUpdate()
    {
        //If the target is within range then calculate when to attack the player
        if (Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(newPoint.x, newPoint.z)) < unit.attackRange)
        {
            LookAtPlayer();
            unit.horizontal = 0;
            unit.vertical = 0;
            moveWaitTime = moveToPlayerWaitTime;

            attackWaitTime -= Time.deltaTime * 2;
            if (attackWaitTime <= 0)
            {
                attackWaitTime = maxAttackWaitTime;
                attackTime = maxAttackTime;
            }
        }
    }

    IEnumerator StalledRequestAgro()
    {
        yield return new WaitForEndOfFrame();
        RequestAgro();
    }

    void MoveToPlayer(Vector3 newPoint)
    {
        float xDistance = Mathf.Abs(transform.position.x - newPoint.x);
        //Moveing on the X Axis
        if (transform.position.x < newPoint.x)
        {
            unit.horizontal = 1;
        }

        if (transform.position.x > newPoint.x)
        {
            unit.horizontal = -1;
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

    public void RequestAgro()
    {
        UnitManager.instance.EnemyAgroRequest(this.gameObject);
        //print("Making Agro Request");
    }

    public void GetHit()
    {
        if (abilityFreq <= 0)
        {
            if (dataManager.difficulty == "Hard" || dataManager.difficulty == "Rough" || unit.isBoss)
            {
                float perc = (unit.health / unit.maxHealth) * 100;
                if (perc < healthDiff)
                {
                    if (canCallBackup)
                    {
                        CallForBackUp();
                        abilityFreq = maxAbilityFreq;
                    }
                }
            }
        }
    }

    void CallForBackUp()
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag("Unit");
        List<EnemyAI> units = new List<EnemyAI>();

        //Check for Allies
        foreach (GameObject go in objects)
        {
            Unit newUnit = go.GetComponent<Unit>();
            if (newUnit.GetComponent<Unit>().team == unit.team)
            {
                if (newUnit.gameObject != this.gameObject)
                    units.Add(newUnit.GetComponent<EnemyAI>());
            }
        }

        //Create an Enemy then
        if (units.Count == 0)
        {
            //Call for an actual Backup
            //Add to thee Wave Manager Script
            int rand = Random.Range(0, backups.Length);
            GameObject newAlly = Instantiate(backups[rand], transform.position + Vector3.left * 3, Quaternion.identity) as GameObject;
            newAlly.transform.parent = transform.parent;
            unit.OnUseAbility.Invoke();
            transform.parent.GetComponent<WaveManager>().enemies.Add(newAlly);

            //Reset the hit armor count to suprise the player and stop Combos
            //You can also use this to increase pressure as it is supposed to be considering its on 'Hard' or 'Rough' Mode
            unit.hitArmorCount = unit.maxHitArmorCount;

            //Play the required sound and animation necessary
            rand = Random.Range(0, backupClips.Length);
            backupAnimator.Play("Whistle", 0, 0);
            GetComponent<AudioSource>().clip = backupClips[rand];
            GetComponent<AudioSource>().Play();
            print("No Viable ally, now creating one");

            //Stop any further functions
            return;
        }

        //Assign Most health to youself
        EnemyAI targetAlly = UnitManager.instance.enemiesAttackingPlayer[0].GetComponent<EnemyAI>();
        float oldHealth = unit.health;

        //Look for an enemy that is idle
        foreach (EnemyAI targetUnit in units)
        {
            if (targetUnit.canAttackPlayer)
            {
                targetAlly = targetUnit;
            }
        }

        if (targetAlly.gameObject != this.gameObject)
        {
            //Get the UnitManager to request agro for the targeted ally
            UnitManager.instance.EnemyAgroRequest(targetAlly.gameObject);

            //Play the required animation necessary
            backupAnimator.Play("Whistle", 0, 0);
            unit.hitArmorCount = unit.maxHitArmorCount;
            unit.OnUseAbility.Invoke();

            //If this unit is a boss DO NOT remove agro as it would look and feel very weird and less of like a boss battle
            if (unit.isBoss == false)
            {
                UnitManager.instance.RemoveAgro(this.gameObject);
                canAttackPlayer = false;
            }

            //Play the necessary sound clip
            int rand = Random.Range(0, backupClips.Length);
            GetComponent<AudioSource>().clip = backupClips[rand];
            GetComponent<AudioSource>().Play();
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
            LookAtTarget();
            hasMoved = true;
            unit.horizontal = 0;
            unit.vertical = 0;
        }
    }
    void LookAtTarget()
    {
        if (newPoint.x < transform.position.x)
        {
            unit.direction = -1;
        }

        if (newPoint.x > transform.position.x)
        {
            unit.direction = 1;
        }
    }

    void LookAtPlayer()
    {
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

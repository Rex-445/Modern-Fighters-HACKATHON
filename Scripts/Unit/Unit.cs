using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(HitEffector))]
public class Unit : MonoBehaviour
{
    public enum MovementState
    {
        idle,
        walk,
        air,
        fall,
        attack,
        stunned,
        death,
    }

    public enum UnitState
    {
        none,
        fire,
        poison,
        bleed,
        heal,
        grabbing,
        grabbed
    }

    public UnitState unitState = UnitState.none;

    //Grabbing
    [Tooltip("This value is to check if the unit is grabbed or in any grabbing state")]
    public string toGrabAnim = "";

    [Header("Other Variables")]
    //State
    public string unitName;
    public float health;
    internal float maxHealth;
    public float resourceValue;
    internal float maxResourcevalue;
    public int direction = 1;
    public int hDirection = 1;
    public int vDirection = 1;

    public string team = "Independent";
    public bool isPlayer = false;
    public bool isNone = false;
    public bool isBoss = false;
    public MovementState state = MovementState.idle;

    //Graphics 
    public Sprite image;
    public Color theme;
    public Sprite attackIcon;
    public float deathTime = 5f;

    [SerializeField] internal float moveSpeed = 5f;
    [SerializeField] private float jumpHeight = 5f;

    internal float maxMoveSpeed;

    [Header("Combat")]
    public float attackRange = 1.3f;
    public float hitArmorCount = 2;
    internal float maxHitArmorCount = 0;
    public float hitArmorTime = 2;
    internal float maxHitArmorTime;
    public float attackSpeed = 1;
    public bool hasHit;
    internal bool inSkill = false;
    internal bool invincibilitySkill = false;
    public int unitLevel = 1;

    public GameObject bleedObject;


    [Header("Physics")]
    //Physics
    internal Rigidbody rb;
    public LayerMask groundLayer;
    private bool bounce = true;

    [Header("Events")]
    public UnityEvent OnDeath;
    public UnityEvent OnTakeDamage;
    public UnityEvent OnUseAbility;

    //Graphics
    [Header("Graphics")]
    //Stun
    public Slider healthBarPrefab;
    public float stunCount = 5;
    internal float maxStunCount;
    internal Slider stunBar;
    internal bool stunned;

    //Health
    [Header("Health and Stats")]
    internal bool alive = true;
    internal Slider healthBar;
    internal Slider armorBar;
    internal float healValue;
    internal float baseHealth;
    internal float baseDamage;
    internal float healSpeed = .08f;
    internal bool ishealing;
    internal float showTime = 0;
    private SpriteRenderer sprite;


    [Header("Animations")]
    internal Animator anim;
    public int attackAnim;
    float attackCoolDown;

    [Header("Skills")]
    public List<Skill> skills;

    //Actions
    [SerializeField] internal float horizontal;
    [SerializeField] internal float vertical;
    internal bool attack;
    internal bool kick;
    internal bool crouch;

    HitEffector hitEffector;

    [Header("Spawnables")]
    [SerializeField] private GameObject parryHitVFX;
    [SerializeField] private GameObject damagePopup;
    [SerializeField] private GameObject expPopup;
    string[] nullStates;


    internal bool hitInAir;
    internal bool isHit;

    // Start is called before the first frame update
    void Start()
    {
        //Get Bleed Object
        try
        {
            if (bleedObject == null)
            {
                bleedObject = transform.Find("Container").Find("Sprite").Find("BleedPoint").gameObject;
            }
        }
        catch { }

        if (unitName == "")
            unitName = this.name;

        //Set Health Values
        baseDamage = transform.Find("Container").Find("Sprite").Find("Hitbox").GetComponent<Hitbox>().damage;
        baseHealth = health;
        for (int i = 1; i < unitLevel; i++)
        {
            health += (int)(baseHealth * .05f);
        }

        //Set Damage Values
        for (int i = 1; i < unitLevel; i++)
        {
            transform.Find("Container").Find("Sprite").Find("Hitbox").GetComponent<Hitbox>().damage += (int)(baseDamage * .1f);
        }

        //Mana
        maxResourcevalue = resourceValue;

        //Stun
        maxStunCount = stunCount;
        maxHitArmorCount = hitArmorCount;
        maxHitArmorTime = hitArmorTime;


        //Move Speed
        maxMoveSpeed = moveSpeed;
        if (moveSpeed < 5)
            maxMoveSpeed += 3;

        //UI
        healthBar = Instantiate(healthBarPrefab, Vector3.zero, Quaternion.identity, UIManager.instance.transform.Find("DisableUI").Find("HealthBars").transform);
        armorBar = healthBar.transform.Find("ArmorBar").GetComponent<Slider>();
        armorBar.maxValue = hitArmorCount;
        healthBar.maxValue = health;
        maxHealth = health;
        healthBar.transform.Find("Name").GetComponent<Text>().text = unitName;

        //For the Hearo Alone
        if (isPlayer)
        {
            //Main Health Bar UI
            UIManager.instance.mainHealthBar.maxValue = maxHealth;
            UIManager.instance.mainHealthBar.value = maxHealth;
        }


        //Initilize Null States
        nullStates = new string[13];
        nullStates[0] = "Hit";
        nullStates[1] = "Land";
        nullStates[2] = "Parry";
        nullStates[3] = "Stunned";
        nullStates[4] = "Attack1";
        nullStates[5] = "Attack2";
        nullStates[6] = "Attack3";
        nullStates[7] = "Fall";
        nullStates[8] = "Lying";

        hitEffector = GetComponent<HitEffector>();
        rb = GetComponent<Rigidbody>();
        if (sprite == null)
            sprite = transform.Find("Container").Find("Sprite").GetComponent<SpriteRenderer>();
        if (anim == null)
            anim = transform.Find("Container").Find("Sprite").GetComponent<Animator>();

        GameObject.FindGameObjectWithTag("UnitManager").GetComponent<UnitManager>().AddUnit(this);
    }

    public void SpawnUnitIndicator(GameObject enemyIndicator)
    {
        GameObject go = Instantiate(enemyIndicator, UIManager.instance.transform);
        go.GetComponent<UnitIndicator>().target = transform;
    }

    // Update is called once per frame
    private void Update()
    {
        transform.Find("shadow").transform.position = new Vector3(transform.position.x, -1, transform.position.z);

        //If the unit is dead then don't display the bleed object
        if (health <= 0)
            bleedObject.SetActive(false);


        if (TimeManager.paused)
            return;

        if (IsGrounded() && health <= 0 || anim.GetCurrentAnimatorStateInfo(0).IsName("Idle") && health <= 0 || transform.position.y < -15)
            Death();

        //UpdateMana
        UpdateStats();

        if (stunned)
        {
            if (health > 0 && IsGrounded() && anim.GetCurrentAnimatorStateInfo(0).IsName("Stunned") == false)
                anim.Play("Stunned");

            return;
        }



        //Randomoize the attack Pattern
        if (attack && hasHit)
        {
            attackCoolDown -= Time.deltaTime;
            if (attackCoolDown <= 0)
            {
                attackCoolDown = .5f;
                int random = Random.Range(1, attackAnim + 1);
                anim.SetInteger("Attack Chain", random);
            }
        }

        //Level Up
        if (health > maxHealth)
            maxHealth = health;
    }


    private void LateUpdate()
    {
        if (TimeManager.paused)
            return;


        UpdateUI();

        if (IsGrounded() && health <= 0 || anim.GetCurrentAnimatorStateInfo(0).IsName("Idle") && health <= 0)
            Death();

        if (stunned)
        {
            if (health > 0 && IsGrounded() && anim.GetCurrentAnimatorStateInfo(0).IsName("Stunned") == false)
                anim.Play("Stunned");

            return;
        }

        //IF this unit is being grabbed then play a grab animation
        //Right now I don't have a grabbed anim so I'll just use the "Fall" anim for all characters
        if (unitState == UnitState.grabbed)
        {
            anim.Play("Fall");
            return;
        }

        //IF the unit is grabbing and they are not playing the "toGrabAnim" animation
        if (unitState == UnitState.grabbing)
        {
            if (toGrabAnim != null)
                if (!anim.GetCurrentAnimatorStateInfo(0).IsName(toGrabAnim))
                anim.Play(toGrabAnim);
            else
                unitState = UnitState.none;
        }

        Movement();
    }

    void UpdateStats()
    {
        if (health <= 0)
            return;

        //If the Health is less than 45% then low health should be true
        bool lowHealth = health < (maxHealth * .45f);
        if (bleedObject != null)
        {
            bleedObject.SetActive(lowHealth);

            //If not playing the Idle or Walk Anim
            if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Idle") && !anim.GetCurrentAnimatorStateInfo(0).IsName("Walk"))
                bleedObject.SetActive(false);
        }


        if (hitArmorCount <= 0 && IsGrounded())
        {
            hitArmorTime -= Time.deltaTime;
            if (hitArmorTime <= 0)
            {
                hitArmorTime = maxHitArmorTime;
                hitArmorCount = maxHitArmorCount;
            }
        }/*

        if (parryTime >= 0)
            parryTime -= Time.deltaTime;
        if (parryTime <= 0)
            parry = false;*/

        ishealing = health < healValue;

        if (resourceValue > maxResourcevalue * 4)
            resourceValue = maxResourcevalue * 4;

        if (ishealing)
            health += healSpeed;

        armorBar.value = (int)hitArmorCount;
    }

    internal void ActivateSkill(int v)
    {
        if (!inSkill && anim.speed > 0)
            skills[v].CheckAbility();
    }

    //UI
    public void UpdateUI()
    {
        GameObject healthPoint = transform.Find("HealthPoint").gameObject;
        if (healthBar != null)
        {
            healthBar.transform.position = Camera.main.WorldToScreenPoint(healthPoint.transform.position);

            healthBar.value = health;
            healthBar.maxValue = maxHealth;
        }

        //Hero UI
        if (isPlayer)
        {
            UIManager uiManager = UIManager.instance;
            uiManager.mainHealthBar.value = health;


            //There are 4 Resource Bars and each should have atheir value based on this unit's resource value
            //Disable and enable the Glow Value to tell the player that they have a charge
            int total = (int)resourceValue;
            for (int i=0; i < uiManager.mainResourceBar.Count; i++)
            {
                //If Total is greater than ten(10) then we can round it up to a full bar
                if (total >= 30)
                {
                    //Decrement Total for the following values
                    total -= 30;
                    uiManager.mainResourceBar[i].value = resourceValue;
                    uiManager.mainResourceBar[i].transform.Find("Glow_Ready").gameObject.SetActive(true);
                }
                
                //Then add the remaining values to the current value of resource and then disable the glow charge
                else
                {
                    uiManager.mainResourceBar[i].value = total;
                    uiManager.mainResourceBar[i].transform.Find("Glow_Ready").gameObject.SetActive(false);
                    total = 0;
                }
            }
        }
    }

    private void Movement()
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Idle") || anim.GetCurrentAnimatorStateInfo(0).IsName("Walk"))
            isHit = false;

        if (health > 0)
        {
            state = MovementState.idle;
            if (IsGrounded() && anim.GetCurrentAnimatorStateInfo(0).IsName("Fall") && rb.velocity.y <= 0)
            {
                SpawnManager.instance.SpawnObject(5, transform.Find("shadow").transform.position, 1);
                //If already bounced then
                if (bounce == false)
                {
                    Invoke("ResetBounce", .1f);
                    anim.Play("GetUp");
                    if (isPlayer)
                        hitEffector.Blink(2);
                }

                //If not yet bounced then bounce once
                else if (bounce == true && rb.velocity.x >= .5f)
                {
                    bounce = false;
                    anim.Play("Lying");
                    rb.velocity = new Vector3(rb.velocity.x, 1.5f, rb.velocity.z);
                }
                else
                    bounce = false;
            }
        }

        else
        {
            if (IsGrounded() && anim.GetCurrentAnimatorStateInfo(0).IsName("Fall") && rb.velocity.y <= 0)
            {
                anim.Play("Death", 0, .5f);
            }
        }


        anim.SetBool("Attack", attack);


        for (int i = 0; i < nullStates.Length; i++)
        {
            if (anim.GetCurrentAnimatorStateInfo(0).IsName(nullStates[i]))
            {
                return;
            }
        }


        //Update Grounded
        anim.SetBool("Grounded", IsGrounded());/*
        if (attack && IsGrounded())
        {
            Attack();
        }*/


        if (isHit)
            return;

        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Idle") || anim.GetCurrentAnimatorStateInfo(0).IsName("Walk"))
        {
            if (IsGrounded() && !inSkill)
            {
                if (Mathf.Abs(horizontal) > .5f)
                    rb.velocity = new Vector3(moveSpeed * (int)hDirection, rb.velocity.y, rb.velocity.z);
                else
                    rb.velocity = new Vector3(0, rb.velocity.y, rb.velocity.z);


                if (Mathf.Abs(vertical) > .5f)
                    rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, (moveSpeed + 3) * (int)vDirection);
                else
                    rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, 0);


                //Check For Directions
                if (horizontal < 0)
                {
                    hDirection = -1;
                }

                if (horizontal > 0)
                {
                    hDirection = 1;
                }

                if (horizontal == 0)
                    hDirection = 0;

                if (vertical < 0)
                    vDirection = -1;

                if (vertical > 0)
                    vDirection = 1;

                if (vertical == 0)
                    vDirection = 0;
            }
        }

        if (Mathf.Abs(horizontal) > .5f || Mathf.Abs(vertical) > .5f)
        {
            state = MovementState.walk;
        }

        transform.localScale = new Vector3(direction, 1, 1);


        UpdateAnimationState();

        //Limit Movmeent
        if (transform.position.z > 9)
            transform.position = new Vector3(transform.position.x, transform.position.y, 9);
        if (transform.position.z < -4)
            transform.position = new Vector3(transform.position.x, transform.position.y, -4);
    }

    void ResetBounce()
    {
        bounce = true;
    }

    private void UpdateAnimationState()
    {  
        anim.SetInteger("State", (int)state);
    }

    public void ChangeDirection(int value)
    {
        direction = value;
    }


    //Buff Script Physics
    public void LaunchUnit(Vector2 force)
    {
        rb.velocity = force;
    }


    //States
    public void Jump()
    {
        for (int i = 0; i < nullStates.Length; i++)
        {
            if (anim.GetCurrentAnimatorStateInfo(0).IsName(nullStates[i]) || anim.GetCurrentAnimatorStateInfo(0).IsName("Crouch"))
                return;
        }

        if (IsGrounded())
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpHeight);
            SoundManager.instance.PlaySound(4, transform);
        }
    }

    public void Block()
    {
        for (int i = 0; i < nullStates.Length; i++)
        {
            if (anim.GetCurrentAnimatorStateInfo(0).IsName(nullStates[i]))
                return;
        }
        anim.Play("Block");

        anim.SetInteger("State", (int)state);
        rb.velocity = Vector2.zero;
    }
/*
    public void Attack()
    {
        if (attackAnim != "" && IsGrounded())
        {
            //ForcedAction(attackAnim);
            return;
        }

        else
        {
            
        }

        rb.velocity = Vector2.zero;
    }*/

    public void ForcedAction(string animation)
    {
        if (!anim.GetCurrentAnimatorStateInfo(0).IsName(animation))
        {
            anim.Play(animation);
            rb.velocity = Vector2.zero;
        }
    }

    public bool IsGrounded()
    {
        float amount = 1.1f;
        bool collision = Physics.Raycast(transform.position, Vector3.down, amount, groundLayer);
        //Debug.DrawRay(transform.position, Vector3.down * amount);
        return collision;
    }

    public void TakeDamage(float amount, int type, float force, GameObject owner, float forceUp = 0, bool isCrit = true)
    {
        //TimeManager.instance.RealStepPause(.1f);

        if (health <= 0 && IsGrounded())
            return;

        if (invincibilitySkill)
            return;


        if (!isPlayer)
            hitEffector.vulnerable = true;

        if (hitEffector.vulnerable && anim.GetCurrentAnimatorStateInfo(0).IsName("GetUp") == false)
        {
            if (hitArmorCount <= 0)
                TimeManager.instance.PlayObject(transform, rb);

            //Check for the case of enemies that have armor to be broken before takeing any damage
            if (hitArmorCount > 0)
            {
                hitArmorCount -= !isCrit ? 1 : maxHitArmorCount;
                if (hitArmorCount < 0)
                    hitArmorCount = 0;

                SFXandVFX(3, isCrit);
                //owner.transform.GetComponent<Unit>().hasHit = true;
                if (isCrit)
                    StartCoroutine(Camera.main.GetComponent<CameraShake>().Shake(.15f, .1f, isCrit));

                //Get a feedback to the player and make a stalled action
                if (anim.speed > 0)
                {
                    StartCoroutine(GetComponent<HitEffector>().Shake(1f, .02f));
                    TimeManager.instance.AnimPause(.5f, anim);
                }
                return;
            }

            //AddForce
            int dir = 1;
            if (transform.position.x - owner.transform.position.x < 0)
                dir = -1;
            if (transform.position.x - owner.transform.position.x > 0)
                dir = 1;
            rb.velocity = new Vector3(dir * force, forceUp);

            hitEffector.Blink();

            SFXandVFX(type, isCrit);

            //Apply Damage
            health -= amount;
            isHit = true;
            OnTakeDamage.Invoke();

            //IF you manage to take damage then reset all skill
            ResetAllSkill();

            //Set Combo
            if (UnitManager.instance.player != this)
                ComboManager.instance.ComboHit();

            //Damage Popup
            GameObject popup = Instantiate(damagePopup, transform.position + Vector3.up, Quaternion.identity);
            popup.transform.SetParent(UIManager.instance.transform);
            popup.transform.GetComponentInChildren<Text>().text = "" + (int)amount;

            //Blue Electric Color
            if (type == 2)
            {
                popup.transform.GetComponentInChildren<Text>().color = new Color(0, 1, 1, 1);
                popup.transform.Find("Crit").GetComponent<Image>().color = new Color(0, 1, 1, 1);
            }

            //If this is a Crit Damage then Intensify the Crit
            // * Add a Camera Shake
            // * Add a Visual Effect different from the normal hit efx
            // * Add a different sound effect different from the normal hit sfx
            // * Show a crit image on the "Damage Popup" text
            if (isCrit)
            {
                popup.transform.localScale = new Vector2(2, 2);
                popup.transform.Find("Crit").gameObject.SetActive(true);
                StartCoroutine(Camera.main.GetComponent<CameraShake>().Shake(.15f, .1f, isCrit));

                //Implement Juggling
                // Here's how it should work: Enemies have to be airborne to be able to be juggled (obviously)
                // The damage dealt would have to CRIT 
                // If all conditions are met (even if the "forceUp" value is zero) this unit will be juggled
                if (!IsGrounded() && isCrit && forceUp == 0)
                {
                    rb.velocity = new Vector3(owner.transform.localScale.x * force, 2.5f);
                }

            }
            else { StartCoroutine(Camera.main.GetComponent<CameraShake>().Shake(.15f, .04f, isCrit)); }


            popup.transform.position = Camera.main.WorldToScreenPoint(transform.position + Vector3.up);
            Destroy(popup, 2);
            //anim.Play("Hit", 0, 0);
            if (IsGrounded() == true)
                anim.Play("Hit", 0, 0);

            if (forceUp > 0)
                anim.Play("Fall", 0, 0);
        }
    }

    private void ResetAllSkill()
    {
        foreach (Skill sk in skills)
        {
            if (sk.inUse)
                sk.DeactivateAbility();
        }
    }

    public void ParryConnect(string animation = "Parry", bool playSound=true)
    {
        Vector2 point = transform.Find("Container").Find("Sprite").Find("Hitbox").transform.position;
        GameObject go = Instantiate(parryHitVFX, point + Vector2.up * .23f, Quaternion.identity) as GameObject;
        Destroy(go, 2f);
        anim.Play(animation);
    }

    public void Death()
    {
        if (alive == false)
            return;

        alive = false;
        OnDeath.Invoke();
        if (isPlayer)
        {
            UIManager.instance.Failed();
            UIManager.instance.DisableUI();
        }

        //If this is not a Hero type Unit
        //do this instead
        else
        {
            //Give Exp to the actual Hero
            DataManager.instance.AddExp(baseHealth * .3f);

            //Add number of enemies beaten to the player
            DataManager.instance.dataPersistence.gameData.enemiesBeaten += 1;

            //If this is a boss then make a dramatic finisher
            if (isBoss)
            {
                TimeManager.instance.SlowMotion(3, .2f);
                Camera.main.GetComponent<CameraShake>().Shake(6, .1f, false);
            }

            //Small Camera Slow Mo.
            /*if (!isBoss)
                TimeManager.instance.SlowMotion(.5f, .5f);*/

            //EXP Popup
            GameObject popup = Instantiate(expPopup, UnitManager.instance.player.transform.position + Vector3.up, Quaternion.identity);
            popup.transform.SetParent(UIManager.instance.transform);
            popup.transform.GetComponentInChildren<Text>().text = "+" + (int)(baseHealth * .3f) + " EXP";
            popup.transform.position = Camera.main.WorldToScreenPoint(UnitManager.instance.player.transform.position + Vector3.up);
            Destroy(popup, 3);
        }

        anim.Play("Death");
        state = MovementState.death;
        anim.SetInteger("State", (int)state);

        hitEffector.Blink(5);

        //Else Die

        try
        {
            Destroy(healthBar.gameObject);
        }
        catch { }

        UnitManager.instance.RemoveUnit(this);
    }
/*
    IEnumerator InitRevive(float healthValue)
    {
        yield return new WaitForSeconds(3);
        hitEffector.Blink(3);
        healValue = maxHealth * healthValue;
        health = 1;
        ishealing = true;
        state = MovementState.idle;
        anim.SetInteger("State", (int)state);
    }*/

    //BODMAS
    void SFXandVFX(int dmgType, bool isCrit)
    {
        Vector3 point = transform.position;
        Vector2 dir = new Vector2(point.x, point.y);
        if (point.x < transform.position.x)
            dir.x = 1;

        if (point.x > transform.position.x)
            dir.x = -1;
        Vector2 offset = new Vector2(0, 0);
        //Fist
        if (dmgType == 0)
        {

            //IF this is not a critical hit
            if (!isCrit)
            {
                SpawnManager.instance.SpawnObject(0, transform.position, 2);
                SoundManager.instance.PlaySound(0, transform);
            }

            //If this IS a critical hit
            if (isCrit)
            {
                SoundManager.instance.PlaySound(1, transform);
                SpawnManager.instance.SpawnObject(1, transform.position, 2);
            }
        }


        //Blood
        if (dmgType == 1)
        {
            if (!isCrit)
            {
                SoundManager.instance.PlaySound(2, transform);
                SpawnManager.instance.SpawnObject(2, transform.position, 2);
            }

            //Intensify if this is a Blood CRIT
            if (isCrit)
            {
                SoundManager.instance.PlaySound(2, transform);
                SpawnManager.instance.SpawnObject(11, transform.position + Vector3.up, .3f);
            }
        }


        //Lightning
        if (dmgType == 2)
        {
            if (!isCrit)
            {
                SoundManager.instance.PlaySound(8, transform);
                SpawnManager.instance.SpawnObject(6, transform.position, 2);
            }

            //Intensify if this is a broken ARMOR CRIT
            if (isCrit)
            {
                SoundManager.instance.PlaySound(9, transform);
                SpawnManager.instance.SpawnObject(7, transform.position + Vector3.up, .3f);
            }
        }

        //Armor
        if (dmgType == 3)
        {
            if (!isCrit)
            {
                SoundManager.instance.PlaySound(5, transform);
                SpawnManager.instance.SpawnObject(3, transform.position, 2);
            }

            //Intensify if this is a broken ARMOR CRIT
            if (isCrit)
            {
                SoundManager.instance.PlaySound(6, transform);
                SpawnManager.instance.SpawnObject(4, transform.position + Vector3.up, .3f);
                SpawnManager.instance.SpawnObject(3, transform.position, .3f);
            }
        }

        //Fire
        if (dmgType == 4)
        {
            if (!isCrit)
            {
                SoundManager.instance.PlaySound(10, transform);
                SpawnManager.instance.SpawnObject(8, transform.position, 2);
            }

            //Intensify if this is a broken ARMOR CRIT
            if (isCrit)
            {
                SoundManager.instance.PlaySound(11, transform);
                SpawnManager.instance.SpawnObject(9, transform.position, .3f);
            }
        }
    }

    public void ClearHealthBar()
    {
        if (healthBar != null)
            Destroy(healthBar);
    }

    public void HideHealthBar()
    {
        if (healthBar != null)
            healthBar.gameObject.SetActive(false);
    }

    public void ShowHealthBar()
    {
        if (healthBar != null)
            healthBar.gameObject.SetActive(true);
    }


    private void OnDestroy()
    {
        ClearHealthBar();
    }

    public void OnDisable()
    {
        HideHealthBar();
    }

    public void OnEnable()
    {
        ShowHealthBar();
    }
}

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;


public enum AbilityType
{
    forwardDamage,
    AOE,
    health,
    counter,
    others
}


public class Skill : MonoBehaviour
{
    ///<summary> 
    /// Skills are abilites that help the unit in various ways (in terms of damage, healing, and other buffs)
    /// They Should have a few conditions necessary for activation
    ///</summary>
    ///
    public Unit unit;

    [Header("Stats")]
    public float resourceCost;
    public float coolDown;
    internal float maxCoolDown;

    [Header("Ability Info")]
    public string animatorSkillName;
    public string abilityName;

    [Tooltip("This damage is added as bonus damage to the base damage of the unit and it scales with level")]
    public float abilityDamage;
    internal bool canUseAbility = true;

    [Tooltip("This is to check if it is necessary to do all that camera and visual effect when the skill is activated")]
    public bool cutSceenSkill = true;
    public bool invincibilitySkill = true;

    [Header("Graphics")]
    public Sprite skillIcon;

    [Tooltip("This checks for the list of contitions necessary before the ability is executed" +
        "\n Condition A: Unit has to be grounded" +
        "\n Condition B: Unit has to be Airborne" +
        "\n Condition C: Unit has to be in the idle or walk state, which means that this ability cannot be used as a combo")]
    public List<bool> abilityConditions;

    public UnityEvent OnActivateAbility;
    public UnityEvent OnDeActivateAbility;

    public bool isAbilityAvailable = false;
    public int abilityLevel = 1;
    internal bool inUse;

    [Header("AI Tools")]
    public AbilityType abilityType;

    private void Start()
    {
#if PLATFORM_STANDALONE || PLATFORM_WEBGL
        Transform skillLayout = UIManager.instance.pcSkillLayout.transform.Find(animatorSkillName);
        if (unit.isPlayer)
            skillLayout.Find("Button").GetComponent<Image>().sprite = skillIcon;
#endif

#if PLATFORM_ANDROID
        Transform skillLayout = UIManager.instance.androidSkillLayout.transform.Find(animatorSkillName);
        if (unit.isPlayer)
            skillLayout.Find("Button").GetComponent<Image>().sprite = skillIcon;
#endif

        maxCoolDown = coolDown;

        //Scale with Level
        float baseDamage = abilityDamage;
        for (int i = 0; i < unit.unitLevel; i++)
        {
            abilityDamage += baseDamage * .2f;
        }
    }

    private void Update()
    {
        //Check if the owner's level is equal to the ability's level
        isAbilityAvailable = unit.unitLevel >= abilityLevel;


        UpdateStatus();

        //Update The UI
        if (unit.isPlayer)
        {
#if PLATFORM_ANDROID
            UpdateSkillUIAndroid();
#endif

#if PLATFORM_STANDALONE || PLATFORM_WEBGL
            UpdateSkillUIPC();
#endif
        }
    }

    private void UpdateStatus()
    {
        //Most importantly is if this ability is not available for the unit, then DO NOT DO ANYTHING
        if (!isAbilityAvailable)
            return;

        if (!unit.inSkill)
        {
            if (coolDown < maxCoolDown)
                coolDown += Time.deltaTime;

            if (coolDown > maxCoolDown)
            {
                canUseAbility = true;
                coolDown = maxCoolDown;
            }
        }
    }
    private void UpdateSkillUIPC()
    {/*
        //Most importantly is if this ability is not available for the unit, then DO NOT DO ANYTHING
        if (!isAbilityAvailable)
            return;*/

#if PLATFORM_STANDALONE || PLATFORM_WEBGL
        Transform skillLayout = UIManager.instance.pcSkillLayout.transform.Find(animatorSkillName);

        //Update if the ability is or is not not available 
        skillLayout.gameObject.SetActive(isAbilityAvailable);

        skillLayout.Find("CoolDownText").GetComponent<TextMeshProUGUI>().text = "" + (maxCoolDown - (int)coolDown);
        skillLayout.Find("CoolDownImage").GetComponent<Image>().transform.localScale = new Vector3(1, 1 - (coolDown / maxCoolDown), 1);

        if (coolDown >= maxCoolDown)
            skillLayout.Find("CoolDownText").GetComponent<TextMeshProUGUI>().text = "";
#endif

#if PLATFORM_ANDROID 
        Transform skillLayout = UIManager.instance.androidSkillLayout.transform.Find(animatorSkillName);

        //Update if the ability is or is not not available 
        skillLayout.gameObject.SetActive(isAbilityAvailable);

        skillLayout.Find("CoolDownText").GetComponent<TextMeshProUGUI>().text = "" + (maxCoolDown - (int)coolDown);
        skillLayout.Find("CoolDownImage").GetComponent<Image>().transform.localScale = new Vector3(1, 1 - (coolDown / maxCoolDown), 1);

        if (coolDown >= maxCoolDown)
            skillLayout.Find("CoolDownText").GetComponent<TextMeshProUGUI>().text = "";
#endif
    }

    private void UpdateSkillUIAndroid()
    {/*
        //Most importantly is if this ability is not available for the unit, then DO NOT DO ANYTHING
        if (!isAbilityAvailable)
            return;*/
        Transform skillLayout = UIManager.instance.androidSkillLayout.transform.Find(animatorSkillName);

        //Update if the ability is or is not not available 
        skillLayout.gameObject.SetActive(isAbilityAvailable);


        skillLayout.Find("CoolDownText").GetComponent<TextMeshProUGUI>().text = "" + (maxCoolDown - (int)coolDown);
        skillLayout.Find("CoolDownImage").GetComponent<Image>().transform.localScale = new Vector3(1, 1 - (coolDown / maxCoolDown), 1);

        if (coolDown >= maxCoolDown)
            skillLayout.Find("CoolDownText").GetComponent<TextMeshProUGUI>().text = "";
    }

    public bool CanActivate()
    {
        //Condition A: Unit has to be Grounded
        if (abilityConditions[0] == true)
        {
            if (!unit.IsGrounded())
                return false;
        }

        //Airborne Condition
        if (abilityConditions[1])
        {
            if (unit.IsGrounded())
                return false;
        }

        //Condition C: Unit has to be in the idle or walk state, which means that this ability cannot be used as a combo
        if (abilityConditions[2])
        {
            Animator unitAnim = unit.transform.Find("Container").Find("Sprite").GetComponent<Animator>();
            if (!unitAnim.GetCurrentAnimatorStateInfo(0).IsName("Idle") && !unitAnim.GetCurrentAnimatorStateInfo(0).IsName("Walk"))
            {
                return false;
            }
        }

        return true;
    }
    public void CheckAbility()
    {
        //Most importantly is if this ability is not available for the unit, then DO NOT DO ANYTHING
        if (!isAbilityAvailable)
            return;

        //IF the unit is knocked down and lying on the ground do nothing
        if (unit.transform.Find("Container").Find("Sprite").GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("GetUp") ||
            unit.transform.Find("Container").Find("Sprite").GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Hit"))
            return;


        //If the resource value is not enough then don't even bother with any more lines of code
        //Simply Return
        if (unit.resourceValue < resourceCost)
            return;


        if (!canUseAbility)
            return;
        if (coolDown < maxCoolDown)
            return;


        //#TODO: Check  if all conditions are met 
        // If so then..
        //LaunchAbility()

        //Conditio B: This Unit has to be airborne to activate this ability
        if (abilityConditions[1])
        {
            if (unit.IsGrounded())
                return;
        }

        //Condition A: Unit has to be Grounded
        if (abilityConditions[0] == true)
        {
            if (!unit.IsGrounded())
                return;
        }

        //Condition C: Unit has to be in the idle or walk state, which means that this ability cannot be used as a combo
        if (abilityConditions[2])
        {
            Animator unitAnim = unit.transform.Find("Container").Find("Sprite").GetComponent<Animator>();
            if (unitAnim.GetCurrentAnimatorStateInfo(0).IsName("Idle") || unitAnim.GetCurrentAnimatorStateInfo(0).IsName("Walk"))
            {
                ActivateAbility();
                return;
            }
            else
                return;
        }

        ActivateAbility();
    }

    public void ActivateAbility()
    {
        //Consume Resource
        unit.resourceValue -= resourceCost;
        canUseAbility = false;
        coolDown = 0;
        OnActivateAbility.Invoke();
        unit.anim.Play(animatorSkillName);
        inUse = true;
        unit.inSkill = true;
        unit.invincibilitySkill = invincibilitySkill;
        unit.OnUseAbility.Invoke();

        if (cutSceenSkill)
        {
            //SFX
            SoundManager.instance.PlaySound(7, transform);

            //UI
            UIManager.instance.transform.Find("SuperAttackBoarder").gameObject.SetActive(true);
            UIManager.instance.transform.Find("SuperAttackBoarder").GetComponent<Animator>().Play("Super_In", 0, 0);
            UIManager.instance.transform.Find("SuperAttackBoarder").Find("Boarders").Find("SuperText").GetComponent<Text>().text = abilityName;
            UIManager.instance.transform.Find("SuperAttackBoarder").Find("Boarders").Find("SuperText").GetComponent<Text>().color = unit.theme;
            UIManager.instance.DisableUI();

            //VFX
            Camera.main.transform.parent.GetComponent<CameraMovement>().targetFOV = 2.5f;
            Camera.main.transform.parent.GetComponent<CameraMovement>().limitless = true;
            Camera.main.transform.parent.GetComponent<CameraMovement>().target = transform.parent.parent;
            BackgroundManager.instance.DeactivateWorld(unit.theme);

            //Time Bending
            TimeManager.instance.StepPause(2, transform.parent.parent);
        }

        //Hero Conditions
        if (unit.isPlayer)
        {
        }
    }

    public void DeactivateAbility()
    {
        unit.inSkill = false;
        unit.invincibilitySkill = false;
        inUse = false;
        OnDeActivateAbility.Invoke();

        //Resetting
        Camera.main.transform.parent.GetComponent<CameraMovement>().targetFOV = 3.8f;
        Camera.main.transform.parent.GetComponent<CameraMovement>().target = UnitManager.instance.player.transform;
        Camera.main.transform.parent.GetComponent<CameraMovement>().limitless = false;
        UIManager.instance.transform.Find("SuperAttackBoarder").gameObject.SetActive(false);

        if (UnitManager.instance.player.alive)
            UIManager.instance.EnableUI();


        //VFX
        BackgroundManager.instance.ActivateWorld();
        if (unit.isPlayer)
        {
        }
    }
}

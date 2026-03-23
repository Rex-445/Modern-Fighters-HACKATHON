using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class UnitController : MonoBehaviour
{
    private Unit unit;
    public bool canControl = true;

    private void Start()
    {
        unit = GetComponent<Unit>();
    }

    private void Update()
    {
        if (!canControl)
        {
            return;
        }

        try
        {
            if (unit.anim.GetCurrentAnimatorStateInfo(0).IsName("Hit"))
                return;
        }
        catch { }

        Movement();
    }

    public void Control(bool value)
    {
        canControl = value;
        unit.horizontal = 0;
        unit.vertical = 0;
        unit.attack = false;
        unit.kick = false;
        unit.crouch = false;
        unit.state = Unit.MovementState.idle;
    }

    void Movement()
    {
        if (unit.health <= 0)
            return;


#if PLATFORM_ANDROID

        unit.attack = CrossPlatformInputManager.GetButton("Attack");
        unit.kick = CrossPlatformInputManager.GetButton("Kick");

        bool jump = CrossPlatformInputManager.GetButton("Jump");
        float h = CrossPlatformInputManager.GetAxis("Horizontal");
        float v = CrossPlatformInputManager.GetAxis("Vertical");

        //Skills
        {
            bool skill1 = CrossPlatformInputManager.GetButtonDown("Skill1");
            bool skill2 = CrossPlatformInputManager.GetButtonDown("Skill2");
            bool skill3 = CrossPlatformInputManager.GetButtonDown("Skill3");
            bool skill4 = CrossPlatformInputManager.GetButtonDown("Skill4");

            if (skill1)
            {
                unit.ActivateSkill(0);
            }

            if (skill2)
            {
                unit.ActivateSkill(1);
            }

            if (skill3)
            {
                unit.ActivateSkill(2);
            }

            if (skill4)
            {
                unit.ActivateSkill(3);
            }
        }

        if (!UIManager.instance.isJoystick)
        {
            //Buttons
            bool up = CrossPlatformInputManager.GetButton("Up");
            bool down = CrossPlatformInputManager.GetButton("Down");
            bool left = CrossPlatformInputManager.GetButton("Left");
            bool right = CrossPlatformInputManager.GetButton("Right");

            if (up) v = 1;
            if (down) v = -1;
            if (left) h = -1;
            if (right) h = 1;
        }


        if (UIManager.instance.isJoystick)
        {

        }

        if (jump)
        {
            unit.Jump();
        }

        unit.horizontal = h;
        unit.vertical = v;
        if (h != 0)
        {
            float value = h;
            if (h < 0)
                value = -1;
            if (h > 0)
                value = 1;
            unit.ChangeDirection((int)value);
            //print(value);
        }


#endif


#if PLATFORM_WEBGL || PLATFORM_STANDALONE

        unit.attack = Input.GetButton("Attack");
        unit.kick = Input.GetButton("Kick");
        bool jump = Input.GetButton("Jump");

        //Movement
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        /*if (jump)
        {
            unit.Jump();
        }*/
        
        //Skills
        {
            bool skill1 = Input.GetButtonDown("Skill1");
            bool skill2 = Input.GetButtonDown("Skill2");
            bool skill3 = Input.GetButtonDown("Skill3");
            bool skill4 = Input.GetButtonDown("Skill4");

            if (skill1)
            {
                unit.ActivateSkill(0);
            }

            if (skill2)
            {
                unit.ActivateSkill(1);
            }

            if (skill3)
            {
                unit.ActivateSkill(2);
            }

            if (skill4)
            {
                unit.ActivateSkill(3);
            }
        }

        //Controls
        {
            bool right = Input.GetButton("Right");
            bool left = Input.GetButton("Left");
            bool up = Input.GetButton("Up");
            bool down = Input.GetButton("Down");
            if (up) v = 1;
            if (down) v = -1;
            if (left) h = -1;
            if (right) h = 1;
        }

        unit.horizontal = h;
        unit.vertical = v;
        if (h != 0)
        {
            float value = h;
            if (h < 0)
                value = -1;
            if (h > 0)
                value = 1;
            unit.ChangeDirection((int)value);
            //print(value);
        }

#endif
    }
}

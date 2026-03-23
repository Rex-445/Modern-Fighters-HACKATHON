using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public Transform left;
    public Transform right;

    public Transform target;
    public float offset;

    public Camera mainCam;
    public float targetFOV;
    public bool limitless = false;

    [Header("Camera Direction")]
    public float changeDirWaitTime;
    public int currentDirection;
    internal float maxChangeDirWaitTime;


    [SerializeField] private float speed = 4;
    [SerializeField] private float followDistance = 4;
    Transform bound;


    private void Start()
    {
        maxChangeDirWaitTime = changeDirWaitTime;

        if (mainCam == null)
            mainCam = transform.Find("ActualCamera").GetComponent<Camera>();
        targetFOV = mainCam.orthographicSize;

        if (target == null)
        {
            GameObject[] objects = GameObject.FindGameObjectsWithTag("Unit");
            foreach (GameObject go in objects)
            {
                if (go.GetComponent<Unit>().isPlayer)
                {
                    target = go.transform;
                    return;
                }
            }
        }

        bound = GameManager.instance.bound;
    }

    private void Update()
    {
        if (bound == null)
        {
            bound = GameManager.instance.bound;
        }

        right.position = new Vector3(bound.position.x, right.position.y, right.position.z);




        if (!left || !right)
            return;
    }

    [System.Obsolete]
    private void LateUpdate()
    {
        mainCam.orthographicSize = Mathf.Lerp(mainCam.orthographicSize, targetFOV, .05f);
        if (target)
        {
            Movement();
            if (target.GetComponent<Unit>() != null)
                UpdateDirections();
        }
        else
        {
            target = UnitManager.instance.player.transform;
            currentDirection = target.GetComponent<Unit>().direction;
        }

        Restrictions();
    }

    [System.Obsolete]
    Vector3 GetCenterPoint()
    {
        List<Transform> availableTargets = GetAvailableTargets();

        if (availableTargets.Count == 1)
            return availableTargets[0].position;

        var bounds = new Bounds(availableTargets[0].position, Vector3.zero);
        for (int i = 0; i < availableTargets.Count; i++)
        {
            bounds.Encapsulate(availableTargets[i].position);
        }

        return bounds.center;
    }

    [System.Obsolete]
    public List<Transform> GetAvailableTargets()
    {
        List<Transform> allUnits = UnitManager.instance.allUnits;
        List<Transform> newUnits = new List<Transform>();

        //Get the distance required to follow the Units
        for (int j = 0; j < allUnits.Count; j++)
        {
            if (Vector3.Distance(target.position, allUnits[j].position) < followDistance)
            {
                if (allUnits[j].gameObject.active)
                    newUnits.Add(allUnits[j]);
            }
        }

        return newUnits;
    }



    private void UpdateDirections()
    {
        //If the Camera's direction is not the same as it's target
        if (currentDirection != target.GetComponent<Unit>().direction)
        {
            //Stall for a bit
            changeDirWaitTime -= Time.deltaTime;
            if (changeDirWaitTime <= 0)
            {
                //Then change it's direction
                changeDirWaitTime = maxChangeDirWaitTime;
                currentDirection = target.GetComponent<Unit>().direction;
            }
        }
        else
        {
            //Reset in case of inaccurate wait times
            changeDirWaitTime = maxChangeDirWaitTime;
        }
    }

    [System.Obsolete]
    void Movement()
    {

        //Check if this is a Unit
        try
        {
            Vector3 center = GetCenterPoint();
            transform.localPosition = Vector2.Lerp(transform.position, new Vector2(center.x + (offset * currentDirection), center.y), speed * Time.deltaTime);
            //transform.localPosition = Vector2.Lerp(transform.position, new Vector3(target.position.x + (offset * currentDirection), target.position.y, 0), speed * Time.deltaTime);
        } 
        
        //Catch to see if it isn't
        catch
        {
            transform.localPosition = Vector2.Lerp(transform.position, target.position, speed * Time.deltaTime);
        }

        //Reset Z Pos
        transform.localPosition = new Vector3(transform.position.x, transform.position.y, 0);
    }


    public IEnumerator Shake(float duration, float mag)
    {
        Vector3 originalPos = transform.localPosition;
        float elapsed = 0.0f;
        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * mag;
            float y = Random.Range(-1f, 1f) * mag;

            Vector3 newPos = new Vector3(transform.position.x + x, transform.position.y + y, originalPos.z);

            transform.localPosition = Vector3.Lerp(transform.position, newPos, Time.deltaTime);

            elapsed += Time.deltaTime;

            yield return null;
        }
    }

    /// <summary>This is used to focus on specific Characters when performing a special skill or in a cutscene</summary>
    /// <param name="duration"> How long the camera focuses on the targeted object </param>
    /// <param name="targetObject"> The object the Camera focuses on for a fixed time </param>
    public void CameraFocus(float duration, Transform targetObject)
    {
        target = targetObject;
        targetFOV = 2.8f;
        Invoke("EndCamFocus", duration);
    }

    void EndCamFocus()
    {
        target = null;
        targetFOV = 4f;
    }

    void Restrictions()
    {
        //Left
        if (transform.position.x < left.position.x && !limitless)
            transform.position = new Vector3(left.position.x, transform.position.y, 0);

        //Top
        if (transform.position.y > left.position.y && !limitless)
            transform.position = new Vector3(transform.position.x, left.position.y, 0);

        //Right
        if (transform.position.x > right.position.x && !limitless)
            transform.position = new Vector3(right.position.x, transform.position.y, 0);

        //Bottom
        if (transform.position.y < right.position.y && !limitless)
            transform.position = new Vector3(transform.position.x, right.position.y, 0);
    }
}

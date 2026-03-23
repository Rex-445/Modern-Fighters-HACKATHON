using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


public class RedPotion : MonoBehaviour
{
    public float amount = .5f;

    public GameObject buffPopup; 

    public UnityEvent OnTrigger;

    public void OnTriggerEnter(Collider collision)
    {
        if (collision.transform.tag == "Unit")
        {
            if (collision.gameObject.layer == 7)
            {
                Unit player = collision.transform.GetComponent<Unit>();
                if (player)
                {
                    float value = (player.maxHealth * amount);
                    player.health += (int)value;

                    if (player.health > player.maxHealth)
                    {
                        player.health = player.maxHealth;
                    }

                    //UI Spawn
                    GameObject go = Instantiate(buffPopup, collision.transform.position + Vector3.up, Quaternion.identity);
                    go.transform.SetParent(UIManager.instance.transform);
                    go.transform.Find("Transform").Find("Offset").Find("Text").GetComponent<Text>().text = "+" + (int)player.maxHealth * amount;
                    go.transform.position = Camera.main.WorldToScreenPoint(collision.transform.position + Vector3.up * 1.3f);
                    Destroy(go, 3f);
                    OnTrigger.Invoke();
                }
            }
        }
    }
}

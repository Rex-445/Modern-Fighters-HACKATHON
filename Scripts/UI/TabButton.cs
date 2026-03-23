using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

namespace Assets.Scripts.UI
{
    [RequireComponent(typeof(Image))]
    public class TabButton : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
    {
        public TabGroup tabGroup;

        public Color background;
        public UnityEvent OnTabSelected;
        public UnityEvent OnTabDeSelected;
        public GameObject content;

        public void OnPointerClick(PointerEventData eventData)
        {
            tabGroup.OnTabSelected(this);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            tabGroup.OnTabEnter(this);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            tabGroup.OnTabExit(this);
        }

        // Use this for initialization
        void Start()
        {
            background = GetComponent<Image>().color;
            if (tabGroup == null)
                tabGroup = transform.parent.GetComponent<TabGroup>();
            tabGroup.Subscribe(this);
        }

        // Update is called once per frame
        void Update()
        {
            GetComponent<Image>().color = Color.Lerp(GetComponent<Image>().color, background, .1f);
        }

        public void Select()
        {
            OnTabSelected.Invoke();
            content.SetActive(true);
        }

        public void DeSelect()
        {
            OnTabDeSelected.Invoke();
            content.SetActive(false);
        }
    }
}
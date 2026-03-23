using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.UI
{
    public class TabGroup : MonoBehaviour
    {
        public List<TabButton> tabButtons;
        public Color tabIdle;
        public Color tabHover;
        public Color tabSelected;
        public TabButton selectedTab;
        public List<GameObject> tabObjects;

        public void LoadDifficulty(GameData gameData)
        {
            foreach(TabButton tab in tabButtons)
            {
                if (tab.name == gameData.difficulty)
                {
                    OnTabSelected(tab);
                    break;
                }
            }
        }


        public void Subscribe(TabButton button)
        {
            if (tabButtons == null)
            {
                tabButtons = new List<TabButton>();
            }

            tabButtons.Add(button);
        }


        public void OnTabEnter(TabButton button)
        {
            ResetTabs();
            if (button != null && button == selectedTab) { return; }
            button.background = tabHover;
        }

        public void OnTabExit(TabButton button)
        {
            ResetTabs();
        }

        public void OnTabSelected(TabButton button)
        {
            if (selectedTab != null)
            {
                selectedTab.DeSelect();
            }
            selectedTab = button;
            selectedTab.Select();
            ResetTabs();
            button.background = tabSelected;

            int index = button.transform.GetSiblingIndex();
            for (int i=0; i < tabObjects.Count; i++)
            {
                if (i == index)
                {
                    tabObjects[i].SetActive(true);
                }
                else
                    tabObjects[i].SetActive(false);
            }
        }

        public void ResetTabs()
        {
            foreach(TabButton btn in tabButtons)
            {
                if (btn != null && btn == selectedTab) { continue; }
                btn.background = tabIdle;
            }
        }

        public void ResetAllTabs()
        {
            foreach (TabButton btn in tabButtons)
            {
                btn.background = tabIdle;
                btn.DeSelect();
            }
        }
    }
}
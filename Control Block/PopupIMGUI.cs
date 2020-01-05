﻿// Popup list with multi-instance support created by Xiaohang Miao. (xmiao2@ncsu.edu)

using UnityEngine;

namespace Control_Block
{
    public class Popup
    {
        public Popup(string[] items)
        {
            this.items = items;
        }
        private string[] items;

        private Vector2 scroll = Vector2.zero;

        // Represents the selected index of the popup list, the default selected index is 0, or the first item
        public int selectedItemIndex = 0;

        // Represents whether the popup selections are visible (active)
        private bool isVisible = false;

        // Represents whether the popup button is clicked once to expand the popup selections
        private bool isClicked = false;

        // If multiple Popup objects exist, this static variable represents the active instance, or a Popup object whose selection is currently expanded
        private static Popup current;

        public Rect Show()
        {
            // Draw a button. If the button is clicked
            if (GUILayout.Button(items[selectedItemIndex]))
            {
                // If the button was not clicked before, set the current instance to be the active instance
                if (!isClicked)
                {
                    current = this;
                    isClicked = true;
                }
                // If the button was clicked before (it was the active instance), reset the isClicked boolean
                else
                {
                    isClicked = false;
                    if (current == this) current = null;
                    isVisible = false;
                }
            }
            var box = GUILayoutUtility.GetLastRect();

            if (isVisible)
            {
                // Draw a Box
                Rect listRect = new Rect(0, 0, box.width - 20, box.height * items.Length);
                Rect scrollRect = new Rect(box.x, box.y + box.height, box.width, Mathf.Min(box.height * items.Length, 120));
                //GUI.Box(scrollRect, "");

                scroll = GUI.BeginScrollView(scrollRect, scroll, listRect, GUIStyle.none, GUIStyle.none);

                GUI.changed = false;
                // Draw a SelectionGrid and listen for user selection
                selectedItemIndex = GUI.SelectionGrid(listRect, selectedItemIndex, items, 1, GUIStyle.none);

                // If the user makes a selection, make the popup list disappear
                if (GUI.changed)
                {
                    current = null;
                }
                GUI.EndScrollView();
            }

            return box;
        }

        // This function is ran inside of OnGUI()
        // For usage, see http://wiki.unity3d.com/index.php/PopupList#Javascript_-_PopupListUsageExample.js
        public int List(Rect box)
        {

            // If the instance's popup selection is visible
            if (isVisible)
            {
                // Draw a Box
                Rect listRect = new Rect(0, 0, box.width - 20, box.height * items.Length);
                Rect scrollRect = new Rect(box.x, box.y + box.height, box.width, Mathf.Min(box.height * items.Length, 120));
                GUI.Box(scrollRect, "");

                scroll = GUI.BeginScrollView(scrollRect, scroll, listRect);

                GUI.changed = false;
                // Draw a SelectionGrid and listen for user selection
                selectedItemIndex = GUI.SelectionGrid(listRect, selectedItemIndex, items, 1);

                // If the user makes a selection, make the popup list disappear
                if (GUI.changed)
                {
                    current = null;
                }
                GUI.EndScrollView();
            }

            // Get the control ID
            int controlID = GUIUtility.GetControlID(FocusType.Passive);

            // Listen for controls
            switch (Event.current.GetTypeForControl(controlID))
            {
                // If mouse button is clicked, set all Popup selections to be retracted
                case EventType.MouseUp:
                    {
                        current = null;
                        isVisible = false;
                        break;
                    }
            }

            // If the instance is the active instance, set its popup selections to be visible
            if (current == this)
            {
                isVisible = true;
            }

            // These resets are here to do some cleanup work for OnGUI() updates
            else
            {
                isVisible = false;
                isClicked = false;
            }

            // Return the selected item's index
            return selectedItemIndex;
        }

        // Get the instance variable outside of OnGUI()
        public int GetSelectedItemIndex()
        {
            return selectedItemIndex;
        }
    }

}

//http://wiki.unity3d.com/index.php?title=PopupList#C.23_-_Popup.cs
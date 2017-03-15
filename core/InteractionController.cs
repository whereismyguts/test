﻿using System;
using System.Collections.Generic;
using System.Linq;


namespace GameCore {

    public class InteractionController {
        GameState GameState { get { return MainCore.Instance.State; } }
        bool oldMousePressed;
        int[] keys;
        Dictionary<GameState, List<InteractiveObject>> interfaces = new Dictionary<GameState, List<InteractiveObject>>();
        int pressCoolDown = 0;

        public int[] Keys { get { return keys; } }

        public void Add(InteractiveObject obj, GameState state) {
            if(!interfaces.ContainsKey(state))
                interfaces[state] = new List<InteractiveObject>();
            if(!interfaces[state].Contains(obj))
                interfaces[state].Add(obj);
        }

        public List<InteractiveObject> GetActualInterface() {
            return interfaces.ContainsKey(GameState) ? interfaces[GameState] : new List<InteractiveObject>();
        }

        public void HitTest(bool pressed, object position, int key) {

            if(pressCoolDown < 0)
                pressCoolDown++;
            else
                if(interfaces.ContainsKey(GameState)) {
                List<InteractiveObject> controls = interfaces[GameState];
                for(int i = 0; i < controls.Count; i++) {
                    var obj = controls[i];
                    if(obj.Contains(position))
                        if(!pressed && oldMousePressed) {
                            obj.HandleMouseClick(position);
                            obj.IsSelected = true;
                            pressCoolDown = -10;
                        }
                        else {
                            obj.HandleMouseHover(position);
                            obj.IsHighlighted = true;
                        }
                    else {
                        obj.IsHighlighted = false;
                        if(!pressed && oldMousePressed)
                            obj.IsSelected = false;
                    }
                    if(obj.IsSelected && key > -1) {
                        obj.HandleKeyPress(key);
                        pressCoolDown = -10;
                    }
                }
            }
            oldMousePressed = pressed;
        }


        public void SetPressedKeys(IEnumerable<int> keys) {
            this.keys = keys.ToArray();
        }
    }
}

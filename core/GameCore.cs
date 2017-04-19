﻿using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GameCore {


    public class StateEventArgs: EventArgs {
        readonly UIState state;
        public UIState State { get { return state; } }

        internal StateEventArgs(UIState state) {
            this.state = state;
        }
    }
    public class MainCore {

        World world = new World(Vector2.Zero);

        static MainCore instance;

        static List<UIState> states = new List<UIState>();
        static int state;

        public StarSystem System;
        public static MainCore Instance {
            get {
                return instance;
            }
        }

        public List<GameObject> Objects {
            get { return GetAllObjects().ToList(); }
        }

        public List<Ship> Ships { get { return ships; } }

        public UIState CurrentState {
            get { return states[state]; }
        }

        public static void SwitchState() {
            if(state < states.Count - 1)
                state++;
            else
                state = 0;
        }

        public static void AddControl(int state, IControl control, int actor) {
            states.First(st => st.Id == state).AddControl(control, actor);
        }

        //public int State {
        //    get { return state; }
        //    set {
        //        if(state == value)
        //            return;
        //        state = value;
        //        if(StateChanged != null)
        //            StateChanged(instance, new StateEventArgs(states[value]));
        //    }
        //}

        public delegate void StateEventHandler(object sender, StateEventArgs e);

        internal bool CursorPressed { get; private set; } = false;

        public GameObject HookedObject { get; private set; } = null;

        public void MousePressed() {
            CursorPressed = true;
            foreach(var obj in Objects) {
                if(obj.ObjectBounds.Contains(Cursor)) {
                    HookedObject = obj;
                    return;
                }
            }
        }

        public void MouseReleased() {
            CursorPressed = false;
            HookedObject = null;
        }

        public static void AddStates(UIState[] newstates) {
            states.AddRange(newstates);
        }

        //public event StateEventHandler StateChanged;
        //public bool TurnBasedMode { get; private set; } = false;
        public Viewport Viewport { get; set; }
        public Vector2 Cursor { get; set; }

        MainCore(Viewport view) {
            System = new StarSystem();
            Viewport = view;
        }
        public void AddPlanets() {
            Instance.System.CreatePlanets(world);
        }

        void CreatePlayers() {
            PlayerController.Clear();
            Player p1 = new Player(new Ship(world, GameObject.GetNewLocation(null), 1), 1);
            PlayerController.AddPlayer(p1);
            Player p2 = new Player(new Ship(world, GameObject.GetNewLocation(null), 2), 2);
            PlayerController.AddPlayer(p2);

            ships.Add(p1.Ship);
            ships.Add(p2.Ship);

            for(int i = 0; i < 6; i++) {
                var ship = new Ship(world, GameObject.GetNewLocation(null), i % 2 == 0 ? 1 : 2);
                AIShipsController.AddController(new DefaultAutoControl(ship));
                ships.Add(ship);
            }

        }
        IEnumerable<GameObject> GetAllObjects(bool all = true) {
            return System.Objects(all);
            //foreach(GameObject obj in System.Objects) {
            //    yield return obj;
            //}
            //if(types == 0 || types == 3)
            //    foreach(Ship s in ships) yield return s;
        }
        void CleanObjects() {
            System.CleanObjects();
            ships.RemoveAll(s => s.ToRemove);
        }

        public void Step(GameTime gameTime) {
            //&& Controller.Keys.ToList().Contains(32)
            if(CurrentState.InGame) {

                if((ships.Count(s => s.Fraction == 1) == 0 || ships.Count(s => s.Fraction == 2) == 0)) {
                    //foreach(Ship ship in ships)
                    //    ship.ToRemove = true;
                    //AIShipsController.Controllers.Clear();
                    CreatePlayers();
                }
                CleanObjects();

                //if(turnIsActive || !TurnBasedMode) {
                //   PlayerController.Step();
                try {
                    world.Step(Math.Min((float)gameTime.ElapsedGameTime.TotalSeconds, (1f / 30f)));
                }
                catch(Exception e) {
                    Debugger.Lines.Add(e.Message);
                }

                AIShipsController.Step();

                foreach(GameObject obj in Objects)
                    obj.Step();

                if(CursorPressed && HookedObject != null)
                    HookedObject.ApplyForce((Cursor - HookedObject.Location) * HookedObject.Mass * HookedObject.Mass);

                //CollideController.Step(ships, GetAllObjects(1));

                //if(TurnBasedMode) {
                //    turnTime++;
                //    if(turnTime == TurnLong) {
                //        turnTime = 0;
                //        turnIsActive = false;
                //    }
                //}
                //}
                UpdateViewport();
            }
        }

        void UpdateViewport() {

            //Viewport.Centerpoint = PlayerController.Players[0].Ship.Location;
            //Viewport.Scale = 1.1f;
            //return;

            float left = float.MaxValue;
            float right = float.MinValue;
            float top = float.MaxValue;
            float bottom = float.MinValue;

            //Vector2 total = new Vector2();

            var viewed = Objects.Where(o => o is SpaceBody || o is Ship);
            //var viewed = PlayerController.Players.Select(p => p.Ship);

            foreach(var obj in viewed) {
                if(obj.Location.X + obj.ObjectBounds.Width > right)
                    right = obj.Location.X + obj.ObjectBounds.Width;
                if(obj.Location.X - obj.ObjectBounds.Width < left)
                    left = obj.Location.X - obj.ObjectBounds.Width;
                if(obj.Location.Y - obj.ObjectBounds.Height < top)
                    top = obj.Location.Y - obj.ObjectBounds.Height;
                if(obj.Location.Y + obj.ObjectBounds.Height > bottom)
                    bottom = obj.Location.Y + obj.ObjectBounds.Height;
            }
            //Cursor = total / Objects.Count;
            //var center = total / Objects.Count;
            // Viewport.Centerpoint = center;
            //Viewport.Centerpoint = new CoordPoint(left + (right - left) / 2, bottom + (top - bottom) / 2);

            Viewport.SetWorldBounds(left, top, right, bottom, 10);
            //Viewport.Scale = Math.Max(right - left, top - bottom) / 300;
            Viewport.Centerpoint = PlayerController.Players[0].Ship.Location;
            Viewport.Update();
        }

        public static void Initialize(Viewport view) {
            instance = new MainCore(view);
        }

        static Random rnd = new Random();
        List<Ship> ships = new List<Ship>();

        //static int pressCoolDown = 0;
        //public static void Pressed(CoordPoint coordPoint) {
        //    if(pressCoolDown < 0)
        //        pressCoolDown++;
        //    else {
        //        pressCoolDown = -10;
        //        // var ship = new Ship(instance.ships[0], instance.StarSystems[0]);
        //        //ship.Position = coordPoint;
        //        //instance.ships.Add(ship);
        //    }
        //}
        //const int TurnLong = 100;
        //int turnTime = 0;
        //bool turnIsActive = false;

        //public void NextTurn() {
        //    turnIsActive = true;
        //}

        //public void Pause() {
        //    TurnBasedMode = !TurnBasedMode;
        //}
    }
}

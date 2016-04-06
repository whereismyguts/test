﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace GameCore {
    public class Ship: GameObject {
        float acceleration = 0;
        float accselerationUp;
        float accselerationDown;
        float accelerationMax = 0.7f;
        float angleSpeed = 0;
        GameObject targetObject;
        CoordPoint velosity = new CoordPoint();
        CoordPoint direction = new CoordPoint(1, 0);
        static Random r = new Random();
        WeaponBase weapon;
        AIController controller;

        public WeaponBase Weapon { get { return weapon; } }
        public ColorCore Color { get; }
        public bool IsBot { get { return controller != null; } }
        public GameObject TargetObject { get { return IsBot ? targetObject : this; } }
        public CoordPoint Direction { get { return direction; } }
        public CoordPoint Velosity { get { return velosity; } }
        public CoordPoint Reactive { get { return -(direction * acceleration) * 50; } }

        protected internal override Bounds Bounds {
            get {
                return new Bounds(Location - new CoordPoint(5, 5), Location + new CoordPoint(5, 5));
            }
        }
        protected internal override string ContentString {
            get {
                return "ship1";
            }
        }

        public Ship(CoordPoint location, GameObject target, StarSystem system) : base(system) {
            Location = location;
            Mass = 1;
            Color = new ColorCore(r.Next(100, 255), r.Next(100, 255), r.Next(100, 255));
            this.targetObject = target;
            weapon = new DefaultCannon();
            controller = new AIController(this, target, TaskType.Peersuit);
            accselerationUp = .1f;
            accselerationDown = accselerationUp / 3f;
        }

        public void AccselerateEngine() {
            if(acceleration + accselerationUp <= accelerationMax)
                acceleration = acceleration + accselerationUp;
            else
                acceleration = accelerationMax;
        }
        public void LowEngine() {
            if(acceleration - accselerationDown >= 0)
                acceleration = acceleration - accselerationDown;
            else
                acceleration = 0;
        }
        public void RotateL() {
            angleSpeed -= .01f;
        }
        public void RotateR() {
            angleSpeed += .01f;
        }

        CoordPoint GetSummaryAttractingForce() {
            var vector = new CoordPoint();
            foreach(var obj in CurrentSystem.Objects)
             //   if(!Bounds.isIntersect(obj.Bounds))
                    vector += PhysicsHelper.GravitationForceVector(this, obj);

            return vector;
        }
        protected internal override float GetRotation() {
            return (float)(Direction.Angle);
        }
        protected internal override void Step() {
            foreach(Body obj in CurrentSystem.Objects)
                if(CoordPoint.Distance(obj.Location, Location) <= obj.Radius)
                    Death();


            if(IsBot) {
                List<Action> actions = controller.Step();
                foreach(Action a in actions)
                    a();
            }



            velosity += Direction * acceleration + GetSummaryAttractingForce();
            Location += velosity;
            direction.Rotate(angleSpeed);
            angleSpeed *= PhysicsHelper.RotationInertia;



            LowEngine();
        }

        void Death() {
            Location = new CoordPoint(-101000, 101000);
            acceleration = 0;
            direction = new CoordPoint(1, 0);
            velosity = new CoordPoint();
        }

        public void SwitchAI() {
            if(controller != null)
                controller = null;
            else controller = new AIController(this, CurrentSystem.Objects[2], TaskType.Peersuit);
        }
    }

    public struct ColorCore {
        public int b;
        public int g;
        public int r;
        public ColorCore(int r, int g, int b) {
            this.r = r;
            this.g = g;
            this.b = b;
        }
    }
}

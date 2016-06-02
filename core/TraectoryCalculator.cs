﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameCore {
    class VirtualObject: GameObject {
        #region implement
        public override Bounds ObjectBounds { get { throw new NotImplementedException(); } }
        protected internal override float Rotation { get { throw new NotImplementedException(); } }
        internal override bool IsMinimapVisible { get { throw new NotImplementedException(); } }
        public override IEnumerable<Item> GetItems() { throw new NotImplementedException(); }
        #endregion
        public bool IsDead = false;
        public VirtualObject(StarSystem system, float mass, CoordPoint position, CoordPoint velosity) : base(system) {
            Position = position;
            Mass = mass;
            Velosity = velosity;
        }
        protected internal override void Step() {
            Velosity += PhysicsHelper.GetSummaryAttractingForce(CurrentSystem.Objects, this);
            foreach(Body b in CurrentSystem.Objects)
                if(CoordPoint.Distance(b.Position, Position) <= b.Radius)
                    IsDead = true;
            base.Step();
        }

    }
    public class TrajectoryCalculator {
        VirtualObject virtObj;
        GameObject realObj;
        List<CoordPoint> result = new List<CoordPoint>();
        public TrajectoryCalculator(GameObject obj) {
            this.virtObj = new VirtualObject(obj.CurrentSystem, obj.Mass, obj.Position, obj.Velosity);
            realObj = obj;
        }
        public List<CoordPoint> Calculate() {
          
             result.Clear();
            virtObj.IsDead = false;
            result.Add(virtObj.Position);
            for(int i = 0; i < 500; i++) {
                for(int j = 0; j < 9; j++) {
                    virtObj.Step();
                    if(virtObj.IsDead)
                        return result;
                }
                result.Add(virtObj.Position.Clone());
            }
            return result;
        }
        internal void Update() {
            virtObj.Position = realObj.Position;
            virtObj.Velosity = realObj.Velosity;
        }
    }
}

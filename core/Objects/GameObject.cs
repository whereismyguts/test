﻿using System;

namespace GameCore {
    public abstract class GameObject {
        public StarSystem CurrentSystem { get; }
        protected string Image { get; set; }
        protected internal bool IsVisible {
            get {
                return true;
            }
        }
        protected Viewport Viewport { get { return Core.Instance.Viewport; } }
        internal CoordPoint Location { get; set; }
        protected internal abstract Bounds Bounds { get; }
        protected internal abstract string ContentString { get; }
        protected internal float Mass { get; set; }
        public virtual string Name { get { return ""; } }

        public GameObject(StarSystem system) {
            CurrentSystem = system;
        }

        protected internal abstract float GetRotation();
        protected internal abstract void Step();

        public Bounds GetScreenBounds() {
            return new Bounds(
                Viewport.World2Screen(Bounds.LeftTop),
                Viewport.World2Screen(Bounds.RightBottom) );
            //var centerPoint = (Bounds.Center - Viewport.Bounds.Center) / Viewport.Scale;
            //var scaleVector = new CoordPoint(Bounds.Width, Bounds.Height) / Viewport.Scale;
            //return new Bounds(centerPoint - scaleVector, centerPoint + scaleVector) + new CoordPoint(Viewport.PxlWidth, Viewport.PxlHeight) / 2;
        }
        public Bounds GetMiniMapBounds() {
            return new Bounds(
                Viewport.World2Screen(Bounds.LeftTop),
                Viewport.World2Screen(Bounds.RightBottom));
            //var centerPoint = (Bounds.Center - Viewport.Bounds.Center) / Viewport.MiniMapScale;
            //var scaleVector = new CoordPoint(Bounds.Width, Bounds.Height) / Viewport.MiniMapScale;
            //return new Bounds(centerPoint - scaleVector, centerPoint + scaleVector) + new CoordPoint(Viewport.PxlWidth, Viewport.PxlHeight) / 2;
        }
    }
}

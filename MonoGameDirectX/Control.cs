﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GameCore;

namespace MonoGameDirectX {
    public class Control: InteractiveObject {
        protected Color BorderColor { get; }
        protected Color FillColor { get; }
        protected SpriteFont Font { get; }

        public virtual Color ActualBorderColor { get { return BorderColor; } }
        public virtual Color ActualFillColor { get { return FillColor; } }
        public Rectangle Rectangle { get; protected set; }

        public Control(Rectangle rect, SpriteFont font) {
            Rectangle = rect;
            FillColor = Color.Transparent;
            BorderColor = Color.Black;
            Font = font;
        }

        internal virtual void Draw( SpriteBatch spriteBatch, GameTime time) {
            DrawPrimitives.DrawRect(Rectangle, spriteBatch, 1, ActualBorderColor, ActualFillColor);
        }

        public override bool Contains(object position) {
            return Rectangle.Contains((Point)position);
        }
    }
    public class Label: Control {
        public string Text { get; set; }
        public Color TextColor { get; internal set; }

        public Label(int x, int y, int w, int h, string text, SpriteFont font) : base(new Rectangle(x, y, w, h), font) {
            Text = text;
            TextColor = Color.Black;
        }

        internal override void Draw( SpriteBatch spriteBatch, GameTime time) {
            base.Draw(spriteBatch, time);
            Vector2 textSize = Font.MeasureString(Text);
            Vector2 panSize = Rectangle.Size.ToVector2();
            Vector2 textLocation = Rectangle.Location.ToVector2() + (panSize - textSize) / 2;
            spriteBatch.DrawString(Font, Text, textLocation, TextColor);
        }
    }
    public class Button: Label {
        public override Color ActualBorderColor {
            get {
                return IsSelected ? Color.AntiqueWhite : IsHighlighted ? Color.FloralWhite : BorderColor;
            }
        }
        public override Color ActualFillColor {
            get {
                return IsSelected ? Color.DarkGray : IsHighlighted ? Color.LightGray : FillColor;
            }
        }
        public object Tag { get; set; }
        public Button(int x, int y, int w, int h, string text, SpriteFont font) : base(x, y, w, h, text, font) {
        }

        public event EventHandler ButtonClick;

        protected override void HandleMouseClick(object position) {
            ButtonClick?.Invoke(this, EventArgs.Empty);
            base.HandleMouseClick(position);
        }
    }
    public class ListBox: Control {
        List<Button> buttons = new List<Button>();
        Point location;
        public override bool IsHighlighted {
            get {
                return base.IsHighlighted;
            }

            set {
                if(!value)
                    for(int i = 0; i < buttons.Count; i++)
                        buttons[i].IsHighlighted = false;
                base.IsHighlighted = value;
            }
        }

        public ListBox(Point location, SpriteFont font, params object[] objects) : base(new Rectangle(), font) {

            this.location = location;
            Update(objects);
            this.Click += ListBox_Click;


        }

        public event EventHandler ItemClick;

        private void ListBox_Click(object position, EventArgs e) {
            for(int i = 0; i < buttons.Count; i++)
                if(buttons[i].Contains((Point)position))
                    if(ItemClick != null)
                        ItemClick(buttons[i], EventArgs.Empty);
        }

        internal void Update(object[] objects) {
            int w = 0;
            int h = 0;
            int hStep = 0;
            for(int i = 0; i < objects.Length; i++) {
                Vector2 size = Font.MeasureString(objects[i].ToString());
                w = Math.Max((int)size.X + 10, w);
                hStep = Math.Max((int)size.Y + 10, hStep);
                h += (int)size.Y + 10;
                Rectangle = new Rectangle(location, new Point(w, h));
            }
            buttons.Clear();
            for(int i = 0; i < objects.Length; i++) {
                Button b = new Button(location.X, location.Y + hStep * i, w, hStep, objects[i].ToString(), Font) { Tag = objects[i]};
                buttons.Add(b);
            }
        }

        protected override void HandleMouseHover(object position) {
            for(int i = 0; i < buttons.Count; i++)
                buttons[i].IsHighlighted = buttons[i].Contains((Point)position) && IsHighlighted;

        }

        internal override void Draw( SpriteBatch spriteBatch, GameTime time) {
            base.Draw( spriteBatch, time);
            for(int i = 0; i < buttons.Count; i++)
                buttons[i].Draw( spriteBatch,  time);
        }

        public override bool Contains(object position) {
            for(int i = 0; i < buttons.Count; i++)
                if(buttons[i].Contains((Point)position))
                    return true;
            return false;
        }
    }
    public class ImageBox: Control {
        public ImageBox(Rectangle rect) : base(rect, null) {
            sprite = new Sprite(new SpriteInfo(), rect);
            
        }
        Sprite sprite;
        public void SetImage(SpriteInfo info) {
            sprite = new Sprite(info, Rectangle);
        }
        internal override void Draw( SpriteBatch spriteBatch, GameTime time) {
            base.Draw( spriteBatch, time);
            sprite.Draw(spriteBatch, time, true);
        }
    }
}

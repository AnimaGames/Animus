using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using MonoGame.Extended.Input;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using System.Collections.Generic;

namespace AnimusEngine
{
    public class Player : GameObject
    {
        public Player()
        {
        }

        public Player(Vector2 initPosition)
        {
            position = initPosition;
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void Load(ContentManager content)
        {
            objectTexture = content.Load<Texture2D>("player");
            base.Load(content);
        }

        public override void Update(List<GameObject> _objects)
        {
            CheckInput();
            base.Update(_objects);
        }

        private void CheckInput()
        {
            var keyboardState = KeyboardExtended.GetState();
            var speed = 1f;

            if (keyboardState.IsKeyDown(Keys.Left))
            {
                position.X -= speed;
            }

            if (keyboardState.IsKeyDown(Keys.Right))
            {
                position.X += speed;
            }

            if (keyboardState.IsKeyDown(Keys.Up))
            {
                position.Y -= speed;
            }

            if (keyboardState.IsKeyDown(Keys.Down))
            {
                position.Y += speed;
            }
            
        }
    }
}

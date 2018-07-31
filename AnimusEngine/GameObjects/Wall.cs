using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using System.Collections.Generic;

namespace AnimusEngine
{
    public class Wall
    {
        public Rectangle wall;
        public bool active = true;

        public Wall()
        { }

        public Wall(Rectangle initPosition)
        {
            wall = initPosition;
        }
    }

    public class Map
    {
        public List<Wall> walls = new List<Wall>();

        public int tileSize = 16;

        public Rectangle CheckCollisions(Rectangle init)
        {
            for (int i = 0; i < walls.Count; i++)
            {
                if (walls[i] != null && walls[i].wall.Intersects(init))
                {
                    return walls[i].wall;
                }
            }
            return Rectangle.Empty;
        }
    }
}

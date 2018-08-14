using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;

namespace AnimusEngine
{
    public class Wall
    {
        public Rectangle wall;
        public bool active = true;
        public bool solid = true;

        public Wall()
        { }

        public Wall(Rectangle initPosition)
        {
            wall = initPosition;
        }
    }

    public class Door
    {
        public Rectangle door;
        public bool solid;
        public bool active = true;
        public string nextRoomNumber;
        static public bool doorEnter;

        public Door()
        { }

        public Door(Rectangle initPosition, string inputRoomNumber)
        {
            door = initPosition;
            nextRoomNumber = inputRoomNumber;
        }
    }

    public class Map
    {
        public List<Wall> walls = new List<Wall>();
        public List<Door> doors = new List<Door>();
        public List<Door> rooms = new List<Door>();

        Texture2D wallTexture;
        Texture2D doorTexture;
        Texture2D pauseTexture;

        public int tileSize = 16;

        //screen trans
        static public string screenDir;

        public void Load(ContentManager content)
        {
            wallTexture = content.Load<Texture2D>("Sprites/pixel");
            doorTexture = content.Load<Texture2D>("Sprites/pixel");
            pauseTexture = content.Load<Texture2D>("Sprites/pixel");
        }

        public void DrawWalls(SpriteBatch _spriteBatch)
        {
            for (int i = 0; i < walls.Count; i++)
            {
                if (walls[i] != null && walls[i].active)
                {
                    //_spriteBatch.Draw(wallTexture, 
                                      //new Vector2(walls[i].wall.X, walls[i].wall.Y), 
                                      //walls[i].wall, 
                                      //Color.Black, 
                                      //0f, 
                                      //Vector2.Zero, 
                                      //1f, 
                                      //SpriteEffects.None, 
                                      //0.5f);
                }
            }
            for (int i = 0; i < doors.Count; i++)
            {
                if (doors[i] != null && doors[i].active)
                {
                    _spriteBatch.Draw(doorTexture, 
                                      new Vector2(doors[i].door.X, doors[i].door.Y), 
                                      doors[i].door, 
                                      Color.Red, 
                                      0f, 
                                      Vector2.Zero, 
                                      1f, 
                                      SpriteEffects.None, 
                                      0.5f);
                }
            }
            
        }

        public Rectangle CheckCollision(Rectangle init)
        {
            for (int i = 0; i < walls.Count; i++)
            {
                if (walls[i] != null && walls[i].wall.Intersects(init))
                {
                    return walls[i].wall;
                }
            }
            for (int i = 0; i < doors.Count; i++)
            {
                if (doors[i] != null && doors[i].door.Intersects(init))
                {
                    Door.doorEnter = true;
                    Entity.applyGravity = false;
                    if (init.Left < doors[i].door.Left)
                    {
                        screenDir = "right";
                    }
                    else if (init.Left > doors[i].door.Left)
                    {
                        screenDir = "left";
                    }
                    Screens.roomPlaceHolder = doors[i].nextRoomNumber;
                    return doors[i].door;
                }
            }
            return Rectangle.Empty;
        }
    }
}

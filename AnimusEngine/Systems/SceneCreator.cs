using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Renderers;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Content;
using System;

namespace AnimusEngine
{
    public class SceneCreator
    {
        //map items
        public Map map = new Map();
        public TiledMap _map;
        public TiledMapRenderer _renderer;
        public TiledMapObjectLayer _objectLayer { get; private set; } = null;

        //cleanup items
        public List<GameObject> _killObjects = new List<GameObject>();
        public List<Door> _killDoors = new List<Door>();
        public List<Wall> _killWalls = new List<Wall>();
        int willKillPlayer;

        public HUD playerHUD = new HUD();
        public static SoundEffect bgMusic;
        public string levelNumberHolder;
        public static SoundEffectInstance soundEffectInstance;


        public SceneCreator()
        {
        }

        public void LevelLoader(ContentManager content, 
                                GraphicsDevice graphics, 
                                List<GameObject> _objects,  
                                string levelNumber, 
                                string roomNumber, 
                                bool restartMusic)
        {
            
            bgMusic = content.Load<SoundEffect>("Audio/Music/Level_" + levelNumber);
            Console.WriteLine(bgMusic.Name);

            if (soundEffectInstance != null && levelNumber != levelNumberHolder)
            {
                soundEffectInstance.Stop(true);
                soundEffectInstance.Dispose();
            }
            
            if (restartMusic)
            {
                soundEffectInstance = bgMusic.CreateInstance();
                soundEffectInstance.Play();
            }

            soundEffectInstance.IsLooped |= levelNumber != "GameOver";

            levelNumberHolder = levelNumber;

            map.Load(content);
            _map = content.Load<TiledMap>("Maps/Level_" + levelNumber);
            _renderer = new TiledMapRenderer(graphics, _map);

            foreach (var tileLayer in _map.TileLayers)
            {
                for (var x = 0; x < tileLayer.Width; x++)
                {
                    for (var y = 0; y < tileLayer.Height; y++)
                    {
                        var tile = tileLayer.GetTile((ushort)x, (ushort)y);

                        if (tile.GlobalIdentifier == 16)
                        {
                            var tileWidth = _map.TileWidth;
                            var tileHeight = _map.TileHeight;
                            map.walls.Add(new Wall(new Rectangle(x * tileWidth, y * tileHeight, tileWidth, tileHeight)));
                        }
                    }
                }
            }
            //parsing in objects from object layer
            _objectLayer = _map.GetLayer<TiledMapObjectLayer>("Room" + roomNumber);

            for (int i = 0; i < _objectLayer.Objects.Length; i++)
            {
                if (_objectLayer.Objects[i].Type == "camera")
                {
                    //set camera max and min per room
                    if (_objectLayer.Objects[i].Name == "cMin" + roomNumber)
                    {
                        Camera.cameraMin = _objectLayer.Objects[i].Position + Camera.cameraOffset;
                    }
                    if (_objectLayer.Objects[i].Name == "cMax" + roomNumber)
                    {
                        Camera.cameraMax = _objectLayer.Objects[i].Position - Camera.cameraOffset;
                    }
                }
                if (_objectLayer.Objects[i].Type == "player" && _objects.Count == 0)
                {
                    //create player if none exists
                    if (_objectLayer.Objects[i].Name == "playerStart")
                    {
                        _objects.Add(new Player(_objectLayer.Objects[i].Position));
                        _objects[0].Initialize();
                        _objects[0].Load(content);
                    }
                }
                //create doors
                if (_objectLayer.Objects[i].Type == "door")
                {
                    map.doors.Add(new Door(new Rectangle((int)_objectLayer.Objects[i].Position.X,
                                                         (int)_objectLayer.Objects[i].Position.Y,
                                                         (int)_objectLayer.Objects[i].Size.Width,
                                                         (int)_objectLayer.Objects[i].Size.Height),
                                           _objectLayer.Objects[i].Name));
                }

                //create enemies
                if (_objectLayer.Objects[i].Type == "enemy")
                {
                    _objects.Add(EnemyLookUp.EnemyLUT(_objectLayer.Objects[i].Name, _objectLayer.Objects[i].Position));
                }

                //create npcs
                if (_objectLayer.Objects[i].Type == "npc")
                {
                    _objects.Add(NPCLookUp.NpcLUT(_objectLayer.Objects[i].Name, _objectLayer.Objects[i].Position));
                }

                //create map objects
                if (_objectLayer.Objects[i].Type == "object")
                {
                    _objects.Add(MapObjectLookUp.MapObjLUT(_objectLayer.Objects[i].Name,
                                                           new Rectangle((int)_objectLayer.Objects[i].Position.X,
                                                                         (int)_objectLayer.Objects[i].Position.Y,
                                                                         (int)_objectLayer.Objects[i].Size.Width,
                                                                         (int)_objectLayer.Objects[i].Size.Height)));

                }
            }
            if (_objects.Count > 0)
            {
                playerHUD = new HUD();
            }
            playerHUD.Load(content);
            LoadObjects(content, _objects);
        }

        public void UnloadObjects(bool killPlayer, List<GameObject> _objects)
        {
            Game1.currentLevel = Game1.levelNumber;

            if (killPlayer == true)
            {
                willKillPlayer = 0;
            }
            else
            {
                willKillPlayer = 1;
            }

            for (int i = willKillPlayer; i < _objects.Count; i++)
            {
                _killObjects.Add(_objects[i]);
            }
            foreach (var doors in map.doors)
            {
                _killDoors.Add(doors);
            }
            foreach (var walls in map.walls)
            {
                _killWalls.Add(walls);
            }
            //detroy old objects --- there must be a smarter way to do this....
            foreach (GameObject o in _killObjects)
            {
                _objects.Remove(o);
            }
            foreach (Door d in _killDoors)
            {
                map.doors.Remove(d);
            }
            foreach (Wall w in _killWalls)
            {
                map.walls.Remove(w);
            }
        }

        public void LoadObjects(ContentManager content, List<GameObject> _objects)
        {
            PauseMenu.Load(content);
            for (int i = 1; i < _objects.Count; i++)
            {
                _objects[i].Initialize();
                _objects[i].Load(content);
            }
        }
    }
}

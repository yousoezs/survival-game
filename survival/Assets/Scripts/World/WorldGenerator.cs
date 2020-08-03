﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.World
{
    public static class WorldGenerator
    {
        public static bool NewWorld = false;
        public static string Size = "Small";
        public static string Name;
        public static List<GameObject> ChunkObjects;
        public static global::World world;

        public static Chunk[,] chunks;

        static WorldGenerator()
        {
            world = new global::World();
        }

        private static int GetWorldSize(string size)
        {
            switch(size)
            {
                case "Small":
                    return 10;
                case "Medium":
                    return 20;
                case "Large":
                    return 30;
                default:
                    return 0;
            }
        }

        public static void CreateWorld()
        {
            CreateChunks();
            PopulateChunks();
            SaveWorld();
        }

        public static void CreateChunks()
        {
            int chunkWidth = 32, chunkHeight = 32;
            chunks = new Chunk[GetWorldSize(Size), GetWorldSize(Size)];
            for(int i = 0; i < chunks.GetLength(0); i++)
            {
                for(int j = 0; j < chunks.GetLength(1); j++)
                {
                    chunks[i, j] = new Chunk(chunkWidth, chunkHeight, i * 4, j * -4);
                }
            }
        }

        public static void PopulateChunks()
        {
            for (int i = 0; i < chunks.GetLength(1); i++)
            {
                for (int j = 0; j < chunks.GetLength(0); j++)
                {
                    if(i < 4)
                    {
                        chunks[j, i].AddAir();
                    }
                    if(i == 4)
                    {
                        chunks[j, i].AddGroundBlocks();
                    }
                    if(i > 4)
                    {
                        chunks[j, i].AddUndergroundBlocks();
                    }
                }
            }
        }

        public static void AddBlock(Vector3 mousePosition, int blockType)
        {
            for(int i = 0; i < chunks.GetLength(0); i++)
            {
                for(int j = 0; j < chunks.GetLength(1); j++)
                {
                    if(chunks[i, j].x + 4 > mousePosition.x &&
                        chunks[i, j].x < mousePosition.x &&
                        chunks[i, j].y + 4 > mousePosition.y &&
                        chunks[i, j].y < mousePosition.y)
                    {
                        var x = ((Math.Round(mousePosition.x * 8, MidpointRounding.AwayFromZero) / 8 - chunks[i, j].x) * 8) == 32 ? 0 : (Math.Round(mousePosition.x * 8, MidpointRounding.AwayFromZero) / 8 - chunks[i, j].x) * 8;
                        var y = ((Math.Round(mousePosition.y * 8, MidpointRounding.AwayFromZero) / 8 - chunks[i, j].y) *8) == 32 ? 0 : (Math.Round(mousePosition.y * 8, MidpointRounding.AwayFromZero) / 8 - chunks[i, j].y) * 8;

                        chunks[i, j].blocks[(int)x, (int)y] = blockType + 1;
                    }
                }
            }
        }

        public static void RemoveBlock(Vector3 mousePosition)
        {
            for (int i = 0; i < chunks.GetLength(0); i++)
            {
                for (int j = 0; j < chunks.GetLength(1); j++)
                {
                    if (chunks[i, j].x + 4 > mousePosition.x &&
                        chunks[i, j].x < mousePosition.x &&
                        chunks[i, j].y + 4 > mousePosition.y &&
                        chunks[i, j].y < mousePosition.y)
                    {
                        var x = ((Math.Round(mousePosition.x * 8, MidpointRounding.AwayFromZero) / 8 - chunks[i, j].x) * 8) == 32 ? 0 : (Math.Round(mousePosition.x * 8, MidpointRounding.AwayFromZero) / 8 - chunks[i, j].x) * 8;
                        var y = ((Math.Round(mousePosition.y * 8, MidpointRounding.AwayFromZero) / 8 - chunks[i, j].y) * 8) == 32 ? 0 : (Math.Round(mousePosition.y * 8, MidpointRounding.AwayFromZero) / 8 - chunks[i, j].y) * 8;

                        chunks[i, j].blocks[(int)x, (int)y] = 0;
                    }
                }
            }
        }

        public static void SaveWorld()
        {
            world.Id = SaveLoadManager.GetWorlds().Count;
            world.Name = Name;
            world.Size = Size;
            world.Chunks = chunks;
            world.PlayerPositionX = 0;
            world.PlayerPositionY = 0;

            SaveLoadManager.SaveWorld(world);
        }
    }
}

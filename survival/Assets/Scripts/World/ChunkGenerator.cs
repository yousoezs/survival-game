﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkGenerator : MonoBehaviour
{
    public GameObject DirtPrefab;
    public GameObject GrassPrefab;
    public GameObject StonePrefab;
    public GameObject CoalPrefab;
    public GameObject IronPrefab;

    public int width;

    //public int minX = 0;
    //public int maxX = 32;
    //public int minY = 0;
    //public int maxY = 32;

    public int minX = -16;
    public int maxX = 16;
    public int minY = -16;
    public int maxY = 16;

    public PerlinNoise noise;

    public List<GameObject> Blocks;

    void Start()
    {
        Blocks = new List<GameObject>();
        //CreateWorld();
        Generate();
    }

    //private void CreateWorld()
    //{
    //    for(int i = minX; i < maxX; i++)
    //    {
    //        int columnHeight = noise.GetNoise(i - minX, maxY - minY);
    //        for (int j = minY; j < minY + columnHeight; j++)
    //        {
    //            // 0 = Air, 1 = Grass, 2 = Dirt, 3 = Stone, 4 = Coal, 5 = Iron
    //            int block = (j == minY + columnHeight - 1) ? 1 : 2;

    //            if (j < minY + columnHeight - 6)
    //            {
    //                if (Random.Range(0, 200) <= 0)
    //                {
    //                    if (Random.Range(0, 5) <= 0)
    //                    {
    //                        block = 5;
    //                    }
    //                    else
    //                    {
    //                        block = 4;

    //                    }
    //                }
    //                else
    //                    block = 3;
    //            }
    //            GameObject newTile = Instantiate(GetBlockPrefab(block), Vector2.zero, Quaternion.identity);
    //            newTile.transform.parent = this.gameObject.transform;
    //            newTile.transform.localPosition = new Vector2(i * 0.125f, j * 0.125f);
    //            Debug.Log(i);
    //            Debug.Log(j);
    //            Blocks[i, j] = block;
    //        }
    //    }
    //}

    private GameObject GetBlockPrefab(int block)
    {
        switch(block)
        {
            case 1:
                return GrassPrefab;
            case 2:
                return DirtPrefab;
            case 3:
                return StonePrefab;
            case 4:
                return CoalPrefab;
            case 5:
                return IronPrefab;
            default:
                return null;
        }
    }

    private void Generate()
    {
        //Columns X
        for (int i = minX; i < maxX; i++)
        {
            //2 + noise
            int columnHeight = noise.GetNoise(i - minX, maxY - minY);
            //Rows Y
            for (int j = minY; j < minY + columnHeight; j++)
            {
                GameObject block = (j == minY + columnHeight - 1) ? GrassPrefab : DirtPrefab;

                if (j < minY + columnHeight - 6)
                {
                    if (Random.Range(0, 200) <= 0)
                    {
                        if (Random.Range(0, 5) <= 0)
                        {
                            block = IronPrefab;
                        }
                        else
                        {
                            block = CoalPrefab;

                        }
                    }
                    else
                        block = StonePrefab;
                }
                GameObject newTile = Instantiate(block, Vector2.zero, Quaternion.identity);
                newTile.transform.parent = this.gameObject.transform;
                newTile.transform.localPosition = new Vector2(i * 0.125f, j * 0.125f);
                Blocks.Add(newTile);
            }
        }
    }
}
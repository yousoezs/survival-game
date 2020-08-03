﻿using Assets.Scripts;
using Assets.Scripts.World;
using Assets.Sprites.Inventory;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public float Health = 100f;
    public float Hunger = 100f;
    public float Thirst = 100f;
    public float Radiation = 100f;
    public float Cold = 100f;
    public float Warm = 100f;

    public Image HealthBar;
    public Image HungerhBar;
    public Image ThirstBar;
    public Image RadiationBar;
    public Image WarmBar;
    public Image ColdBar;

    public float movementSpeed = 5f;
    public float jumpHeight = 5f;

    public Transform groundCheck;
    public float groundCheckRadius;
    public LayerMask whatIsGround;
    private bool grounded;

    private Rigidbody2D rigidbody;
    private Animator anim;

    public float YVelocity = 0.0f;

    public Inventory inventory;
    public Hotbar hotbar;
    public InventoryHandler inventoryHandler;
    private ChunksHandler chunksHandler;

    private bool male;
    private Color32[] skinColors;
    private Color32[] hairColors;
    public Sprite[] Hairs;
    private Vector2[] HairPositions;

    public GameObject bullet;

    private Item equippedItem;

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Item")
        {
            inventory.AddItem(other.gameObject.GetComponent<Item>());
            //Destroy(other.gameObject);
        }

        if (other.gameObject.tag == "Ground" && other.relativeVelocity.y > 10)
        {
            Health -= other.relativeVelocity.y * rigidbody.gravityScale * 1.5f;
        }
    }

    void Start()
    {
        inventoryHandler = new InventoryHandler();
        gameObject.transform.position = new Vector3(WorldGenerator.world.PlayerPositionX, WorldGenerator.world.PlayerPositionY, 0);
        chunksHandler = FindObjectOfType<ChunksHandler>();
        rigidbody = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        anim.SetBool("Male", PlayerHandler.Body == 0 ? true : false);
        hairColors = new Color32[]
        {
            new Color32(255, 255, 255, 255),
            new Color32(255,215,0, 255),
            new Color32(184,134,11, 255),
            new Color32(128,128,0, 255),
            new Color32(0,128,0, 255),
            new Color32(70,130,180, 255),
            new Color32(0,0,255, 255),
            new Color32(147,112,219, 255),
            new Color32(128,0,128, 255),
            new Color32(165,42,42, 255),
            new Color32(255,0,0, 255),
            new Color32(139,69,19, 255),
            new Color32(205,133,63, 255),
            new Color32(128,128,128, 255),
            new Color32(0, 0, 0, 255)
        };
        skinColors = new Color32[] {
            new Color32(254, 253, 253, 255),
            new Color32(245, 234, 223, 255),
            new Color32(235, 214, 193, 255),
            new Color32(220, 185, 149, 255),
            new Color32(206, 155, 105, 255),
            new Color32(190, 126, 62, 255),
            new Color32(160, 106, 52, 255),
            new Color32(131, 87, 43, 255),
            new Color32(101, 67, 33, 255),
            new Color32(86, 57, 28, 255),
            new Color32(71, 47, 23, 255),
            new Color32(57, 38, 19, 255)
        };
        HairPositions = new Vector2[]
        {
            new Vector2(0.0108f, 0.3537f),
            new Vector2(0.0206f, 0.3645f),
            new Vector2(-0.01f, 0.375f),
            new Vector2(0.011f, 0.302f),
            new Vector2(0.011f, 0.26f),
            new Vector2(-0.031f, 0.281f),
            new Vector2(-0.052f, 0.364f),
            new Vector2(-0.052f, 0.384f)
        };
        GameObject hair = GameObject.Find("Hair");
        hair.GetComponent<SpriteRenderer>().sprite = Hairs[PlayerHandler.Hair];
        hair.GetComponent<SpriteRenderer>().color = hairColors[PlayerHandler.HairColor];
        hair.GetComponent<Transform>().localPosition = HairPositions[PlayerHandler.Hair];
        GetComponent<SpriteRenderer>().color = skinColors[PlayerHandler.SkinColor];
    }

    void FixedUpdate()
    {
        grounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);
    }

    void Update()
    {
        Movement();
        Jump();
        Save();
        Load();
        RemoveBlock();
        PlaceBlock();
        EquipItem();
        Shoot();
        Throw();
        UpdateStats();
        UseItem();
    }

    private void UseItem()
    {
        if (!hotbar.selectedSlot.GetComponent<Slot>().IsEmpty)
        {
            if (hotbar.selectedSlot.GetComponent<Slot>().CurrentItem.Type == ItemType.Soda)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    Thirst += hotbar.selectedSlot.GetComponent<Slot>().CurrentItem.Thirst;
                    hotbar.selectedSlot.GetComponent<Slot>().UseItem();
                }
            }
        }
    }

    private void UpdateStats()
    {
        HealthBar.GetComponent<RectTransform>().localScale = new Vector3(Health / 100, 1, 1);
        HungerhBar.GetComponent<RectTransform>().localScale = new Vector3(Hunger / 100, 1, 1);
        ThirstBar.GetComponent<RectTransform>().localScale = new Vector3(Thirst / 100, 1, 1);
        RadiationBar.GetComponent<RectTransform>().localScale = new Vector3(Radiation / 100, 1, 1);
        WarmBar.GetComponent<RectTransform>().localScale = new Vector3(Warm / 100, 1, 1);
        ColdBar.GetComponent<RectTransform>().localScale = new Vector3(Cold / 100, 1, 1);
    }

    private void Shoot()
    {
        if (!hotbar.selectedSlot.GetComponent<Slot>().IsEmpty)
        {
            if (hotbar.selectedSlot.GetComponent<Slot>().CurrentItem.Type == ItemType.Gun)
            {
                if(Input.GetMouseButtonDown(0))
                {
                    GameObject b = (GameObject)Instantiate(bullet, GameObject.Find("Equipped Item").transform.position, Quaternion.identity);
                    b.GetComponent<Rigidbody2D>().velocity = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - b.transform.position).normalized * 5;
                    b.GetComponent<Transform>().rotation = GameObject.Find("Equipped Item").transform.rotation;
                }
            }
        }
    }

    private void Throw()
    {
        if (!hotbar.selectedSlot.GetComponent<Slot>().IsEmpty)
        {
            if (hotbar.selectedSlot.GetComponent<Slot>().CurrentItem.Type == ItemType.CreatureCatcher)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    Item b = (Item)Instantiate(hotbar.selectedSlot.GetComponent<Slot>().CurrentItem, GameObject.Find("Equipped Item").transform.position, Quaternion.identity);
                    b.GetComponent<Rigidbody2D>().velocity = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - b.transform.position).normalized * 10;
                    GameObject.Find("Equipped Item").GetComponent<SpriteRenderer>().sprite = null;
                    hotbar.selectedSlot.GetComponent<Slot>().UseItem();
                }
            }
        }
    }

    private void EquipItem()
    {
        if(!hotbar.selectedSlot.GetComponent<Slot>().IsEmpty)
        {
            if(hotbar.selectedSlot.GetComponent<Slot>().CurrentItem.CanBeEquipped) {
                GameObject.Find("Equipped Item").GetComponent<SpriteRenderer>().sprite = hotbar.selectedSlot.GetComponent<Slot>().CurrentItem.Sprite;
                GameObject.Find("Equipped Item").gameObject.transform.localPosition = new Vector3(hotbar.selectedSlot.GetComponent<Slot>().CurrentItem.PositionX, hotbar.selectedSlot.GetComponent<Slot>().CurrentItem.PositionY, 0f);
                //equippedItem = (Item)Instantiate(hotbar.selectedSlot.GetComponent<Slot>().CurrenItem, GameObject.Find("arm").transform.localPosition, Quaternion.identity, GameObject.Find("arm").transform);
                //equippedItem.GetComponent<Rigidbody2D>().simulated = false;
                //equippedItem.GetComponent<Transform>().localRotation = new Quaternion(0, 0, 0, 0);
                //equippedItem.GetComponent<Transform>().localPosition = new Vector3(0.333f, 0f, 0f);
            }
        }
        else
        {
            GameObject.Find("Equipped Item").GetComponent<SpriteRenderer>().sprite = null;
        }
    }

    private void PlaceBlock()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            RaycastHit2D hit2d = Physics2D.Raycast(mousePosition, Vector2.zero);

            if (!hotbar.selectedSlot.GetComponent<Slot>().IsEmpty)
            {
                Instantiate(GameObject.Find("ItemList").GetComponent<ItemsList>().Items[hotbar.selectedSlot.GetComponent<Slot>().Items.Peek().Id], new Vector3((float)Math.Round(mousePosition.x * 8, MidpointRounding.AwayFromZero) / 8, (float)Math.Round(mousePosition.y * 8, MidpointRounding.AwayFromZero) / 8, 0), Quaternion.identity);
                AddBlockInWorld(mousePosition, hotbar.selectedSlot.GetComponent<Slot>().Items.Peek().Id);
                hotbar.selectedSlot.GetComponent<Slot>().UseItem();
            }
        }
    }

    private void RemoveBlock()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            if (mousePosition.x > gameObject.transform.position.x - 1 &&
                mousePosition.x < gameObject.transform.position.x + 1 &&
                mousePosition.y > gameObject.transform.position.y - 1 &&
                mousePosition.y < gameObject.transform.position.y + 1)
            {
                RaycastHit2D hit2d = Physics2D.Raycast(mousePosition, Vector2.zero);

                if (hit2d)
                {
                    if (hit2d.collider.CompareTag("Block"))
                    {
                        Block block = hit2d.collider.gameObject.GetComponent<Block>();
                        BlockAnimationsHandler blockAnimationsHandler = FindObjectOfType<BlockAnimationsHandler>();
                        block.DestroyBlock(blockAnimationsHandler.GetDestroyAnimationForSelectedBlock(block.BlockType));
                        Item blockItem = hit2d.collider.gameObject.GetComponent<Item>();
                        inventory.AddItem(blockItem);
                        RemoveBlockInWorld(mousePosition);
                    }
                }
            }

        }
    }

    private void AddBlockInWorld(Vector3 mousePosition, int blockType)
    {
        WorldGenerator.AddBlock(mousePosition, blockType);
    }

    private void RemoveBlockInWorld(Vector3 mousePosition)
    {
        WorldGenerator.RemoveBlock(mousePosition);
    }

    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && grounded)
        {
            rigidbody.velocity = new Vector2(rigidbody.velocity.x, jumpHeight);
        }
    }

    void Movement()
    {
        if (Input.GetKey(KeyCode.A))
        {
            if (Input.GetKey(KeyCode.LeftShift))
                rigidbody.velocity = new Vector2(-movementSpeed * 2, rigidbody.velocity.y);
            else
                rigidbody.velocity = new Vector2(-movementSpeed, rigidbody.velocity.y);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            if (Input.GetKey(KeyCode.LeftShift))
                rigidbody.velocity = new Vector2(movementSpeed * 2, rigidbody.velocity.y);
            else
                rigidbody.velocity = new Vector2(movementSpeed, rigidbody.velocity.y);
        }
        else
        {
            rigidbody.velocity = new Vector2(0, rigidbody.velocity.y);
        }

        anim.SetFloat("Speed", Mathf.Abs(rigidbody.velocity.x));

        if (rigidbody.velocity.x > 0)
            transform.localScale = new Vector3(1f, 1f, 1f);
        else if (rigidbody.velocity.x < 0)
            transform.localScale = new Vector3(-1f, 1f, 1f);
    }

    void Save()
    {
        if (Input.GetKeyDown(KeyCode.Insert))
        {
            var player = GetComponent<Player>();
            SaveLoadManager.SavePlayer(player);
        }
    }

    void Load()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            var playerData = SaveLoadManager.LoadPlayer(1);
            var player = GetComponent<Player>();
            player.Name = playerData.Name;
        }
    }
}

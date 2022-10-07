using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class MovementController : MonoBehaviour
{
    //Stores input from the PlayerInput
    private Vector2 movementInput;

    private Vector3Int direction;

    public Grid NavGrid; //Point Top Hex
    public Tilemap fogOfWar;

    public bool hasMoved = false;
    void Update()
    {
        if(movementInput.x == 0 && movementInput.y == 0)
        {
            hasMoved = false;
            direction = Vector3Int.zero;
        }
        else if ((movementInput.x != 0 || movementInput.y != 0) && !hasMoved)
        {
            hasMoved = true;

            GetMovementDirection();
        }

    }

    public void GetMovementDirection()
    {
        if (movementInput.x < 0) direction += new Vector3Int(-1, 0);
        if (movementInput.x > 0) direction += new Vector3Int(1, 0);
        if (movementInput.y < 0) direction += new Vector3Int(0, -1);
        if (movementInput.y > 0) direction += new Vector3Int(0, 1);

        var gridPos = NavGrid.WorldToCell(transform.position) + direction;
        var newPos = NavGrid.CellToWorld(gridPos);

        print($"Moving to Hex [{newPos}]");
        var height = WorldGrid.Instance.SampleStepped(gridPos.x, gridPos.y) * WorldGrid.Instance.MapHeight;
        transform.position = new Vector3(newPos.x, height + 1f, newPos.z);
        //UpdateFogOfWar();       
    }

    public void OnMove(InputValue value)
    {
        movementInput = value.Get<Vector2>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        transform.position -= direction;
    }

    public int vision = 1;

    void UpdateFogOfWar()
    {
        Vector3Int currentPlayerTile = fogOfWar.WorldToCell(transform.position);

        //Clear the surrounding tiles
        for(int x=-vision; x<= vision; x++)
        {
            for(int y=-vision; y<= vision; y++)
            {
                //print($"Clearing Fog at [{currentPlayerTile.x + x},{currentPlayerTile.y + y}]");
                fogOfWar.SetTile(currentPlayerTile + new Vector3Int(x, y, 0), null);               
               
            }

        }
        fogOfWar.RefreshAllTiles();

    }
}

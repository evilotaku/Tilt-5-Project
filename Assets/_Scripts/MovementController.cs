using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using UnityEngine.Events;
using System.Threading.Tasks;

public class MovementController : MonoBehaviour
{
    //Stores input from the PlayerInput
    private Vector2 movementInput;
    private PathNode currentNode;
    private Vector3Int direction;
    private Camera mainCamera;

    

    public Grid NavGrid; //Point Top Hex
    public Tilemap fogOfWar;
    public LayerMask GroundLayer;

    public float speed = 2f; //Tiles per second

    public bool hasMoved = false;

    private void Start()
    {
        mainCamera = Camera.main;
        WorldGrid.Instance.OnWorldInit += Spawn;
    }

    void Spawn()
    {
        var gridPos = NavGrid.CellToWorld(new Vector3Int(Random.Range(0, WorldGrid.Instance.Width),
                                           Random.Range(0, WorldGrid.Instance.Length)));
        transform.position = WorldGrid.Instance.GetTilePos(gridPos);
    }
    //void Update()
    //{
    //    if(movementInput.x == 0 && movementInput.y == 0)
    //    {
    //        hasMoved = false;
    //        direction = Vector3Int.zero;
    //    }
    //    else if ((movementInput.x != 0 || movementInput.y != 0) && !hasMoved)
    //    {
    //        hasMoved = true;

    //        GetMovementDirection();
    //    }

    //}

    //public void GetMovementDirection()
    //{
    //    if (movementInput.x < 0) direction += new Vector3Int(-1, 0);
    //    if (movementInput.x > 0) direction += new Vector3Int(1, 0);
    //    if (movementInput.y < 0) direction += new Vector3Int(0, -1);
    //    if (movementInput.y > 0) direction += new Vector3Int(0, 1);

    //    var gridPos = NavGrid.WorldToCell(transform.position) + direction;
    //    var newPos = NavGrid.CellToWorld(gridPos);

    //    print($"Moving to Hex [{newPos}]");
    //    var height = WorldGrid.Instance.SampleStepped(gridPos.x, gridPos.y) * WorldGrid.Instance.MapHeight;
    //    //transform.position = WorldGrid.Instance.GetTilePos()
    //    //UpdateFogOfWar();       
    //}

    //public void OnMove(InputValue value)
    //{
    //    movementInput = value.Get<Vector2>();
    //}

    public void OnClick()
    {
        print("Clicked");
        var ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit) && hit.collider)
        {            
            var target = hit.collider.gameObject.GetComponent<PathNode>();
            print($"on Hex {target.GridCoords}");
            StartCoroutine(MoveTowards(target));
        }
    }

    IEnumerator MoveTowards(PathNode target)
    {
        var gridPos = NavGrid.WorldToCell(transform.position);
        gridPos = new Vector3Int(gridPos.x, gridPos.y, 0);
        var path = Pathfinding.FindPath(WorldGrid.PathMap[gridPos], target);
        if (path == null)
        {
            print("Path not Found...");
            yield break;
        }
        path.Reverse();
        for (int i = 0; i < path.Count; i++)
        {
            transform.position = path[i].WorldCoords;
            //UpdateFogOfWar(); 
            yield return new WaitForSeconds(1 / speed);
        }
        
        
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

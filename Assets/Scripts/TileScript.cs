using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TileScript : MonoBehaviour
{
    public Point GridPosition { get; private set; }

    public bool Walkable { get; set; }

    /// <summary>
    /// Свободен ли тайл
    /// </summary>
    public bool IsEmpty { get; set; }

    private Tower myTower;

    public Vector2 WorldPosition
    {
        get
        {
            return new Vector2(transform.position.x + (GetComponent<SpriteRenderer>().bounds.size.x/2), transform.position.y - (GetComponent<SpriteRenderer>().bounds.size.y / 2));
        }
    }

    public SpriteRenderer SpriteRenderer { get => spriteRenderer; set => spriteRenderer = value; }

    private Color32 disabledColor = new Color32(255, 118, 118, 255);

    private Color32 enabledColor = new Color32(96, 255, 90, 255);


    private SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        this.SpriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    /// <summary>
    /// Устанавливает тайл
    /// </summary>
    /// <param name="gridPos"></param>
    /// <param name="worldPos"></param>
    public void Setup(Point gridPos, Vector3 worldPos, Transform parent, bool walkable)
    {
        Walkable = walkable;
        IsEmpty = !walkable;
        this.GridPosition = gridPos;
        transform.position = worldPos;
        transform.SetParent(parent);
        LevelManager.Instance.Tiles.Add(gridPos, this);
    }
    /// <summary>
    /// Когда наводим мышку на тайл
    /// </summary>
    private void OnMouseOver()
    {
        if (!EventSystem.current.IsPointerOverGameObject() && GameManager.Instance.ClickedBtn != null)
        {
            if (IsEmpty)
            {
                ColorTile(enabledColor);
            }
            if (!IsEmpty)
            {
                ColorTile(disabledColor);// нельзя разместить
            }
            else if (Input.GetMouseButtonDown(0))
            {
                PlaceTower();
            }
        }
        else if (!EventSystem.current.IsPointerOverGameObject() && GameManager.Instance.ClickedBtn == null && Input.GetMouseButtonDown(0))
        {
            if (myTower != null)
            {
                GameManager.Instance.SelectTower(myTower);
            }
            else
            {
                GameManager.Instance.DeselectTower();
            }
        }

        
    }
    /// <summary>
    /// Когда убираем мышку с тайла
    /// </summary>
    private void OnMouseExit()
    {
        ColorTile(Color.white);
    }
    /// <summary>
    /// Устанавливает башню на тайле
    /// </summary>

    private void PlaceTower()
    {
        GameObject tower = (GameObject)Instantiate(GameManager.Instance.ClickedBtn.TowerPrefab, transform.position, Quaternion.identity);
        tower.GetComponent<SpriteRenderer>().sortingOrder = GridPosition.Y;
        tower.transform.SetParent(transform);
        myTower = tower.transform.GetChild(0).GetComponent<Tower>();

        myTower.Price = GameManager.Instance.ClickedBtn.Price;
        // покупаем башню
        GameManager.Instance.BuyTower();
        // клетка спрайт теперь занята
        IsEmpty = false;
        //возвращаем прежний цвет
        ColorTile(Color.white);
        Walkable = false;
    }
    /// <summary>
    /// Раскраска тайла
    /// </summary>
    /// <param name="newColor"></param>
    private void ColorTile(Color32 newColor)
    {
        SpriteRenderer.color = newColor;
    }
}

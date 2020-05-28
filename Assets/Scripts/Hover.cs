using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hover : Singleton<Hover>
{
    /// То, что перетаскиваем
    private SpriteRenderer spriteRenderer;
    /// <summary>
    /// ссылка на радиус башни
    /// </summary>
    private SpriteRenderer rangeSpriteRenderer;

    public bool IsVisible { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        this.spriteRenderer = GetComponent<SpriteRenderer>();
        
    }

    // Update is called once per frame
    void Update()
    {
        FollowMouse();
        
    }

    private void FollowMouse()
    {
        if (this.spriteRenderer.sprite != null)
        {
            transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = new Vector3(transform.position.x, transform.position.y, 0);
        }

    }

    public void Activate(Sprite sprite)
    {
        string rangeType = GameManager.Instance.ClickedBtn.name;
        switch (rangeType)
        {
            case "BlueButton":
                this.rangeSpriteRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
                break;
            case "GreenButton":
                this.rangeSpriteRenderer = transform.GetChild(1).GetComponent<SpriteRenderer>();
                break;
            case "PinkButton":
                this.rangeSpriteRenderer = transform.GetChild(2).GetComponent<SpriteRenderer>();
                break;
        }
        this.spriteRenderer.sprite = sprite;

        rangeSpriteRenderer.enabled = true;

        IsVisible = true;
    }

    public void Deactivate()
    {
        GameManager.Instance.ClickedBtn = null;
        this.spriteRenderer.sprite = null;
        rangeSpriteRenderer.enabled = false;
        IsVisible = false;
    }

}

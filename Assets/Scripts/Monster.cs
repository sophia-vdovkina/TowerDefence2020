using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour
{
    [SerializeField]
    private float speed;

    public float Speed { get => speed; set => speed = value; }

    public float MaxSpeed { get; set; }

    private List<Debuff> debuffs = new List<Debuff>();

    private List<Debuff> debuffsToRemove = new List<Debuff>();

    private List<Debuff> newDebuffs = new List<Debuff>();
    /// <summary>
    /// Стек хранит в себе путь по которому идет монстр
    /// Он должен быть сгенерирован А* алгоритмом
    /// </summary>
    private Stack<Node> path;

    public Point GridPosition { get; set; }

    // тайл до которого нужно дойти в данный момент !!!не цель
    private Vector3 destination;
    // показывает закончил ли монстр спавниться
    public bool IsActive { get; set; }
    


    private Animator myAnimator;

    [SerializeField]
    private HealthBarScript health;

    private SpriteRenderer spriteRenderer;

    public bool Alive
    {
        get
        {
            return health.Health > 0;
        }
    }


    private void Update()
    {
        HandleDebuffs();
        Move();
    }


    public void Spawn(int health)
    {
        myAnimator = GetComponent<Animator>();

        spriteRenderer = GetComponent<SpriteRenderer>();

        MaxSpeed = Speed;

        this.health.Reset();

        IsActive = false;

        transform.position = LevelManager.Instance.GreenPortal.transform.position;

        this.health.SetMaxHealth(health);

        StartCoroutine(Scale(new Vector3(0.1f, 0.1f), new Vector3(1,1), false));

        SetPath(LevelManager.Instance.Path);
    }
    /// <summary>
    /// постепенно увеличивает/уменьшает монстра
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <returns></returns>
    public IEnumerator Scale(Vector3 from, Vector3 to, bool remove)
    {
        float progress = 0;

        while (progress <= 1)
        {
            transform.localScale = Vector3.Lerp(from, to, progress);

            progress += Time.deltaTime;

            yield return null;
        }
        // на всякий случай убеждаемся что монстр нужного размера
        transform.localScale = to;

        IsActive = true;

        if (remove)
        {
            Release();
        }
        
    }
    
    private void Move()
    {
        if (IsActive)
        {
            transform.position = Vector2.MoveTowards(transform.position, destination, Speed * Time.deltaTime);

            if (transform.position == destination)
            {
                if (path != null && path.Count > 0)
                {
                    GridPosition = path.Peek().GridPosition;
                    destination = path.Pop().WorldPosition;
                }
            }
        }
        else
        {
            Debug.Log(GridPosition);
        }
    }

    private void SetPath(Stack<Node> newPath)
    {
        if (newPath != null)
        {
            path = newPath;

            GridPosition = path.Peek().GridPosition;
            destination = path.Pop().WorldPosition;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "PurplePortal")
        {
            StartCoroutine(Scale(new Vector3(1, 1), new Vector3(0.1f, 0.1f), true));
            GameManager.Instance.Health--;
        }

        if (collision.tag == "Tile")
        {
            spriteRenderer.sortingOrder = collision.GetComponent<TileScript>().GridPosition.Y;
        }


    }
    /// <summary>
    /// Подготавливает удаленного монстра к повторному использованию
    /// </summary>
    public void Release()
    {
        debuffs.Clear();
        // монстр не активен
        IsActive = false;
        // помещаем его в точку спавна
        GridPosition = LevelManager.Instance.GreenSpawn;
        // помещаем монстра обратно в пул
        GameManager.Instance.Pool.ReleaseObject(gameObject);
        // удаляем монстра из игры
        GameManager.Instance.RemoveMonster(this);

        Speed = MaxSpeed;
    }

    public void TakeDamage(float damage, Element damageSource)
    {
        if (IsActive)
        {
            SoundManager.Instance.PlaySFX("Switch");

            health.Health = health.Health - damage;

            if (!Alive)
            {

                IsActive = false;
                GameManager.Instance.Gold += 3;

                myAnimator.SetTrigger("Die");

                SoundManager.Instance.PlaySFX("Death");


                GetComponent<SpriteRenderer>().sortingOrder--;
            }
        }
        
    }

    public void AddDebuff(Debuff debuff)
    {
        // чтобы на монстре не было двух одинаковых дебаффов
        if (!debuffs.Exists(x => x.GetType() == debuff.GetType()))
        {
            newDebuffs.Add(debuff);
        }
        
    }

    private void HandleDebuffs()
    {
        // Смотрим появились ли у нас новые дебаффы
        if(newDebuffs.Count > 0)
        {
            debuffs.AddRange(newDebuffs);

            newDebuffs.Clear();
        }
        // Смотрим нужно ли удалить какой-нибудь дебафф
        foreach (Debuff debuff in debuffsToRemove)
        {
            debuffs.Remove(debuff);
        }
        debuffsToRemove.Clear();
        foreach(Debuff debuff in debuffs)
        {
            debuff.Update();
        }
    }

    public void RemoveDebuff(Debuff debuff)
    {
        debuffsToRemove.Add(debuff);
    }
}

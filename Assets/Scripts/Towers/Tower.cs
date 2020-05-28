using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Element {PINK, GREEN, BLUE};

public abstract class Tower : MonoBehaviour
{
    [SerializeField]
    protected Sprite[] upgratedTowers;


    [SerializeField]
    private int damage;
    public int Damage { get => damage;}

    public int Price { get; set; }

    [SerializeField]
    private float debuffDuration;

    public float DebuffDuration { get => debuffDuration; set => debuffDuration = value; }
    /// <summary>
    /// Вероятностьь того, что дебафф будет наложен
    /// </summary>
    [SerializeField]
    private float proc;

    public float Proc { get => proc; set => proc = value; }

    /// <summary>
    /// Тип пули
    /// </summary>
    [SerializeField]
    private string projectileType;

    [SerializeField]
    private float projectileSpeed;

    public float ProjectileSpeed { get => projectileSpeed; private set => projectileSpeed = value; }
    
    private SpriteRenderer mySpriteRenderer;

    private Monster target;

    public Monster Target { get => target; private set => target = value; }
    

    /// <summary>
    /// Очередь монстров, которых можно атаковать
    /// </summary>
    private List<Monster> monsters = new List<Monster>();

    private bool canAttack = true;

    private float attackTimer;

    [SerializeField]
    private float attackCooldown;
    
    [SerializeField]
    protected GameObject parent;

    public Element ElementType { get; protected set; }
    
    public TowerUpgrade[] Upgrades { get; protected set; }

    public int Level { get; protected set; }

    protected SpriteRenderer parentSpriteRenderer;

    private void Awake()
    {
        mySpriteRenderer = GetComponent<SpriteRenderer>();
        parentSpriteRenderer = parent.GetComponent<SpriteRenderer>();
        Level = 1;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Attack();
        Debug.Log(Target);
    }

    public void Select()
    {
        mySpriteRenderer.enabled = !mySpriteRenderer.enabled;
        GameManager.Instance.UpdateUpgradeTip();
    }

    public TowerUpgrade NextUpgrade
    {
        get
        {
            if (Upgrades.Length > Level - 1)
            {
                return Upgrades[Level - 1];
            }
            return null;
        }
    }
    /// <summary>
    /// Башня атакует монстра
    /// </summary>
    private void Attack()
    {
        if (!canAttack)
        {
            attackTimer += Time.deltaTime;

            if (attackTimer >= attackCooldown)
            {
                canAttack = true;

                attackTimer = 0;
            }
        }
        else if (Target != null && !Target.Alive || Target != null && !Target.IsActive)
        {
            
            monsters.Remove(Target);
            Target = null;
        }
        if (Target == null && monsters.Count > 0 && monsters[0].IsActive)
        {
            Target = monsters[0];
        }
        if (Target != null && Target.IsActive)
        {
            if (canAttack)
            {
                Shoot();

                canAttack = false;
            }
            
        }
        

    }
    
    /// <summary>
    /// Башня стреляет
    /// </summary>
    private void Shoot()
    {
        // Получаем пулю из пулa объектов
        Projectile projectile = GameManager.Instance.Pool.GetObject(projectileType).GetComponent<Projectile>();
        projectile.transform.position = transform.position;
        projectile.Initialize(this);
    }

    public virtual string GetStats()
    {
        if(NextUpgrade != null)
        {
            return string.Format("\nУровень: {0} \nУрон: {1} <color=#00ff00ff> +{2}</color>\nВероятность дебаффа: {3}% <color=#00ff00ff>+{4}%</color>\nДлительность дебаффа:" +
                " {5} сек <color=#00ff00ff>+{6} сек</color>", Level, damage, NextUpgrade.Damage - damage, proc, NextUpgrade.Proc - proc, DebuffDuration, NextUpgrade.DebuffDuration - DebuffDuration);
        }
        return string.Format("\nУровень: {0} \nУрон: {1}\nВероятность дебаффа: {2}%\nДлительность дебаффа: {3} сек", Level, damage, proc, DebuffDuration);
    }

    public abstract Debuff GetDebuff();

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Monster")
        {
            monsters.Add(collision.GetComponent<Monster>());
        }
    }

    public virtual void Upgrade()
    {
        parentSpriteRenderer.sprite = upgratedTowers[Level - 1];
        GameManager.Instance.Gold -= NextUpgrade.Price;
        Price = NextUpgrade.Price;
        damage = NextUpgrade.Damage;
        proc = NextUpgrade.Proc;
        debuffDuration = NextUpgrade.DebuffDuration;
        Level++;
        GameManager.Instance.UpdateUpgradeTip();
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Monster")
        {
            if (collision.GetComponent<Monster>() == Target)
            {
                monsters.Remove(Target);
                Target = null;
            }
            else
            {
                monsters.Remove(collision.GetComponent<Monster>());
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Monster target;

    private Tower myTower;

    private Element elementType;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        MoveToTarget();
    }

    public void Initialize(Tower myTower)
    {
        this.myTower = myTower;
        this.target = myTower.Target;
        elementType = myTower.ElementType;
    }


    private void MoveToTarget()
    {
        if (target != null && target.IsActive)
        {
            // двигается к цели
            transform.position = Vector3.MoveTowards(transform.position, target.transform.position, Time.deltaTime * myTower.ProjectileSpeed);

            // считает напрвление цели пули
            Vector2 dir = target.transform.position - transform.position;

            // считает угол под которым летит пуля
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

            // поворачивает пулю под определенным углом
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
        else if (!target.IsActive)
        {
            GameManager.Instance.Pool.ReleaseObject(gameObject);
        }
    }

    private void ApplyDebuff()
    {
        if (target != null && target.IsActive)
        {
            float roll = Random.Range(0, 100);

            if (roll <= myTower.Proc)
            {
                target.AddDebuff(myTower.GetDebuff());
            }
        }
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Monster")
        {
            if (target.gameObject == collision.gameObject && target.IsActive)
            {
                target.TakeDamage(myTower.Damage, elementType);

                GameManager.Instance.Pool.ReleaseObject(gameObject);

                ApplyDebuff();
            }
            //target = collision.GetComponent<Monster>();
            
        }
    }

    
}

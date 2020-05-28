using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class TowerBtn : MonoBehaviour
{
    /// <summary>
    /// Префаб кнопки
    /// </summary>
    [SerializeField]
    private GameObject towerPrefab;

    /// <summary>
    /// Ссылка на спрайт иконки башни
    /// </summary>
    [SerializeField]
    private Sprite sprite;

    [SerializeField]
    private int price;

    [SerializeField]
    private Text priceTxt;

    /// <summary>
    /// Изображение на котором находится цена
    /// </summary>
    [SerializeField]
    private Image image;

    public GameObject TowerPrefab { get => towerPrefab;}
    public Sprite Sprite { get => sprite;}
    public int Price { get => price;}

    private void Start()
    {
        priceTxt.text = Price + "$";

        GameManager.Instance.Changed += new GoldChanged(PriceCheck);
    }

    private void PriceCheck()
    {
        if(price <= GameManager.Instance.Gold)
        {
            GetComponent<Image>().color = Color.white;
            image.color = Color.white;
            priceTxt.color = Color.white;
        }
        else
        {
            GetComponent<Image>().color = Color.grey;
            image.color = Color.grey;
            priceTxt.color = Color.grey;
        }
    }

    public void ShowInfo(string type)
    {
        string tooltip = string.Empty;

        switch (type)
        {
            case "Blue":
                BlueTower blue = towerPrefab.GetComponentInChildren<BlueTower>();
                tooltip = string.Format("<color=#4e9edf><size=20><b> Башня зайца </b></size></color>\n\nС некоторой вероятностью ударяет молнией" +
                    "\nв цель и останавливает цель на месте" +
                    " \nУрон: {0} \nВероятность дебаффа: {1}%" +
                    "\nДлительность дебаффа: {2} cек", blue.Damage, blue.Proc, blue.DebuffDuration);
                break;
            case "Pink":
                PinkTower pink = towerPrefab.GetComponentInChildren<PinkTower>();
                tooltip = string.Format("<color=#df4e9e><size=20><b> Башня кролика </b></size></color>\n\nС некоторой вероятностью" +
                    "\nподжигает цель" +
                    " \nУрон: {0} \nВероятность дебаффа: {1}%" +
                    "\nДлительность дебаффа: {2} cек \nУрон от горения: {3}", pink.Damage, pink.Proc, pink.DebuffDuration, pink.TickDamage);
                break;
            case "Green":
                GreenTower green = towerPrefab.GetComponentInChildren<GreenTower>();
                tooltip = string.Format("<color=#339a49><size=20><b> Башня кошки </b></size></color>\n\nС некоторой вероятностью отравляет цель" +
                    "\nи замедляет ее" +
                    " \nУрон: {0} \nВероятность дебаффа: {1}%" +
                    "\nДлительность дебаффа: {2} cек \nЗамедление: {3}%", green.Damage, green.Proc, green.DebuffDuration, green.SlowingFactor);
                break;
        }

        GameManager.Instance.SetTooltipText(tooltip);
        GameManager.Instance.ShowStats();
    }
}

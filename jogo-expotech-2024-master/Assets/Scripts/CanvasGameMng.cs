using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class CanvasGameMng : MonoBehaviour
{
    public static CanvasGameMng Instance;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            return;
        }
        Destroy(gameObject);
    }

    public TextMeshProUGUI txtMoney;
    public int money = 0;

    public Scrollbar scrPutOutFire;
    public Image imgHandleScrPutOutFire;
    public float factorWaterButton = 5;
    public Color colorEnablePutOutFire;
    public Color colorDisblePutOutFire;
    public GameObject pnlDrones;
    public GameObject pnlRenewIndustry;
    bool isPutOutFireFilled = false;

    public int priceDrone = 2000;
    public int priceHoldingCO2 = 3000;
    public int priceRenewIndustry = 5000;
    bool isDronesEnable = false;
    bool isPnlRenewIndustry = false;
    
    // Start is called before the first frame update
    void Start()
    {
        scrPutOutFire.value = 0;
        txtMoney.text = $"${money}";
    }

    // Update is called once per frame
    void Update()
    {
        if (isPutOutFireFilled == false)
        {
            scrPutOutFire.value += Time.deltaTime/factorWaterButton;
            imgHandleScrPutOutFire.color = colorDisblePutOutFire;
            if (scrPutOutFire.value >=1)
            {
                scrPutOutFire.value = 1;
                isPutOutFireFilled = true;
                imgHandleScrPutOutFire.color = colorEnablePutOutFire;
            }
        }
    }

    public void PutOutFire()
    {
        if(isPutOutFireFilled == true)
        {
            scrPutOutFire.value = 0;
            isPutOutFireFilled = false;
            HexagonAreaMng.Instance.PutOutFireHexagonArea();
            
        }        
    }

    public void IncrementMoney(int value)
    {
        money += value;
        txtMoney.text = $"${money}";
    }

    public void BuyDrone()
    {
        if(money>= priceDrone && isDronesEnable == false)
        {
            money -= priceDrone;
            txtMoney.text = money.ToString();
            pnlDrones.SetActive(true);
            isDronesEnable = true;
        }
    }

    public void EnableBuyDrones()
    {
        isDronesEnable = false;
    }

    public void ActivedDroneRegion(int region)
    {
        switch (region)
        {
            case 1: // Norte
                HexagonAreaMng.Instance.PutOutFireHexagonInRegion(EnumRegion.norte);
                break;
            case 2: // Nordeste
                HexagonAreaMng.Instance.PutOutFireHexagonInRegion(EnumRegion.nordeste);
                break;
            case 3: // Centro Oeste
                HexagonAreaMng.Instance.PutOutFireHexagonInRegion(EnumRegion.centro_oeste);
                break;
            case 4: // Sudeste
                HexagonAreaMng.Instance.PutOutFireHexagonInRegion(EnumRegion.sudeste);
                break;
            case 5: // Sul
                HexagonAreaMng.Instance.PutOutFireHexagonInRegion(EnumRegion.sul);
                break;
        }
        HideDrones();
        EnableBuyDrones();
    }

    public void HideDrones()
    {
        pnlDrones.SetActive(false);
    }

    public void BuyHoldinCO2()
    {
        if (money >= priceHoldingCO2 && HexagonAreaMng.Instance.IsHoldCo2 == false)
        {
            money -= priceHoldingCO2;
            txtMoney.text = money.ToString();
            HexagonAreaMng.Instance.HoldCO2();
        }
    }

    public void BuyRenewIndustry()
    {
        if(money >= priceRenewIndustry && pnlRenewIndustry.activeSelf == false) {
            pnlRenewIndustry.SetActive(true);
        }
    }

    public void RenewIndustry(int idIndustry)
    {
        pnlRenewIndustry.SetActive(false);
        HexagonAreaMng.Instance.RenewHexagonIndustry(idIndustry);
    }

    public void PayRenewIndustry()
    {
        money -= priceRenewIndustry;
        txtMoney.text = money.ToString();
    }
}

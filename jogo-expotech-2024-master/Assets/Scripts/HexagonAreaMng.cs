using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HexagonAreaMng : MonoBehaviour
{
    public static HexagonAreaMng Instance;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            return;
        }
        Destroy(gameObject);
    }

    public List<HexagonoArea> listHexagons;
    public int totalInitialFlame = 4;
    public int totalInitialIndustry = 3;
    public List<HexagonoArea> listHexagonsFilled;
    public List<HexagonoArea> listHexagonosIndustries;
    public List<GameObject> rawCameraIndustries;
    bool isHoldCo2 = false;
    Coroutine coroutine;
    public int autoMoney = 5;
    public float maxTimeArea = 12;
    public float maxTimeMoney = 5;
    public int co2Money = 150;
    public int flameMoney = 250;
    public float timeHoldCo2 = 15;

    float nextTime = 0;
    float nextTimeMoney = 0;

    public bool IsHoldCo2
    {
        get { return isHoldCo2; }
    }
    // Start is called before the first frame update
    void Start()
    {
        listHexagons = FindObjectsOfType<HexagonoArea>().ToList();
        listHexagonsFilled = new List<HexagonoArea>();
        InsertFlamesInitials();
        InsertIndustryInitials();
        nextTime += Time.timeSinceLevelLoad + maxTimeArea;
        nextTimeMoney += Time.timeSinceLevelLoad + maxTimeMoney;
    }

    private void FixedUpdate()
    {
        if(nextTime < Time.timeSinceLevelLoad)
        {
            nextTime = Time.timeSinceLevelLoad + maxTimeArea;
            foreach (var hexagon in listHexagonsFilled.ToList())
            {
                if(isHoldCo2 == false && (hexagon.iconArea == EnumIconArea.co2 || hexagon.iconArea == EnumIconArea.industry))
                {
                    hexagon.ExpandArea();
                }
                else if(hexagon.iconArea == EnumIconArea.flame)
                {
                    hexagon.ExpandArea();
                }
            }
        }

        if (nextTimeMoney < Time.timeSinceLevelLoad)
        {
            nextTimeMoney = Time.timeSinceLevelLoad + maxTimeMoney;
            foreach (var hexagon in listHexagons.ToList())
            {
                if (hexagon.iconArea == EnumIconArea.nothing)
                {
                    CanvasGameMng.Instance.IncrementMoney(autoMoney);
                }
            }
        }
    }

    private void InsertFlamesInitials()
    {
        int totalHexagons = listHexagons.Count;
        for(int i = 0; i < totalInitialFlame; i++)
        {
            int random = Random.Range(0, totalHexagons);
            if (listHexagons[random].iconArea == EnumIconArea.nothing)
            {
                listHexagons[random].SetIconArea(EnumIconArea.flame);
                listHexagonsFilled.Add(listHexagons[random]);
            }
            else
            {
                i--;
            }
        }
    }

    private void InsertIndustryInitials()
    {
        int totalInstantiate = 0;
        int totalHexagons = listHexagons.Count;
        for (int i = 0; i < totalInitialIndustry; i++)
        {
            int random = Random.Range(0, totalHexagons);
            if (listHexagons[random].iconArea == EnumIconArea.nothing)
            {
                
                listHexagons[random].SetIconArea(EnumIconArea.industry);
                listHexagons[random].SetHexagonOrigin(listHexagons[random].GetComponent<HexagonoArea>());
                var rawCamera = Instantiate(rawCameraIndustries[totalInstantiate]);
                float positionX = listHexagons[random].transform.position.x;
                float positionY = listHexagons[random].transform.position.y;
                float positionZ = rawCamera.transform.position.z;
                rawCamera.transform.position = new Vector3(positionX, positionY, positionZ);
                listHexagonsFilled.Add(listHexagons[random]);
                listHexagonosIndustries.Add(listHexagons[random]);
                totalInstantiate++;
            }
            else
            {
                i--;
            }
        }
    }

    public void AddNewHexagonAreaWithPadding(HexagonoArea hexagonoArea)
    {
        listHexagonsFilled.Add(hexagonoArea);
    }
    public void RemoveNewHexagonAreaWithPadding(HexagonoArea hexagonoArea)
    {
        listHexagonsFilled.Remove(hexagonoArea);
    }

    public void PutOutFireHexagonArea()
    {
        HexagonoArea hexagonoArea = null;
        foreach(HexagonoArea hexagon in listHexagonsFilled)
        {
            if(hexagon.iconArea == EnumIconArea.flame)
            {
                hexagon.SetIconArea(EnumIconArea.nothing);
                hexagonoArea = hexagon;
                break;
            }
        }
        if(hexagonoArea != null)
        {
            listHexagonsFilled.Remove(hexagonoArea);
            CanvasGameMng.Instance.IncrementMoney(flameMoney);
        }
    }

    public void PutOutFireHexagonInRegion(EnumRegion region)
    {
        List<HexagonoArea> listHexagons = new List<HexagonoArea>();
        foreach(HexagonoArea hexagon in listHexagonsFilled)
        {
            if(hexagon.regionId == region && (hexagon.iconArea == EnumIconArea.flame || hexagon.iconArea == EnumIconArea.co2))
            {
                listHexagons.Add(hexagon);
                switch (hexagon.iconArea)
                {
                    case EnumIconArea.co2:
                        CanvasGameMng.Instance.IncrementMoney(co2Money);
                        break;
                    case EnumIconArea.flame:
                        CanvasGameMng.Instance.IncrementMoney(flameMoney);
                        break;
                }
            }
        }

        if (listHexagons.Count > 0)
        {
            foreach (HexagonoArea hexagon in listHexagons)
            {
                hexagon.SetIconArea(EnumIconArea.nothing);
                hexagon.FreeHexagon();
                hexagon.RemoveHexagonOrigin();
                listHexagonsFilled.Remove(hexagon);                
            }            
        }
    }

    public void HoldCO2()
    {
        isHoldCo2 = true;
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
            coroutine = StartCoroutine(TimerHoldCO2());
        }
        else {
            coroutine = StartCoroutine(TimerHoldCO2());
        }
    }

    IEnumerator TimerHoldCO2()
    {
        foreach(var hexagon in listHexagonsFilled)
        {
            if(hexagon.iconArea == EnumIconArea.co2 || hexagon.iconArea == EnumIconArea.industry)
            {
                hexagon.HoldHexagon();
            }
        }

        yield return new WaitForSeconds(timeHoldCo2);

        foreach (var hexagon in listHexagonsFilled)
        {
            if (hexagon.iconArea == EnumIconArea.co2 || hexagon.iconArea == EnumIconArea.industry)
            {
                hexagon.FreeHexagon();
            }
        }
        isHoldCo2 = false;
    }

    public void RenewHexagonIndustry(int idIndustry)
    {
        if (listHexagonosIndustries[idIndustry].iconArea == EnumIconArea.industry)
        {
            listHexagonosIndustries[idIndustry].SetRenewIndustry();
            CanvasGameMng.Instance.PayRenewIndustry();
        }
        else
        {
            //Mensagem dizendo que já foi comprado
        }
        
    }
}

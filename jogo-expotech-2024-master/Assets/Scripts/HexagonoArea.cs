using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class HexagonoArea : MonoBehaviour
{
    public float vX = 1;
    public float vY = 1.65f;
    public List<HexagonoArea> listHexagonosAdjacent = new List<HexagonoArea>();
    public EnumRegion regionId;
    public SpriteRenderer sprite;
    public EnumIconArea iconArea = 0;
    public SpriteRenderer sptIconArea;
    public List<Sprite> listIconsArea;
    public bool isCompletedAdjacentArea = false;
    public HexagonoArea hexagonOrigin;
    public List<HexagonoArea> hexagonosChildren;

    // Start is called before the first frame update
    void Start()
    {
        RaycastHit2D[] hit1 = Physics2D.RaycastAll(transform.position, transform.TransformDirection(new Vector2(vX, vY)), 0.5f);
        Debug.DrawRay(transform.position, transform.TransformDirection(new Vector2(vX, vY)) * 0.3f, Color.red);
        RaycastHit2D[] hit2 = Physics2D.RaycastAll(transform.position, transform.TransformDirection(new Vector2(-vX, vY)), 0.5f);
        Debug.DrawRay(transform.position, transform.TransformDirection(new Vector2(-vX, vY)) * 0.3f, Color.blue);
        RaycastHit2D[] hit3 = Physics2D.RaycastAll(transform.position, transform.TransformDirection(new Vector2(vX, -vY)), 0.5f);
        Debug.DrawRay(transform.position, transform.TransformDirection(new Vector2(vX, -vY)) * 0.3f, Color.yellow);
        RaycastHit2D[] hit4 = Physics2D.RaycastAll(transform.position, transform.TransformDirection(new Vector2(-vX, -vY)), 0.5f);
        Debug.DrawRay(transform.position, transform.TransformDirection(new Vector2(-vX, -vY)) * 0.3f, Color.magenta);
        RaycastHit2D[] hit5 = Physics2D.RaycastAll(transform.position, transform.TransformDirection(new Vector2(vX, 0)), 0.5f);
        Debug.DrawRay(transform.position, transform.TransformDirection(new Vector2(vX, 0)) * 0.5f, Color.green);
        RaycastHit2D[] hit6 = Physics2D.RaycastAll(transform.position, transform.TransformDirection(new Vector2(-vX, 0)), 0.5f);
        Debug.DrawRay(transform.position, transform.TransformDirection(new Vector2(-vX, 0)) * 0.5f, Color.cyan);

        List<RaycastHit2D[]> listHits = new List<RaycastHit2D[]>() { hit1, hit2, hit3, hit4, hit5, hit6 };
        foreach (var hit in listHits)
        {
            foreach(var hitCollider in hit)
            {
                if (hitCollider.collider != null && hitCollider.collider.name != gameObject.name)
                {
                    Debug.Log($"Hit Name: {hitCollider.collider.name} ");
                    listHexagonosAdjacent.Add(hitCollider.transform.GetComponent<HexagonoArea>());
                }
            }
            
        }

        DefineColorHexagon();

        sptIconArea.sprite = listIconsArea[((int)iconArea)];

        hexagonOrigin = null;
    }

    private void DefineColorHexagon()
    {
        switch (regionId)
        {
            case EnumRegion.norte: // Norte
                sprite.color = Color.green;
                break;
            case EnumRegion.nordeste: // Nordeste
                sprite.color = Color.yellow;
                break;
            case EnumRegion.centro_oeste: // Centro Oeste
                sprite.color = new Color(1, (167f / 255f), (167f / 255f));
                break;
            case EnumRegion.sudeste: // Sudeste
                sprite.color = new Color(1, 0.7f, 0.45f);
                break;
            case EnumRegion.sul: // Sul
                sprite.color = Color.cyan;
                break;
        }
    }

    public void SetIconArea(EnumIconArea iconId)
    {
        iconArea = iconId;
        sptIconArea.sprite = listIconsArea[(int)iconId];
    }

    public void ExpandArea()
    {
        if(iconArea == EnumIconArea.nothing) { return; }
        foreach(var hexagon in listHexagonosAdjacent)
        {
            if(hexagon.iconArea == EnumIconArea.nothing)
            {
                switch (iconArea)
                {
                    case EnumIconArea.flame:
                        hexagon.SetIconArea(EnumIconArea.flame);                        
                        break;
                    case EnumIconArea.industry:
                    case EnumIconArea.co2:
                        hexagon.SetIconArea(EnumIconArea.co2);
                        hexagon.SetHexagonOrigin(hexagonOrigin);
                        hexagonOrigin.AddHexagonoChildren(hexagon);
                        break;
                }
                HexagonAreaMng.Instance.AddNewHexagonAreaWithPadding(hexagon);
                return;
            }
        }
    }

    public void SetHexagonOrigin(HexagonoArea hexagonOrigin)
    {
        this.hexagonOrigin = hexagonOrigin;
    }

    public void AddHexagonoChildren(HexagonoArea hexagonoChild)
    {
        hexagonosChildren.Add(hexagonoChild);
    }

    public void RemoveHexagonOrigin()
    {
        if (hexagonOrigin != null)
        {
            hexagonOrigin.RemoveHexagonChildren(this);
            hexagonOrigin = null;
        }        
    }

    public void RemoveHexagonChildren(HexagonoArea hexagon)
    {
        hexagonosChildren.Remove(hexagon);
    }

    public void HoldHexagon()
    {
        sprite.color = Color.gray;
    }

    public void FreeHexagon()
    {
        DefineColorHexagon();
    }

    public void SetRenewIndustry()
    {
        SetIconArea(EnumIconArea.renew_industry);
        foreach(var hexagon in hexagonosChildren.ToList())
        {
            hexagon.SetIconArea(EnumIconArea.nothing);
            hexagon.FreeHexagon();
            hexagon.RemoveHexagonOrigin();
            HexagonAreaMng.Instance.RemoveNewHexagonAreaWithPadding(hexagon);
        }
        hexagonosChildren.Clear();
    }
}

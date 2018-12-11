using HoloToolkit.Unity.InputModule;
using HoloToolkit.Unity.InputModule.Utilities.Interactions;
using HoloToolkit.Unity.Receivers;
using HoloToolkit.Unity.UX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelButton : InteractionReceiver
{
    public Transform modelRoot;
    public Material scaleHandleMaterial;
    public Material rotateHandleMaterial;
    public Material interactingMaterial;
    public BoundingBox boundingBoxPrefab;
    public AppBar appBarPrefab;
    public GameObject[] buttons;
    private GameObject[] models;
    private int currentPage;
    // Use this for initialization
    void Start()
    {
        models = Resources.LoadAll<GameObject>("ModelPrefabs");
        currentPage = 0;
        RefreshButtons();
        CreatModel(models[0]);
    }
    void RefreshButtons()
    {
        int remainNum = models.Length - buttons.Length * currentPage;
        int maxIndex = buttons.Length > remainNum ? remainNum : buttons.Length;
        for (int i = 0; i < maxIndex; i++)
        {
            buttons[i].name = (buttons.Length * currentPage + i).ToString();
            buttons[i].SetActive(true);
        }
        for (int i = maxIndex; i < buttons.Length; i++)
        {
            buttons[i].SetActive(false);
        }
    }
    void CreatModel(GameObject model)
    {

        GameObject appBar = GameObject.Find("AppBar(Clone)");
        if (appBar != null)
        {
            BoundingBox box = appBar.GetComponent<AppBar>().BoundingBox;
            box.Target.GetComponent<BoundingBoxRig>().Deactivate();
            Destroy(box.Target.GetComponent<BoundingBoxRig>());
            Destroy(box.Target);
            Destroy(box.gameObject);
            Destroy(appBar);
        }
        GameObject prebox = GameObject.Find("BoundingBoxBasic(Clone)");
        if (prebox != null)
        {
            Destroy(prebox);
        }
        GameObject center = GameObject.Find("center");
        if (center != null)
        {
            Destroy(center);
        }


        GameObject g = Instantiate<GameObject>(model, modelRoot);
        g.transform.localScale = new Vector3(0.0003f, 0.0003f, 0.0003f);
        Vector3 size = g.GetComponent<MeshFilter>().mesh.bounds.size;
        Debug.Log(size);
        g.transform.position = new Vector3(0, (-1f) * size.y * g.transform.localScale.y / 2f, 1 + size.z * g.transform.localScale.z / 2f);
        BoundingBoxRig rig = g.AddComponent<BoundingBoxRig>();
        rig.ScaleHandleMaterial = scaleHandleMaterial;
        rig.RotateHandleMaterial = rotateHandleMaterial;
        rig.InteractingMaterial = interactingMaterial;
        rig.BoundingBoxPrefab = boundingBoxPrefab;
        rig.AppBarPrefab = appBarPrefab;
        rig.MaxScale = 10;
        rig.AppBarHoverOffsetZ = 0.01f;
        TwoHandManipulatable twoHand = g.AddComponent<TwoHandManipulatable>();
        twoHand.BoundingBoxPrefab = boundingBoxPrefab;
        twoHand.ManipulationMode = ManipulationMode.MoveScaleAndRotate;
    }
    protected override void InputClicked(GameObject obj, InputClickedEventData eventData)
    {
        switch (obj.name)
        {
            case "Pre":
                if (currentPage > 0)
                {
                    currentPage--;
                    RefreshButtons();
                }
                else
                {

                }
                break;
            case "Next":
                if ((currentPage + 1) * buttons.Length < models.Length)
                {
                    currentPage++;
                    RefreshButtons();
                }
                else
                {

                }
                break;
            default:
                CreatModel(models[int.Parse(obj.name)]);
                break;
        }
    }
}

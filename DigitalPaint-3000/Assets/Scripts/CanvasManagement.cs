using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CanvasManagement : MonoBehaviour, IPointerEnterHandler
{

    public Transform line;
    public Transform cube;
    public Transform point;
    public Color actualColor;
    Camera cam;
    private Transform canvas;
    private Transform boutons;
    public List<List<Transform>> MultiSommets;
    private bool onMenu = false;
    private Mode mode = Mode.Polygone;
    [SerializeField]
    private int step;
    public LineRenderer ControlPolygon;
    enum Mode
    {
        Bezier,
        Polygone

    }
    private void Start()
    {
        canvas = GameObject.Find("Canvas").transform;
        Transform obj = Instantiate(line, Vector3.zero, Quaternion.identity);
        ControlPolygon = obj.GetComponent<LineRenderer>();
        ControlPolygon.name = "ControlPolygon";
        
        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        MultiSommets = new List<List<Transform>>();
        boutons = canvas.Find("Boutons");
        actualColor = Color.red;
        StartCoroutine(UpdateLinesAndBezier());
    }

    private void LateUpdate()
    {
        step = Tool_Bezier.getInstance().step;
        if (!onMenu)
            {
                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    Vector3 mousePos = Input.mousePosition;
                    Ray ray = cam.ScreenPointToRay(mousePos);
                    RaycastHit hit;
                    switch (mode)
                    {
                        case Mode.Bezier:
                            if (Physics.Raycast(ray, out hit))
                            {
                                Tool_Bezier.getInstance().Pressed(hit.point, point);
                            }
                            break;
                        case Mode.Polygone:
                            if (Physics.Raycast(ray, out hit))
                            {
                                Tool_Polygone.getInstance().Pressed(hit.point, point);
                            }
                            break;

                    }

                }

                if (Input.GetKeyDown(KeyCode.A))
                {
                    Tool_Polygone.getInstance().End_Polygon();
                }
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Tool_Bezier.getInstance().path.PointMesh();
            }
            }
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            boutons.gameObject.SetActive(!boutons.gameObject.activeSelf);
            onMenu = !onMenu;
        }

        if (Input.GetKey(KeyCode.B))
        {
            Tool_Bezier.getInstance().step += 10;
        }
        if (Input.GetKey(KeyCode.N))
        {
            Tool_Bezier.getInstance().step -= 10;
        }



    }
    

    public IEnumerator UpdateLinesAndBezier()
    {
        while (true)
        {
            ControlPolygon.startColor = actualColor;
            ControlPolygon.endColor = actualColor;
            ControlPolygon.positionCount = Tool_Bezier.getInstance().ListeSommets.Count;
            for (int i = 0; i < Tool_Bezier.getInstance().ListeSommets.Count; ++i)
            {
                ControlPolygon.SetPosition(i, Tool_Bezier.getInstance().ListeSommets[i].position);


            }
            if (Tool_Bezier.getInstance().ListeSommets.Count >= 3)
            {
                Tool_Bezier.getInstance().Bezier();
            }
            yield return new WaitForSeconds(0.1f);
        }
    }


    public LineRenderer DrawALine(Vector3 Point1, Vector3 Point2, Color color)
    {
        
        Transform obj = Instantiate(line, Vector3.zero, Quaternion.identity);
        LineRenderer lineR = obj.GetComponent<LineRenderer>();

        lineR.SetPosition(0, Point1);
        lineR.SetPosition(1, Point2);
        lineR.startColor = color;
        lineR.endColor = color;
        lineR.useWorldSpace = false;
        return lineR;

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log(eventData.position);
    }


    public void ModeBezier()
    {
        mode = Mode.Bezier;
    }

    public void ModePolygon()
    {
        mode = Mode.Polygone;
    }
}

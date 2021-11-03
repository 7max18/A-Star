using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class PlayerTank : MonoBehaviour 
{
    public Transform targetTransform;
    private float movementSpeed, rotSpeed, curMovementSpeed;

    public Node startNode { get; set; }
    public Node goalNode { get; set; }

    public ArrayList pathArray;
    private Node nextNode;

    private UnityAction mouseClickedListener;

    private void Awake()
    {
        mouseClickedListener = new UnityAction(FindPath);
    }

    private void OnEnable()
    {
        EventManager.StartListening("Mouse Clicked", mouseClickedListener);
    }

    private void OnDisable()
    {
        EventManager.StopListening("Mouse Clicked", mouseClickedListener);
    }

    // Use this for initialization
    void Start () 
    {
        movementSpeed = 10.0f;
        curMovementSpeed = movementSpeed;
        rotSpeed = 5.0f;

        pathArray = new ArrayList();
    }
	
	// Update is called once per frame
	void Update () 
    {
        if (pathArray == null)
            return;
        else if (pathArray.Count == 0)
            return;

        if(Vector3.Distance(transform.position, nextNode.position) <= 0.1)
        {
            StartCoroutine(FindNextPoint());
        }

        transform.Translate(Vector3.forward * Time.deltaTime * curMovementSpeed);
    }

    void FindPath()
    {
        //Assign StartNode and Goal Node
        startNode = new Node(GridManager.instance.GetGridCellCenter(GridManager.instance.GetGridIndex(transform.position)));
        goalNode = new Node(GridManager.instance.GetGridCellCenter(GridManager.instance.GetGridIndex(targetTransform.position)));

        pathArray = AStar.FindPath(startNode, goalNode);
        
        if (pathArray != null)
        {
            StartCoroutine(FindNextPoint());
        }
    }

    IEnumerator FindNextPoint()
    {
        pathArray.RemoveAt(0);
        
        if(pathArray.Count == 0)
        {
            yield break;
        }

        nextNode = (Node)pathArray[0];

        Quaternion targetRotation = Quaternion.LookRotation(nextNode.position - transform.position);
        curMovementSpeed = 0;

        while(Quaternion.Angle(transform.rotation, targetRotation) > 1.0f)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * rotSpeed);
            yield return null;
        }

        curMovementSpeed = movementSpeed;
    }

    void OnDrawGizmos()
    {
        if (pathArray == null)
            return;

        if (pathArray.Count > 0)
        {
            int index = 1;
            foreach (Node node in pathArray)
            {
                if (index < pathArray.Count)
                {
                    Node nextNode = (Node)pathArray[index];
                    Debug.DrawLine(node.position, nextNode.position, Color.green);
                    index++;
                }
            };
        }
    }
}

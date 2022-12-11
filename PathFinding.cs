using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinding : MonoBehaviour
{
    public GameObject MainCamera;
    public Transform Player;
    public Transform Target;
    public Animator PlayerAnimator;
    float FindCoolDown;
    bool IsMoving;
    bool CancelMove;
    Grid grid;

    private void Awake()
    {
        grid = GetComponent<Grid>();
    }
    private void Start()
    {
        MainCamera.GetComponent<CameraMove>().enabled = true;        
    }
    private void Update()
    {
        if(Input.GetMouseButtonDown(0) && FindCoolDown >0.1f)
        {
            FindCoolDown = 0;
            Target.gameObject.SetActive(true);
            Target.position = FindTarget();
            FindPath(Player.position, Target.position);
        }
        FindCoolDown += Time.deltaTime;
        if(IsMoving)
        {
            WalkToTarget(grid.Path);
        }
    }

    Vector3 FindTarget()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            if (Input.GetMouseButtonDown(0) && hit.transform.gameObject.tag == "floor")
            {
                return new Vector3(hit.point.x, 1, hit.point.z);
            }
            else
            {
                return Target.transform.position;
            }
        }
        else
        {
            return Target.transform.position;
        }
    }
 

    void FindPath(Vector3 StartPos,Vector3 TargetPos)
    {
        Nodes StartNode = grid.NodeFromWorldPoint(StartPos);
        Nodes TargetNode = grid.NodeFromWorldPoint(TargetPos);
        Target.position = new Vector3(TargetNode.WorldPosition.x,1, TargetNode.WorldPosition.z);
        List<Nodes> OpenSet = new List<Nodes>();
        HashSet<Nodes> ClosedSet = new HashSet<Nodes>();
        OpenSet.Add(StartNode);
        while (OpenSet.Count > 0)
        {
            Nodes CurrentNode = OpenSet[0];
            for (int i = 1; i < OpenSet.Count; i++)
            {
                if (OpenSet[i].fCost < CurrentNode.fCost || OpenSet[i].fCost == CurrentNode.fCost && OpenSet[i].hCost <CurrentNode.hCost)
                {
                    CurrentNode = OpenSet[i];
                }
            }
            OpenSet.Remove(CurrentNode);
            ClosedSet.Add(CurrentNode);
            if (CurrentNode == TargetNode)
            {
                RetracePath(StartNode, TargetNode);
                return;
            }
            foreach (Nodes Neighbour in grid.GetNeighbours(CurrentNode))
            {
                if (!Neighbour.Can_Walk || ClosedSet.Contains(Neighbour))
                {
                    continue;
                }
                int NewMovementCost = CurrentNode.gCost + GetDistance(CurrentNode, Neighbour);
                if(NewMovementCost < Neighbour.gCost || !OpenSet.Contains(Neighbour))
                {
                    Neighbour.gCost = NewMovementCost;
                    Neighbour.hCost = GetDistance(Neighbour, TargetNode);
                    Neighbour.Parent = CurrentNode;
                    if (!OpenSet.Contains(Neighbour))
                    {
                        OpenSet.Add(Neighbour);
                    }
                }
            }
        }
    }
    void RetracePath(Nodes startNode,Nodes endNode)
    {
        if(IsMoving)
        {
            CancelMove = true;
        }
        List<Nodes> Path = new List<Nodes>();
        Nodes CurrentNode = endNode;
        while (CurrentNode != startNode)
        {
            Path.Add(CurrentNode);
            CurrentNode = CurrentNode.Parent;
        }
        Path.Reverse();
        grid.Path = Path;
        IsMoving = true;
        PlayerAnimator.SetBool("Walk", true);
        if(Path.Count > 0)
        {
            LookAtWalkDirection(Player, Path[0]);
        }
    }
    int GetDistance(Nodes NodeA,Nodes NodeB)
    {
        int DistanceX = Mathf.Abs(NodeA.gridX - NodeB.gridX);
        int DistanceY = Mathf.Abs(NodeA.gridY - NodeB.gridY);
        if (DistanceX > DistanceY)
        {
            return 14 * DistanceY + 10 * (DistanceX - DistanceY);
        }
        return 14 * DistanceX + 10 * (DistanceY - DistanceX);
    }

    void WalkToTarget(List<Nodes> Path)
    {
        if (Path.Count !=0)
        {
            Player.position = Vector3.MoveTowards(Player.position, Path[0].WorldPosition, 2 * Time.deltaTime);
            
            if(Vector3.Distance(Player.position, Path[0].WorldPosition)==0)
            {
                Path.RemoveAt(0);
                if(Path.Count !=0)
                {
                    LookAtWalkDirection(Player, Path[0]);
                }
            }            
            if(CancelMove)
            {
                CancelMove = false;
                return;
            }
        }
        else
        {
            IsMoving = false;
            PlayerAnimator.SetBool("Walk", false);
            Target.gameObject.SetActive(false);
        }
    }

    void LookAtWalkDirection(Transform Player,Nodes NodeB)
    {
        if(Player.position.x > NodeB.WorldPosition.x && Player.position.z > NodeB.WorldPosition.z)
        {
            Player.rotation = Quaternion.Euler(0, 225,0);          
        }
        else if (Player.position.x > NodeB.WorldPosition.x && Player.position.z == NodeB.WorldPosition.z)
        {
            Player.rotation = Quaternion.Euler(0, -90, 0);
        }
        else if (Player.position.x > NodeB.WorldPosition.x && Player.position.z < NodeB.WorldPosition.z)
        {
            Player.rotation = Quaternion.Euler(0, -45, 0);
        }
        else if (Player.position.x < NodeB.WorldPosition.x && Player.position.z > NodeB.WorldPosition.z)
        {
            Player.rotation = Quaternion.Euler(0, 135,  0);
        }
        else if (Player.position.x < NodeB.WorldPosition.x && Player.position.z == NodeB.WorldPosition.z)
        {
            Player.rotation = Quaternion.Euler(0, 90,0);
        }
        else if (Player.position.x < NodeB.WorldPosition.x && Player.position.z < NodeB.WorldPosition.z)
        {
            Player.rotation = Quaternion.Euler(0, 45, 0);
        }
        else if (Player.position.x == NodeB.WorldPosition.x && Player.position.z > NodeB.WorldPosition.z)
        {
            Player.rotation = Quaternion.Euler(0, -180,  0);
        }
        else if (Player.position.x == NodeB.WorldPosition.x && Player.position.z < NodeB.WorldPosition.z)
        {
            Player.rotation = Quaternion.Euler(0, 0, 0);
        }
    }
}

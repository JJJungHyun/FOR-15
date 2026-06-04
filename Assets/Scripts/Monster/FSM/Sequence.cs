using System.Collections.Generic;
using UnityEngine;

public class Sequence : Node
{
    private List<Node> children;
    public Sequence(List<Node> nodes) => children = nodes;
    public override NodeState Evaluate()
    {
        foreach (var node in children)
        {
            var result = node.Evaluate();
            if (result != NodeState.Success) return result;
        }
        return NodeState.Success;
    }
}
using System.Collections.Generic;
using UnityEngine;

public class Selector : Node
{
    private List<Node> children;
    public Selector(List<Node> nodes) => children = nodes;
    public override NodeState Evaluate()
    {
        foreach (var node in children)
        {
            var result = node.Evaluate();
            if (result != NodeState.Failure) return result;
        }
        return NodeState.Failure;
    }
}
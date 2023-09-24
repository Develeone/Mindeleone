using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodesFiltersController : MonoBehaviour
{
    public static NodesFiltersController instance;

    public List<Node.NodeStatus> visibleNodeStatuses;

    private void Awake()
    {
        instance = this;
    }

    internal static void Refresh()
    {
        foreach (Node node in NodesFactory.instance.nodes) {
            node.SetVisibility(instance.visibleNodeStatuses.Contains(node.status));
        }
    }
}

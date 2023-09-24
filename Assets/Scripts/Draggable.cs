using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Draggable : MonoBehaviour
{
    private Vector2 lastMousePosition;
    private Node node;
    private Vector2 grabPivotOffset;
    private Camera cam;

    private void Start()
    {
        node = GetComponent<Node>();
        cam = Camera.main;
    }

    private void OnMouseDown()
    {
        grabPivotOffset = (Vector2) transform.position - GetMousePosition();
    }

    private void OnMouseDrag()
    {
        CameraMovement.instance.isMovementAvailable = false;
        node.position = GetMousePosition() + grabPivotOffset;
    }

    Vector2 GetMousePosition() {
        return (Vector2) cam.ScreenToWorldPoint(Input.mousePosition);
    }
}

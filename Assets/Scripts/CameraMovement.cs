using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    internal static CameraMovement instance;

    public float zoom {
        get { return camera.orthographicSize; }
        set { camera.orthographicSize = value; }
    }

    public float scrollSensivity = 0.0001f;
    public float zoomSensivity = 0.01f;
    public float scrollBorderAreaWidth = 100f;

    public KeyCode masterKey = KeyCode.LeftControl;
    public KeyCode movementKey = KeyCode.Mouse2;
    public KeyCode searchNodeKey = KeyCode.F;

    Camera camera;
    private Vector2 lastMousePosition;
    public bool isMovementAvailable;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        camera = GetComponent<Camera>();
    }

    void Update()
    {
        // Focus node
        if (Input.GetKey(masterKey) && Input.GetKeyDown(searchNodeKey) && NodesFactory.selectedNode != null)
            FocusNode(NodesFactory.selectedNode);

        // Zoom
        if (Input.mouseScrollDelta != Vector2.zero && !Input.GetKey(KeyCode.LeftShift))
            zoom -= Input.mouseScrollDelta.y * zoomSensivity * zoom;

        // Reset last mouse position on mouse down
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            lastMousePosition = Input.mousePosition;
            isMovementAvailable = true;
        }

        // Move camera if cursor is not over any Node
        if (Input.GetKey(KeyCode.Mouse0) && isMovementAvailable)
        {
            Vector2 mouseDelta = lastMousePosition - (Vector2)Input.mousePosition;
            transform.position += (Vector3)mouseDelta * scrollSensivity * zoom;
            lastMousePosition = Input.mousePosition;
        }
    }

    public void FocusNode(Node selectedNode)
    {
        transform.position = new Vector3(NodesFactory.selectedNode.position.x, NodesFactory.selectedNode.position.y, transform.position.z);
        zoom = NodesFactory.selectedNode.size * 2f;
    }
}

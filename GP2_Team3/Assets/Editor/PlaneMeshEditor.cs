using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(PlaneMesh))]
[CanEditMultipleObjects]
public class PlaneMeshEditor : Editor
{

    public override void OnInspectorGUI()
    {
        var mesh = (PlaneMesh)target;
        base.OnInspectorGUI();

        if (GUI.changed)
        {
            mesh.Rebuild();
            EditorUtility.SetDirty(target);
        }
    }

    private void OnSceneGUI()
    {
        var mesh = (PlaneMesh)target;
        if (Event.current.commandName == "UndoRedoPerformed")
        {
            EditorUtility.SetDirty(target);
            mesh.Rebuild();
        }

        Undo.RecordObject(mesh, "Edit dimensions");
        Handles.color = Color.green;
        mesh.Dimensions.x = Handles.FreeMoveHandle(mesh.transform.position + mesh.transform.right * mesh.Dimensions.x / 2, Quaternion.identity, 0.05f, Vector3.one / 2, Handles.DotCap).x * 2;
        mesh.Dimensions.x = Handles.FreeMoveHandle(mesh.transform.position - mesh.transform.right * mesh.Dimensions.x / 2, Quaternion.identity, 0.05f, Vector3.one / 2, Handles.DotCap).x * -2;
        mesh.Dimensions.y = Handles.FreeMoveHandle(mesh.transform.position + mesh.transform.forward * mesh.Dimensions.y / 2, Quaternion.identity, 0.05f, Vector3.one / 2, Handles.DotCap).z * 2;
        mesh.Dimensions.y = Handles.FreeMoveHandle(mesh.transform.position - mesh.transform.forward * mesh.Dimensions.y / 2, Quaternion.identity, 0.05f, Vector3.one / 2, Handles.DotCap).z * -2;

        Handles.DrawLine(mesh.transform.position + mesh.transform.right * mesh.Dimensions.x / 2 + mesh.transform.forward * mesh.Dimensions.y / 2, mesh.transform.position + mesh.transform.right * mesh.Dimensions.x / 2 - mesh.transform.forward * mesh.Dimensions.y / 2);
        Handles.DrawLine(mesh.transform.position + mesh.transform.right * mesh.Dimensions.x / 2 - mesh.transform.forward * mesh.Dimensions.y / 2, mesh.transform.position - mesh.transform.right * mesh.Dimensions.x / 2 - mesh.transform.forward * mesh.Dimensions.y / 2);
        Handles.DrawLine(mesh.transform.position - mesh.transform.right * mesh.Dimensions.x / 2 - mesh.transform.forward * mesh.Dimensions.y / 2, mesh.transform.position - mesh.transform.right * mesh.Dimensions.x / 2 + mesh.transform.forward * mesh.Dimensions.y / 2);
        Handles.DrawLine(mesh.transform.position - mesh.transform.right * mesh.Dimensions.x / 2 + mesh.transform.forward * mesh.Dimensions.y / 2, mesh.transform.position + mesh.transform.right * mesh.Dimensions.x / 2 + mesh.transform.forward * mesh.Dimensions.y / 2);


        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
            mesh.Rebuild();
        }
    }

    private Vector2 Vec3DToVec2D(Vector3 vector)
    {

        return new Vector2(vector.x, vector.z);
    }

}

using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Mirror
{
    [CustomEditor(typeof(NetworkBehaviour), true)]
    [CanEditMultipleObjects]
    public class NetworkBehaviourInspector : Editor
    {
        Type scriptClass;
        bool syncsAnything;
        SyncObjectCollectionsDrawer syncObjectCollectionsDrawer;

        // does this type sync anything? otherwise we don't need to show syncInterval
        bool SyncsAnything()
        {
            // check for all SyncVar fields, they don't have to be visible
            foreach (FieldInfo field in InspectorHelper.GetAllFields(scriptClass, typeof(NetworkBehaviour)))
            {
                if (field.IsSyncVar())
                {
                    return true;
                }
            }

            // has OnSerialize that is not in NetworkBehaviour?
            // then it either has a syncvar or custom OnSerialize. either way
            // this means we have something to sync.
            MethodInfo method = scriptClass.GetMethod("OnSerialize");
            if (method != null && method.DeclaringType != typeof(NetworkBehaviour))
            {
                return true;
            }

            // SyncObjects are serialized in NetworkBehaviour.OnSerialize, which
            // is always there even if we don't use SyncObjects. so we need to
            // search for SyncObjects manually.
            // Any SyncObject should be added to syncObjects when unity creates an
            // object so we can check length of list so see if sync objects exists
            return ((NetworkBehaviour)serializedObject.targetObject).HasSyncObjects();
        }

        void OnEnable()
        {
            // sometimes target is null. just return early.
            if (target == null) return;

            // If target's base class is changed from NetworkBehaviour to MonoBehaviour
            // then Unity temporarily keep using this Inspector causing things to break
            if (!(target is NetworkBehaviour)) { return; }

            scriptClass = target.GetType();

            syncObjectCollectionsDrawer = new SyncObjectCollectionsDrawer(serializedObject.targetObject);

            syncsAnything = SyncsAnything();
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            DrawSyncObjectCollections();
            DrawDefaultSyncSettings();
        }

        // Draws Sync Objects that are IEnumerable
        protected void DrawSyncObjectCollections()
        {
            // Need this check in case OnEnable returns early
            if (syncObjectCollectionsDrawer == null) return;

            syncObjectCollectionsDrawer.Draw();
        }

        // Draws SyncSettings if the NetworkBehaviour has anything to sync
        protected void DrawDefaultSyncSettings()
        {
            // does it sync anything? then show extra properties
            // (no need to show it if the class only has Cmds/Rpcs and no sync)
            if (!syncsAnything)
            {
                return;
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Sync Settings", EditorStyles.boldLabel);

            // sync direction
            SerializedProperty syncDirection = serializedObject.FindProperty("syncDirection");
            EditorGUILayout.PropertyField(syncDirection);

            // sync mdoe: only show for ServerToClient components
            if (syncDirection.enumValueIndex == (int)SyncDirection.ServerToClient)
                EditorGUILayout.PropertyField(serializedObject.FindProperty("syncMode"));

            // sync method: Don't show for NT-based components
            if (((NetworkBehaviour)serializedObject.targetObject).showSyncMethod())
            {
                SerializedProperty syncMethod = serializedObject.FindProperty("syncMethod");
                EditorGUILayout.PropertyField(syncMethod);

                // Hybrid sync method: show a warning!
                if (syncMethod.enumValueIndex == (int)SyncMethod.Hybrid)
                    EditorGUILayout.HelpBox("Beware! Hybrid is experimental!\n- Do not use this in production yet!\n- Doesn't support [SyncVars] yet!\n- You need to use OnDe/Serialize manually!", MessageType.Warning);
            }

            // sync interval
            EditorGUILayout.PropertyField(serializedObject.FindProperty("syncInterval"));

            // apply
            serializedObject.ApplyModifiedProperties();
        }
    }
}

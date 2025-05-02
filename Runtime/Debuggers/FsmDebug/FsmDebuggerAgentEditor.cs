using System;
using LogicGamer.Core.Engine.Fsm;
using UnityEditor;
using UnityEngine;

namespace UnityLogicGamer.Runtime.Debuggers.FsmDebug
{
    [CustomEditor(typeof(FsmDebuggerAgent))]
    public class FsmDebuggerAgentEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            FsmDebuggerAgent agent = (FsmDebuggerAgent)target;
            Fsm fsm = agent.Fsm;

            if (fsm == null)
            {
                EditorGUILayout.HelpBox("FSM 尚未绑定", MessageType.Warning);
                return;
            }
            // 强制每帧刷新
            Repaint();
            // “只读”信息字段（不变灰）
            DrawReadOnlyField("状态机名称", fsm.Name);
            DrawReadOnlyField("当前状态", fsm.CurrentState?.GetType().Name ?? "无");
            DrawReadOnlyField("运行状态", fsm.Running ? "正在运行" : "未运行");

            TimeSpan duration = DateTime.Now - fsm.StateStartTime;
            DrawReadOnlyField("当前状态持续时长", $"{duration.TotalSeconds:F2} 秒");

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("包含状态:", EditorStyles.boldLabel);
            foreach (var kv in fsm.States)
            {
                EditorGUILayout.LabelField("    • " + kv.Key.Name);
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("携带数据:", EditorStyles.boldLabel);
            string json = fsm.Userdata?.ToString() ?? "null";
            var style = new GUIStyle(EditorStyles.textArea)
            {
                wordWrap = true,
                normal = { } // 保证颜色正常
            };
            EditorGUILayout.TextArea(json, style,
                GUILayout.Height(Mathf.Clamp(json.Split('\n').Length * 18f, 60f, 400f)));
        }

        private void DrawReadOnlyField(string label, string value)
        {
            var style = new GUIStyle(EditorStyles.textField)
            {
                normal = { }
            };
            EditorGUILayout.TextField(label, value, style);
        }
    }
}

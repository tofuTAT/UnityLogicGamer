using UnityEditor;

namespace UnityLogicGamer.Editor
{
    public interface IDrawEditorView
    {
        void Draw(SerializedProperty property);
    }
}
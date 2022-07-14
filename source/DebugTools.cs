using Godot;

namespace SimGame
{
    class DebugTools
    {
        public static void Assert(bool condition, string message)
        {
#if DEBUG
            if (!condition)
                OS.Alert(message);
#endif
        }

    }
}

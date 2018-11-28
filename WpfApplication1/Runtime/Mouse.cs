using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Runtime.InteropServices;

using InputManager;

namespace HandGestures.Runtime
{
    public enum MOUSE_BUTTON { LEFT, RIGHT, MIDDLE };

    public class Mouse
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        private static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint cButtons, uint dwExtraInfo);
        [DllImport("user32.dll")]
        static extern bool SetCursorPos(int X, int Y);

        protected const Int32 MOUSEEVENTF_ABSOLUTE = 0x8000;
        protected const Int32 MOUSEEVENTF_LEFTDOWN = 0x0002;
        protected const Int32 MOUSEEVENTF_LEFTUP = 0x0004;
        protected const Int32 MOUSEEVENTF_MIDDLEDOWN = 0x0020;
        protected const Int32 MOUSEEVENTF_MIDDLEUP = 0x0040;
        protected const Int32 MOUSEEVENTF_MOVE = 0x0001;
        protected const Int32 MOUSEEVENTF_RIGHTDOWN = 0x0008;
        protected const Int32 MOUSEEVENTF_RIGHTUP = 0x0010;

        private static bool leftPressed = false;
        private static bool rightPressed = false;
        private static bool middlePressed = false;

        public static Point GetPos()
        {
            return Cursor.Position;
        }
        public static void SetPos(int x, int y)
        {
            InputManager.Mouse.Move(x, y);
        }
        public static void SetPos(Point pos)
        {
            //SetCursorPos(pos.X, pos.Y);
            /*uint nx = (uint)(pos.X * (65535 / Screen.PrimaryScreen.Bounds.Width));
            uint ny = (uint)(pos.Y * (65535 / Screen.PrimaryScreen.Bounds.Height));
            mouse_event(MOUSEEVENTF_MOVE | MOUSEEVENTF_ABSOLUTE, nx, ny, 0, 0);*/
            SetPos(pos.X, pos.Y);
        }

        public static void MoveBy(int x, int y)
        {
            InputManager.Mouse.MoveRelative(x, y);
        }
        public static void MoveBy(Point p)
        {
            MoveBy(p.X, p.Y);
        }

        public static void Click(MOUSE_BUTTON button)
        {
            switch (button)
            {
                case MOUSE_BUTTON.LEFT:
                    InputManager.Mouse.PressButton(InputManager.Mouse.MouseKeys.Left);
                    leftPressed = false;
                    break;
                case MOUSE_BUTTON.RIGHT:
                    InputManager.Mouse.PressButton(InputManager.Mouse.MouseKeys.Right);
                    rightPressed = false;
                    break;
                case MOUSE_BUTTON.MIDDLE:
                    InputManager.Mouse.PressButton(InputManager.Mouse.MouseKeys.Middle);
                    middlePressed = false;
                    break;
            }
        }
        public static void Press(MOUSE_BUTTON button)
        {
            switch (button)
            {
                case MOUSE_BUTTON.LEFT:
                    if (!leftPressed)
                    {
                        InputManager.Mouse.ButtonDown(InputManager.Mouse.MouseKeys.Left);
                        leftPressed = true;
                    }
                    break;
                case MOUSE_BUTTON.RIGHT:
                    if (!rightPressed)
                    {
                        InputManager.Mouse.ButtonDown(InputManager.Mouse.MouseKeys.Right);
                        rightPressed = true;
                    }
                    break;
                case MOUSE_BUTTON.MIDDLE:
                    if (!middlePressed)
                    {
                        InputManager.Mouse.ButtonDown(InputManager.Mouse.MouseKeys.Middle);
                        middlePressed = true;
                    }
                    break;
            }
        }
        public static void Release(MOUSE_BUTTON button)
        {
            switch (button)
            {
                case MOUSE_BUTTON.LEFT:
                    if (leftPressed)
                    {
                        InputManager.Mouse.ButtonUp(InputManager.Mouse.MouseKeys.Left);
                        leftPressed = false;
                    }
                    break;
                case MOUSE_BUTTON.RIGHT:
                    if (rightPressed)
                    {
                        InputManager.Mouse.ButtonUp(InputManager.Mouse.MouseKeys.Right);
                        rightPressed = false;
                    }
                    break;
                case MOUSE_BUTTON.MIDDLE:
                    if (middlePressed)
                    {
                        InputManager.Mouse.ButtonUp(InputManager.Mouse.MouseKeys.Middle);
                        middlePressed = false;
                    }
                    break;
            }
        }
    }
}

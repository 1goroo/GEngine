using Microsoft.Xna.Framework.Input;

namespace GEngine.Input
{
    public static class Input
    {
        static KeyboardState currentState = Keyboard.GetState();
        static KeyboardState previousState;
        static MouseState currentMouseState = Mouse.GetState();
        static MouseState previousMouseState;
        /// <summary>
        /// Used by general core Update loop
        /// </summary>
        internal static void Update()
        {
            previousState = currentState;
            currentState = Keyboard.GetState();
            previousMouseState = currentMouseState;
            currentMouseState = Mouse.GetState();
        }
        // Keyboard
        /// <summary>
        /// returns true if the key is pressed
        /// </summary>
        public static bool GetKeyDown(Keys key) => currentState.IsKeyDown(key) && previousState.IsKeyUp(key);

        /// <summary>
        /// returns true if the key is released
        /// </summary>
        public static bool GetKeyUp(Keys key) => currentState.IsKeyUp(key) && previousState.IsKeyDown(key);

        /// <summary>
        /// returns true if the key is pressed and held
        /// </summary>
        public static bool GetKey(Keys key) => currentState.IsKeyDown(key);

        public static bool AnyKey() => currentState.GetPressedKeyCount() > 0;

        public static bool AnyKeyDown() => currentState.GetPressedKeyCount() > 0 && previousState.GetPressedKeyCount() == 0;

        // Mouse
        /// <summary>
        /// returns true if the mouse button is pressed
        /// </summary>
        public static bool GetMouseButtonDown(int button)
        {
            if (button == 0)
                if (currentMouseState.LeftButton == ButtonState.Pressed && previousMouseState.LeftButton == ButtonState.Released) return true;
                else return false;
            else if (button == 1)
                if (currentMouseState.MiddleButton == ButtonState.Pressed && previousMouseState.MiddleButton == ButtonState.Released) return true;
                else return false;
            else if (button == 2)
                if (currentMouseState.RightButton == ButtonState.Pressed && previousMouseState.RightButton == ButtonState.Released) return true;
                else return false;
            else return false;
        }

        /// <summary>
        /// returns true if the mouse button is released
        /// </summary>
        public static bool GetMouseButtonUp(int button)
        {
            if (button == 0)
                if (currentMouseState.LeftButton == ButtonState.Released && previousMouseState.LeftButton == ButtonState.Pressed) return true;
                else return false;
            else if (button == 1)
                if (currentMouseState.MiddleButton == ButtonState.Released && previousMouseState.MiddleButton == ButtonState.Pressed) return true;
                else return false;
            else if (button == 2)
                if (currentMouseState.RightButton == ButtonState.Released && previousMouseState.RightButton == ButtonState.Pressed) return true;
                else return false;
            else return false;
        }

        /// <summary>
        /// returns true if the mouse button is pressed and held
        /// </summary>
        public static bool GetMouseButton(int button)
        {
            if (button == 0)
                if (currentMouseState.LeftButton == ButtonState.Pressed && previousMouseState.LeftButton == ButtonState.Pressed) return true;
                else return false;
            else if (button == 1)
                if (currentMouseState.MiddleButton == ButtonState.Pressed && previousMouseState.MiddleButton == ButtonState.Pressed) return true;
                else return false;
            else if (button == 2)
                if (currentMouseState.RightButton == ButtonState.Pressed && previousMouseState.RightButton == ButtonState.Pressed) return true;
                else return false;
            else return false;
        }

        /// <summary>
        /// returns normalised whell scroll value
        /// </summary>
        public static int GetNormalizedWheelScroll()
        {
            if (currentMouseState.ScrollWheelValue > previousMouseState.ScrollWheelValue) return 1;
            else if (currentMouseState.ScrollWheelValue == previousMouseState.ScrollWheelValue) return 0;
            else return -1;
        }
    }
}

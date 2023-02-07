using Microsoft.Xna.Framework.Input;

namespace modular_synth_frontend.Scripts;

    public class InputManager
{
    MouseState oldState, newState;

    public void Update()
    {
        newState = Mouse.GetState();
        oldState = newState;
    }

    public bool LeftMousePressed()
    {
        if(newState.LeftButton == ButtonState.Pressed)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool LeftMouseClickDown()
    {
        if (newState.LeftButton == ButtonState.Pressed && oldState.LeftButton == ButtonState.Released)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool LeftMouseClickUp()
    {
        if(newState.LeftButton == ButtonState.Released && oldState.LeftButton == ButtonState.Pressed)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}


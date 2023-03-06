using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using modular_synth_frontend.Core;

namespace modular_synth_frontend.UI;

internal class Button : Interactable
{
	public Button(Texture2D sprite, Vector2 position) : base(sprite, position)
	{
		fixedOnScreen = true;
	}
	public Button(Vector2 position) : base(position)
	{
	}

	public override void Update()
	{ }

	//For getting interactions with button and defining them within class that is using them
	public Rectangle getBoundingBox()
	{
		return screenSpaceBoundingBox;
	}

	public void SetInactive()
	{
		colour = Color.DarkGray;
	}

	public void SetActive()
	{
		colour = Color.White;
	}
}

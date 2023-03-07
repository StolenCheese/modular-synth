using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using modular_synth_frontend.Core;
using System;
using System.Diagnostics;

namespace modular_synth_frontend.UI;

internal class ModuleSpawnButton : Button
{
	private readonly Module buttonImage;

	private readonly InputManager input = InputManager.GetInstance();
	private readonly Grid grid = Grid.GetInstance();

	private readonly SectionDef def;

	public static event Action ModuleSpawned;

	public ModuleSpawnButton(Vector2 position, SectionDef def, double scale = 0.5) : base(position)
	{ 
		this.def = def;

		ModuleSpawned += Menu.GetInstance().ChangeState;

		buttonImage = new Module(position, def, false, (float)scale);
		this.width = (int)(scale * buttonImage.sprite.Width);
		this.height = (int)(scale * buttonImage.sprite.Height);
		fixedOnScreen = true;
		buttonImage.fixedOnScreen = true;

		worldSpaceBoundingBox = new Rectangle((int)position.X, (int)position.Y, width, height);
	}

	public Module Spawn()
	{
		ModuleSpawned.Invoke();

		int offset = def.width * grid.GetGridSideLength() / 2;
		Vector2 pos = new(input.MousePosVector().X - offset, input.MousePosVector().Y - (grid.GetGridSideLength() * Grid.ROWS / 2));
		return new Module(pos, def);
	}

	public override void Draw(SpriteBatch spriteBatch)
	{
		buttonImage.Draw(spriteBatch);
	}

	public override void Update()
	{
		if (screenSpaceBoundingBox.Contains(input.MousePosition()))
		{
			if (input.LeftMouseClickDown() && !Menu.justOpen)
			{
				Module newModule = Spawn();
				newModule.Drag();
			}
		}
	}
}

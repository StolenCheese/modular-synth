using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using modular_synth_frontend.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace modular_synth_frontend.UI;

internal class Menu : Interactable
{
	private static Menu instance;

	InputManager input = InputManager.GetInstance();
	private Texture2D boxSprite;

	private Button leftNav;
	private Button rightNav;

	private List<Button> activeButtons;
	private List<Button> visibleButtons;
	private int activeButtonListIndex;
	private bool open;
	public static bool justOpen = true;

	private Texture2D moduleTexture;
	private Texture2D leftArrowTexture;
	private Texture2D rightArrowTexture;

	public static event Action MenuOpened;
	public static event Action MenuClosed;

	const int GAPBETWEENMODULES = 20;
	const int BUTTONSPERSCREEN = 4;

	private Menu(Texture2D boxSprite, Texture2D handleSprite, Vector2 position) : base(handleSprite, position)
	{
		this.boxSprite = boxSprite;
		open = false;
		activeButtonListIndex = 0;

		MenuOpened += EntityManager.DisableEntities;
		MenuClosed += EntityManager.EnableEntities;

		//TODO: Make button spawning proper but for now this will do:
		activeButtons = new List<Button>();
		visibleButtons = new List<Button>();
	}

	public static Menu GetInstance()
	{
		if (instance != null)
		{
			return instance;
		}
		else
		{
			throw new Exception("rip - this is called we didn't plan for singleton and there's like 4 days left so im making terrible code");
		}
	}

	public static Menu CreateInstance(Texture2D boxSprite, Texture2D handleSprite, Vector2 position)
	{
		if (instance != null)
		{
			throw new Exception("rip - this is called we didn't plan for singleton and there's like 4 days left so im making terrible code");
		}
		else
		{
			instance = new Menu(boxSprite, handleSprite, position);
			return instance;
		}
	}

	public void LoadContent()
	{
		moduleTexture = ModularSynth.instance.Content.Load<Texture2D>("module");

		leftArrowTexture = ModularSynth.instance.Content.Load<Texture2D>("left_arrow");
		rightArrowTexture = ModularSynth.instance.Content.Load<Texture2D>("right_arrow");

		leftNav = new Button(leftArrowTexture, new Vector2(70, 280));
		rightNav = new Button(rightArrowTexture, new Vector2(1150, 280));

		activeButtons.Add(new ModuleSpawnButton(new Vector2(120, 10), SectionDefManager.LoadSectionDef("sin-ui")));
		activeButtons.Add(new ModuleSpawnButton(new Vector2(120 + moduleTexture.Width + GAPBETWEENMODULES, 10), SectionDefManager.LoadSectionDef("brownnoise-ui")));
		activeButtons.Add(new ModuleSpawnButton(new Vector2(120 + 2 * (moduleTexture.Width + GAPBETWEENMODULES), 10), SectionDefManager.LoadSectionDef("pan-ui")));
		activeButtons.Add(new ModuleSpawnButton(new Vector2(120 + 3 * (moduleTexture.Width + GAPBETWEENMODULES), 10), SectionDefManager.LoadSectionDef("speaker-ui")));
		activeButtons.Add(new ModuleSpawnButton(new Vector2(120, 10), SectionDefManager.LoadSectionDef("organ-ui")));
		activeButtons.Add(new ModuleSpawnButton(new Vector2(120 + (moduleTexture.Width + GAPBETWEENMODULES), 10), SectionDefManager.LoadSectionDef("sin-control-ui")));
		activeButtons.Add(new ModuleSpawnButton(new Vector2(120 + 2 * (moduleTexture.Width + GAPBETWEENMODULES), 10), SectionDefManager.LoadSectionDef("midi-player-ui")));

		if (activeButtons.Count > BUTTONSPERSCREEN)
		{
			visibleButtons = activeButtons.GetRange(0, BUTTONSPERSCREEN);
		}
		else
		{
			visibleButtons = activeButtons.GetRange(0, activeButtons.Count);
		}

	}

	public override void Update()
	{
		if ((boundingBox.Contains(input.MousePosition()) && input.LeftMouseClickDown()) || input.KeyDown(Microsoft.Xna.Framework.Input.Keys.E))
		{
			ChangeState();
		}

		if (open)
		{
			if (activeButtonListIndex != 0)
			{
				leftNav.SetActive(); //TODO: do these set actives and inactives not every frame
				if (leftNav.getBoundingBox().Contains(input.MousePosition()) && input.LeftMouseClickDown())
				{
					Debug.WriteLine("Clicked Left");

					activeButtonListIndex -= BUTTONSPERSCREEN;

					visibleButtons = activeButtons.GetRange(activeButtonListIndex, BUTTONSPERSCREEN);
				}
			}
			else
			{
				leftNav.SetInactive();
			}

			if (activeButtonListIndex < activeButtons.Count - BUTTONSPERSCREEN)
			{
				rightNav.SetActive();
				if (rightNav.getBoundingBox().Contains(input.MousePosition()) && input.LeftMouseClickDown())
				{
					Debug.WriteLine("Clicked Right");
					activeButtonListIndex += BUTTONSPERSCREEN;

					if (activeButtons.Count > activeButtonListIndex + BUTTONSPERSCREEN)
					{
						visibleButtons = activeButtons.GetRange(activeButtonListIndex, BUTTONSPERSCREEN);
					}
					else
					{
						visibleButtons = activeButtons.GetRange(activeButtonListIndex, activeButtons.Count - activeButtonListIndex);
					}
				}
			}
			else
			{
				rightNav.SetInactive();
			}

			foreach (Button button in visibleButtons)
			{
				button.Update();
			}
		}
	}

	public override void Draw(SpriteBatch spriteBatch)
	{
		spriteBatch.Draw(sprite, GetPosition(), colour);

		if (open)
		{
			spriteBatch.Draw(boxSprite, new Vector2(ModularSynth.viewport.Width / 2 - boxSprite.Width / 2, 0), Color.White);

			leftNav.Draw(spriteBatch);
			rightNav.Draw(spriteBatch);

			foreach (Button button in visibleButtons)
			{
				//Gonna have to have the menu handle how the positioning and everything is done for these (equal spacing and that)
				//TODO: do that 
				button.Draw(spriteBatch);
			}
			if (justOpen)
			{
				justOpen = false;
			}
		}
	}

	public void ChangeState()
	{
		if (open)
		{
			ShiftPosition(0, -ModularSynth.viewport.Height / 2);
			MenuClosed.Invoke();
			open = false;
			justOpen = true;
		}
		else
		{
			ShiftPosition(0, ModularSynth.viewport.Height / 2);
			MenuOpened.Invoke();
			open = true;
		}
	}

}

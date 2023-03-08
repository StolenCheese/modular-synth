using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using modular_synth_frontend.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace modular_synth_frontend.Core;
public class EntityManager
{

	public static List<Entity> entities = new();

	public static bool isMouseOverEntity;
	public static bool isUpdating;

	public static void Update()
	{
		isMouseOverEntity = false;
		isUpdating = true;

		foreach (Entity entity in entities)
		{
			if (entity.enabled)
			{
				entity.Update();
			}
		}

		isUpdating= false;

        entities = entities.Where(x => !x.deleted).ToList();
    }

	public static void Draw(SpriteBatch spriteBatch)
	{
		foreach (Entity entity in entities)
		{
			if (entity.visible)// omitted entity.GetType()!=typeof(Wire) check as we dont add wires to entity manager
			{
				entity.Draw(spriteBatch);
			}

			//render wires at the front by calling this last
		}
		foreach (Port port in Port.ports)
		{
			if (port.portsConnectedTo.Count != 0)
			{
				foreach (var portWire in port.portsConnectedTo)
				{
					portWire.Value.Position = port.Position;
					portWire.Value.endPosition = portWire.Key.Position;
					portWire.Value.Draw(spriteBatch);
				}
			}
			if (port.dragging)
			{
				port.draggingWire.Draw(spriteBatch);
			}
		}

        
        //error messages
        if(Menu.warningDuration > 0){
                spriteBatch.Draw(Menu.cycleWarningTexture,new Vector2((ModularSynth.viewport.Width-Menu.cycleWarningTexture.Width)/ 2, ModularSynth.viewport.Height / 2+200), Color.White);

                Menu.warningDuration--;
        }

	}

	public static void DisableEntities()
	{
		foreach (Entity entity in entities)
		{
			entity.enabled = false;
		}
	}

	public static void EnableEntities()
	{
		foreach (Entity entity in entities)
		{
			entity.enabled = true;
		}
	}
}
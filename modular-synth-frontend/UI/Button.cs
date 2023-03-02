using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using modular_synth_frontend.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace modular_synth_frontend.UI;

internal abstract class Button : Interactable
{
    public Button(Texture2D sprite, Vector2 position) : base(sprite, position)
    {
    }
}

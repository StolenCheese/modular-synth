using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace modular_synth_frontend.UI;

internal class NavButton : Button
{
    private List<Button> NavigatesTo;

    public NavButton(Texture2D sprite, Vector2 position) : base(sprite, position)
    {
    }

    public override void Update()
    {
        throw new NotImplementedException();
    }
}

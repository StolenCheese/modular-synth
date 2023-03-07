using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace modular_synth_frontend.UI;

//unused class, rip - was gonna be used when we thought we'd separate modules into categories
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

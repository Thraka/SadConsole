
class CustomRenderer2 : DrawableGameComponent
{
    Effect spriteEffect;

    public CustomRenderer2(Microsoft.Xna.Framework.Game game) : base(game)
    {
        Settings.DoFinalDraw = false;
        DrawOrder = 6;
        spriteEffect = SadConsole.Game.Instance.Content.Load<Effect>("CRT2");
    }

    protected override void UnloadContent()
    {
        spriteEffect.Dispose();
    }

    public override void Draw(GameTime gameTime)
    {
        // Respect the draw flag for sadconsole
        if (Settings.DoDraw)
        {
            spriteEffect.Parameters["textureSize"].SetValue(new Vector2(Global.RenderOutput.Width, Global.RenderOutput.Height));
            spriteEffect.Parameters["videoSize"].SetValue(new Vector2(Global.RenderOutput.Width, Global.RenderOutput.Height));
            spriteEffect.Parameters["outputSize"].SetValue(new Vector2(Global.RenderRect.Width, Global.RenderRect.Height));

            Global.SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.DepthRead, RasterizerState.CullNone);
            spriteEffect.CurrentTechnique.Passes[0].Apply();

            Global.SpriteBatch.Draw(Global.RenderOutput, Global.RenderRect, Color.White);
            Global.SpriteBatch.End();
        }
    }
}

class CustomRenderer : DrawableGameComponent
{
    Effect spriteEffect;
    bool effect_toggle = false;

    public CustomRenderer(Microsoft.Xna.Framework.Game game) : base(game)
    {
        // Disable final draw from the core game component (used by SadConsole.Game.SadConsoleGameComponent)
        Settings.DoFinalDraw = false;

        // Draw after SadConsole.Game.SadConsoleGameComponent
        DrawOrder = 6;

        // Load the effect (This is OK to do here in the case of SadConsole)
        spriteEffect = SadConsole.Game.Instance.Content.Load<Effect>("CRT");

        spriteEffect.Parameters["hardScan"]?.SetValue(-20.0f);
        spriteEffect.Parameters["hardPix"]?.SetValue(-10.0f);
        spriteEffect.Parameters["warpX"]?.SetValue(0.011f); // 0.031
        spriteEffect.Parameters["warpY"]?.SetValue(0.0f); // 0.041
        spriteEffect.Parameters["maskDark"]?.SetValue(1f);
        spriteEffect.Parameters["maskLight"]?.SetValue(1f);
        spriteEffect.Parameters["scaleInLinearGamma"]?.SetValue(1.0f);
        spriteEffect.Parameters["shadowMask"]?.SetValue(3.0f);
        spriteEffect.Parameters["brightboost"]?.SetValue(2.0f);
        spriteEffect.Parameters["hardBloomScan"]?.SetValue(-10.0f);
        spriteEffect.Parameters["hardBloomPix"]?.SetValue(-2.0f);
        spriteEffect.Parameters["bloomAmount"]?.SetValue(0.8f);
        spriteEffect.Parameters["shape"]?.SetValue(1.0f);
    }

    protected override void LoadContent()
    {
        // Do not use LoadContent in this case. The SadConsoleGame object will not call this
        // if you add the component in the game OnInitialize() callback.
    }

    protected override void UnloadContent()
    {
        spriteEffect.Dispose();
    }

    public override void Draw(GameTime gameTime)
    {
        // Respect the draw flag for sadconsole
        if (Settings.DoDraw)
        {
            if (SadConsole.Global.KeyboardState.IsKeyReleased(Microsoft.Xna.Framework.Input.Keys.E))
                effect_toggle = !effect_toggle;

            spriteEffect.Parameters["textureSize"].SetValue(new Vector2(Global.RenderOutput.Width, Global.RenderOutput.Height));
            spriteEffect.Parameters["videoSize"].SetValue(new Vector2(Global.RenderOutput.Width, Global.RenderOutput.Height));
            spriteEffect.Parameters["outputSize"].SetValue(new Vector2(Global.RenderRect.Width, Global.RenderRect.Height));

            Global.SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.DepthRead, RasterizerState.CullNone);

            if (effect_toggle)
                spriteEffect.CurrentTechnique.Passes[0].Apply();

            Global.SpriteBatch.Draw(Global.RenderOutput, Global.RenderRect, Color.White);
            Global.SpriteBatch.End();
        }
    }
}
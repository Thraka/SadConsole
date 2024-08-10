#if MONOGAME
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Color = Microsoft.Xna.Framework.Color;

namespace SadConsole.Host;

public class PostProcessingFX : DrawableGameComponent
{
    public Effect Effect;
    private static int width = 960;
    private static int height = 720;

    public PostProcessingFX() : base((Microsoft.Xna.Framework.Game)SadConsole.Game.Instance.MonoGameInstance)
    {
        DrawOrder = 6;

        Effect = SadConsole.Game.Instance.MonoGameInstance.Content.Load<Effect>("crt-lottes-mg");
    }

    protected override void LoadContent()
    {
        base.LoadContent();
    }

    public override void Draw(GameTime gameTime)
    {
        if (Visible)
        {
            // Respect the draw flag for sadconsole 
            width = Host.Global.RenderOutput.Width;
            height = Host.Global.RenderOutput.Height;


            Effect.Parameters["hardScan"]?.SetValue(-8.0f);
            Effect.Parameters["hardPix"]?.SetValue(-3.0f);
            Effect.Parameters["warpX"]?.SetValue(.031f);
            Effect.Parameters["warpY"]?.SetValue(0.041f);
            Effect.Parameters["maskDark"]?.SetValue(0.5f);
            Effect.Parameters["maskLight"]?.SetValue(1.5f);
            Effect.Parameters["scaleInLinearGamma"]?.SetValue(1.0f);
            Effect.Parameters["shadowMask"]?.SetValue(3.0f);
            Effect.Parameters["brightboost"]?.SetValue(1.0f);
            Effect.Parameters["hardBloomScan"]?.SetValue(-1.5f);
            Effect.Parameters["hardBloomPix"]?.SetValue(-2.0f);
            Effect.Parameters["bloomAmount"]?.SetValue(2f);
            Effect.Parameters["shape"]?.SetValue(2.0f);


            Effect.Parameters["brightboost"].SetValue(0.5f);
            Effect.Parameters["textureSize"].SetValue(new Vector2(width, height));
            Effect.Parameters["outputSize"].SetValue(new Vector2(SadConsole.Settings.Rendering.RenderRect.Width, SadConsole.Settings.Rendering.RenderRect.Height));
            Effect.Parameters["videoSize"].SetValue(new Vector2(width, height));

            Host.Global.SharedSpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.DepthRead, RasterizerState.CullNone);

            //Apply the shader before draw, but after begin.
            Effect.CurrentTechnique.Passes[0].Apply();

            Host.Global.SharedSpriteBatch.Draw(Host.Global.RenderOutput, SadConsole.Settings.Rendering.RenderRect.ToMonoRectangle(), Color.White);
            Host.Global.SharedSpriteBatch.End();
        }
    }
}
#endif

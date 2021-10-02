﻿using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using SadConsole.Host;

namespace SadConsole.DrawCalls
{
    /// <summary>
    /// Helps manage the <see cref="Global.SharedSpriteBatch"/> while draw calls are drawing.
    /// </summary>
    public static class DrawCallManager
    {
        /// <summary>
        /// Called 
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ResumeBatch()
        {
            Global.GraphicsDevice.SetRenderTarget(Global.RenderOutput);
            Global.SharedSpriteBatch.Begin(SpriteSortMode.Deferred, SadConsole.Host.Settings.MonoGameScreenBlendState, SamplerState.PointClamp, DepthStencilState.DepthRead, RasterizerState.CullNone);
        }

        /// <summary>
        /// Ends the <see cref="Global.SharedSpriteBatch"/> so another can be started, perhaps with an effect. Render target remains <see cref="Global.RenderOutput"/>.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void InterruptBatch()
        {
            Global.SharedSpriteBatch.End();
        }
    }
}

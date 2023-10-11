namespace SadConsole.Configuration;

internal static class MonoGameDebuggerExtensions
{
    public static Builder KeyhookMonoGameDebugger(this Builder builder)
    {
        builder.GetOrCreateConfig<MonoGameDebugger>();
        return builder;
    }

    internal static void Game_FrameUpdate(object? sender, GameHost e)
    {
        if (e.Keyboard.IsKeyPressed(Input.Keys.F12))
        {
#if PROJREFS && MONOGAME
            SadConsole.Debug.MonoGame.Debugger.Start();
#endif
        }
    }
}

internal class MonoGameDebugger : IConfigurator
{
    public void Run(Builder config, Game game) =>
        game.FrameUpdate += MonoGameDebuggerExtensions.Game_FrameUpdate;
}

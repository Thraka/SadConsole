using System;

namespace SadConsole.DrawCalls
{
    public class DrawCallCustom : IDrawCall
    {
        public Action DrawCallback;

        public DrawCallCustom(Action draw) => DrawCallback = draw;

        public void Draw() => DrawCallback();
    }
}
namespace ZZTGame.ObjectComponents;

interface ITick
{
    int Tick { get; set;  }

    void Action(Screens.Board board, GameObject obj);
}

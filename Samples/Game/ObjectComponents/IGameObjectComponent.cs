namespace ZZTGame.ObjectComponents;

interface IGameObjectComponent
{
    void Added(GameObject obj);
    void Removed(GameObject obj);
}

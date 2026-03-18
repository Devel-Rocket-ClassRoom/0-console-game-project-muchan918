namespace Framework.Engine
{
    // 씬이 업로드 됐다 -> 게임 내의 이벤트
    // 키가 눌렸다는 이벤트
    // 들을 처리하는 GameAction
    public delegate void GameAction();
    public delegate void GameAction<T>(T value);
}

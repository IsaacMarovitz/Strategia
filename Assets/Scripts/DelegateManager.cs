using System;

public static class DelegateManager {
    public static Action<Player> playerTurnWaitDelegate;
    public static Action<Player> playerTurnStartDelegate;
    public static Action<Player> playerTurnCompleteDelegate;
    public static Action<Player> playerTurnEndDelegate;

    public static Action<Unit> unitTurnStartDelegate;
    public static Action<Unit> unitMoveDelegate;
    public static Action unitActionDelegate;
    
    public static Action<Player> fogOfWarUpdateDelegate;

    public static Action<Unit> unitSelectedDelegate;
    public static Action unitDeselectedDelegate;
    
    public static Action<City> citySelectedDelegate;
    public static Action cityDeselectedDelegate;

    public static Action<Tile> mouseOverTileSelectedDelegate;
    public static Action mouseOverTileDeselectedDelegate;
}
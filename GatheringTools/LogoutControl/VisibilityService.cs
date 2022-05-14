namespace GatheringTools.LogoutControl
{
    public class VisibilityService
    {
        public static bool ShouldBeVisible(bool show,
                                           bool showOnMap,
                                           bool showOnCharSelectAndCutScene,
                                           bool isInGame,
                                           bool mapIsClosed)
        {
            return show 
                   && (isInGame || showOnCharSelectAndCutScene) 
                   && (mapIsClosed || showOnMap);
        }
    }
}
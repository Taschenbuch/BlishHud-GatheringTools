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
            if (show == false)
                return false;

            return (isInGame || showOnCharSelectAndCutScene) && (mapIsClosed || showOnMap);
        }
    }
}
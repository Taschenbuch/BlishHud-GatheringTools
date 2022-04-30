using Blish_HUD.Controls;
using Microsoft.Xna.Framework;

namespace GatheringTools.ToolSearch.Controls
{
    public class LoadingSpinnerContainer : Container // Container is abstract class. cannot use it directly 
    {
        public LoadingSpinnerContainer()
        {
            new LoadingSpinner
            {
                Location = new Point(60, 0),
                Parent   = this
            };
        }
    }
}
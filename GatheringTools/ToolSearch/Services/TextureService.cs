using System;
using Blish_HUD.Modules.Managers;
using Microsoft.Xna.Framework.Graphics;

namespace GatheringTools.ToolSearch.Services
{
    public class TextureService : IDisposable
    {
        public TextureService(ContentsManager contentsManager)
        {
            ReminderBackgroundTexture = contentsManager.GetTexture(@"logoutOverlay\logoutDialogTextArea.png");
            Tool1Texture              = contentsManager.GetTexture(@"logoutOverlay\tool1_1998933.png");
            Tool2Texture              = contentsManager.GetTexture(@"logoutOverlay\tool2_1998934.png");
            Tool3Texture              = contentsManager.GetTexture(@"logoutOverlay\tool3_1998935.png");

            WindowBackgroundTexture   = contentsManager.GetTexture(@"toolSearch\windowsBackground_155985.png");
            ToolSearchWindowEmblem    = contentsManager.GetTexture(@"toolSearch\sickle.png");
            CornerIconTexture         = contentsManager.GetTexture(@"toolSearch\sickle.png");
            HoverCornerIconTexture    = contentsManager.GetTexture(@"toolSearch\sickle_hover.png");
            BankTexture               = contentsManager.GetTexture(@"toolSearch\bank_156670.png");
            CharacterInventoryTexture = contentsManager.GetTexture(@"toolSearch\inventory_157098.png");
            SharedInventoryTexture    = contentsManager.GetTexture(@"toolSearch\sharedInventory.png");
            EquipmentTexture          = contentsManager.GetTexture(@"toolSearch\equipment_156714.png");
            UnknownToolTexture        = contentsManager.GetTexture(@"toolSearch\unknownTool_66591.png");
        }

        public void Dispose()
        {
            ReminderBackgroundTexture?.Dispose();
            Tool1Texture?.Dispose();
            Tool2Texture?.Dispose();
            Tool3Texture?.Dispose();

            WindowBackgroundTexture?.Dispose();
            ToolSearchWindowEmblem?.Dispose();
            CornerIconTexture?.Dispose();
            HoverCornerIconTexture?.Dispose();
            BankTexture?.Dispose();
            CharacterInventoryTexture?.Dispose();
            SharedInventoryTexture?.Dispose();
            EquipmentTexture?.Dispose();
            UnknownToolTexture?.Dispose();
        }

        public Texture2D ReminderBackgroundTexture { get; }
        public Texture2D Tool1Texture { get; }
        public Texture2D Tool2Texture { get; }
        public Texture2D Tool3Texture { get; }
        public Texture2D WindowBackgroundTexture { get; }
        public Texture2D ToolSearchWindowEmblem { get; }
        public Texture2D CornerIconTexture { get; }
        public Texture2D HoverCornerIconTexture { get; }
        public Texture2D BankTexture { get; }
        public Texture2D CharacterInventoryTexture { get; }
        public Texture2D SharedInventoryTexture { get; }
        public Texture2D EquipmentTexture { get; }
        public Texture2D UnknownToolTexture { get; }
    }
}
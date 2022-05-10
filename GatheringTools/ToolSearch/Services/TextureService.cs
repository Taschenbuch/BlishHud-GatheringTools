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
            ReminderIconTexture       = contentsManager.GetTexture(@"logoutOverlay\reminderIcon.png");
            WindowBackgroundTexture   = contentsManager.GetTexture(@"toolSearch\textures\windowsBackground_155985.png");
            ToolSearchWindowEmblem    = contentsManager.GetTexture(@"toolSearch\textures\toolSearchWindowEmblem.png");
            CornerIconTexture         = contentsManager.GetTexture(@"toolSearch\textures\cornerIcon.png");
            HoverCornerIconTexture    = contentsManager.GetTexture(@"toolSearch\textures\cornerIcon_hover.png");
            BankTexture               = contentsManager.GetTexture(@"toolSearch\textures\bank_156670.png");
            CharacterInventoryTexture = contentsManager.GetTexture(@"toolSearch\textures\inventory_157098.png");
            SharedInventoryTexture    = contentsManager.GetTexture(@"toolSearch\textures\sharedInventory.png");
            EquipmentTexture          = contentsManager.GetTexture(@"toolSearch\textures\equipment_156714.png");
            UnknownToolTexture        = contentsManager.GetTexture(@"toolSearch\textures\unknownTool_66591.png");
        }

        public void Dispose()
        {
            ReminderBackgroundTexture?.Dispose();
            ReminderIconTexture?.Dispose();
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
        public Texture2D ReminderIconTexture { get; }
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
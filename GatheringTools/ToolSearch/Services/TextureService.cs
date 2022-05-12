using System;
using Blish_HUD.Modules.Managers;
using Microsoft.Xna.Framework.Graphics;

namespace GatheringTools.ToolSearch.Services
{
    public class TextureService : IDisposable
    {
        public TextureService(ContentsManager contentsManager)
        {
            LogoutButtonTexture              = contentsManager.GetTexture(@"reminder\logoutButton.png");
            LogoutButtonActiveTexture        = contentsManager.GetTexture(@"reminder\logoutButtonActive.png");
            ReminderBackgroundTexture        = contentsManager.GetTexture(@"reminder\logoutDialogTextArea.png");
            ReminderImageNoOutlineTexture    = contentsManager.GetTexture(@"reminder\reminderImage_noOutline.png");
            ReminderImageSmallOutlineTexture = contentsManager.GetTexture(@"reminder\reminderImage_smallOutline.png");
            ReminderImageBigOutlineTexture   = contentsManager.GetTexture(@"reminder\reminderImage_bigOutline.png");
            WindowBackgroundTexture          = contentsManager.GetTexture(@"toolSearch\textures\windowsBackground_155985.png");
            ToolSearchWindowEmblem           = contentsManager.GetTexture(@"toolSearch\textures\toolSearchWindowEmblem.png");
            CornerIconTexture                = contentsManager.GetTexture(@"toolSearch\textures\cornerIcon.png");
            HoverCornerIconTexture           = contentsManager.GetTexture(@"toolSearch\textures\cornerIcon_hover.png");
            BankTexture                      = contentsManager.GetTexture(@"toolSearch\textures\bank_156670.png");
            CharacterInventoryTexture        = contentsManager.GetTexture(@"toolSearch\textures\inventory_157098.png");
            SharedInventoryTexture           = contentsManager.GetTexture(@"toolSearch\textures\sharedInventory.png");
            EquipmentTexture                 = contentsManager.GetTexture(@"toolSearch\textures\equipment_156714.png");
            UnknownToolTexture               = contentsManager.GetTexture(@"toolSearch\textures\unknownTool_66591.png");
        }

        public void Dispose()
        {
            LogoutButtonTexture?.Dispose();
            LogoutButtonActiveTexture?.Dispose();
            ReminderBackgroundTexture?.Dispose();
            ReminderImageNoOutlineTexture?.Dispose();
            ReminderImageSmallOutlineTexture?.Dispose();
            ReminderImageBigOutlineTexture?.Dispose();
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

        public Texture2D LogoutButtonTexture { get; }
        public Texture2D LogoutButtonActiveTexture { get; }
        public Texture2D ReminderBackgroundTexture { get; }
        public Texture2D ReminderImageNoOutlineTexture { get; }
        public Texture2D ReminderImageSmallOutlineTexture { get; }
        public Texture2D ReminderImageBigOutlineTexture { get; }
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
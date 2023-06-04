using System;
using Blish_HUD.Modules.Managers;
using Microsoft.Xna.Framework.Graphics;

namespace GatheringTools.ToolSearch.Services
{
    public class TextureService : IDisposable
    {
        public TextureService(ContentsManager contentsManager)
        {
            LogoutButtonTexture              = contentsManager.GetTexture(@"textures\reminder\logoutButton.png");
            LogoutButtonActiveTexture        = contentsManager.GetTexture(@"textures\reminder\logoutButtonActive.png");
            ReminderBackgroundTexture        = contentsManager.GetTexture(@"textures\reminder\logoutDialogTextArea.png");
            ReminderImageNoOutlineTexture    = contentsManager.GetTexture(@"textures\reminder\reminderImage_noOutline.png");
            ReminderImageSmallOutlineTexture = contentsManager.GetTexture(@"textures\reminder\reminderImage_smallOutline.png");
            ReminderImageBigOutlineTexture   = contentsManager.GetTexture(@"textures\reminder\reminderImage_bigOutline.png");
            WindowBackgroundTexture          = contentsManager.GetTexture(@"textures\toolSearch\windowsBackground_155985.png");
            ToolSearchWindowEmblem           = contentsManager.GetTexture(@"textures\toolSearch\toolSearchWindowEmblem.png");
            CornerIconTexture                = contentsManager.GetTexture(@"textures\toolSearch\cornerIcon.png");
            CornerIconHoverTexture           = contentsManager.GetTexture(@"textures\toolSearch\cornerIcon_hover.png");
            BankTexture                      = contentsManager.GetTexture(@"textures\toolSearch\bank_156670.png");
            CharacterInventoryTexture        = contentsManager.GetTexture(@"textures\toolSearch\inventory_157098.png");
            SharedInventoryTexture           = contentsManager.GetTexture(@"textures\toolSearch\sharedInventory.png");
            EquipmentTexture                 = contentsManager.GetTexture(@"textures\toolSearch\equipment_156714.png");
            UnknownToolTexture               = contentsManager.GetTexture(@"textures\toolSearch\unknownTool_66591.png");
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
            CornerIconHoverTexture?.Dispose();
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
        public Texture2D CornerIconHoverTexture { get; }
        public Texture2D BankTexture { get; }
        public Texture2D CharacterInventoryTexture { get; }
        public Texture2D SharedInventoryTexture { get; }
        public Texture2D EquipmentTexture { get; }
        public Texture2D UnknownToolTexture { get; }
    }
}
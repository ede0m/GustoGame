﻿using Comora;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gusto.Models.Interfaces
{
    public interface ICraftingObject
    {
        void DrawCanCraft(SpriteBatch sb, Camera camera);
        string GetCraftSet();
        bool GetShowMenu();
        Queue<InventoryItem> GetCraftingQueue();
    }
}

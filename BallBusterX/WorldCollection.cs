using AgateLib;
using AgateLib.Display;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace BallBusterX
{
    public class WorldCollection
    {
        List<CWorld> worlds = new List<CWorld>();

        public void LoadWorlds(IContentProvider content)
        {
            CSettingsFile file = new CSettingsFile(content.ReadAllText("lvls/worlds.cfg"));

            for (int i = 0; i < file.SectionCount; i++)
            {
                string w = file.getSection(i);
                CWorld world;

                world = new CWorld();

                world.name = file.ReadString(w + ".name", w);
                world.background = file.ReadString(w + ".background", "bg1.png");

                string light = file.ReadString(w + ".ambient", "ffffff");
                world.light = ColorX.FromArgb(light);

                string[] values = file.ReadStringArray(w + ".levels");

                world.lvls.AddRange(values);

                if (world.lvls.Count > 0)
                {
                    worlds.Add(world);
                }
            }
        }

        public int Count => worlds.Count;

        public CWorld this[int index] => worlds[index];
    }
}

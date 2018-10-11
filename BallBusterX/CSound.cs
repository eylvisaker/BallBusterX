/*****************************************************************************
	Ball: Buster
	Copyright (C) 2004-9 Patrick Avella, Erik Ylvisaker

    This file is part of Ball: Buster.

    Ball: Buster is free software; you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation; either version 2 of the License, or
    (at your option) any later version.

    Ball: Buster is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with Ball: Buster; if not, write to the Free Software
    Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
*/

using AgateLib;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using System.Collections.Generic;


namespace BallBusterX
{
    public class CSound
    {
        public SoundEffect bounce, shatter, powerup, ballfall, speedup, ching, die;
        public List<Song> music = new List<Song>();

        public void Load(IContentProvider content)
        {
            bounce = content.Load<SoundEffect>("snd/bounce");
            shatter = content.Load<SoundEffect>("snd/break");
            powerup = content.Load<SoundEffect>("snd/powerup");
            ballfall = content.Load<SoundEffect>("snd/zoom");
            speedup = content.Load<SoundEffect>("snd/speedup");
            ching = content.Load<SoundEffect>("snd/ching");
            die = content.Load<SoundEffect>("snd/die");

            try
            {
                music.Add(content.Load<Song>("snd/music/Rockin1"));
                music.Add(content.Load<Song>("snd/music/Rockin2"));
                music.Add(content.Load<Song>("snd/music/Grunge"));
                music.Add(content.Load<Song>("snd/music/Rave"));
                music.Add(content.Load<Song>("snd/music/FastDance"));
                music.Add(content.Load<Song>("snd/music/SweetDreams"));
            }
            catch
            {
                // can't read ogg files.
            }
        }
    }
}
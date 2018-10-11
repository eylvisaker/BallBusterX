using System;

namespace BallBusterX
{
    public class BBXConfig
    {
        public BBXConfig()
        {

        }

        public bool PlayMusic { get; set; } = true;
        public bool BackgroundScroll { get; set; } = true;

        public void ParseArgs(string[] args)
        {
            foreach(var arg in args)
            {
                switch(arg)
                {
                    case "-nomusic":
                        PlayMusic = false;
                        break;
                }
            }
        }
    }
}
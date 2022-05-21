using System;
using System.Collections.Generic;
using System.IO;
using System.Media;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ScreenActivator.Buisness
{
    public class Sound
    {
        private string path = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
        private SoundPlayer player;

        public Sound()
        {
            player = new SoundPlayer(path + @"\Sound\ClickSound.wav");
        }
        public void BeepSound()
        {
            SystemSounds.Beep.Play();
        }

        public void HandSound()
        {
            SystemSounds.Hand.Play();
        }

        public void AsteriskSound()
        {
            SystemSounds.Asterisk.Play();
        }

        public void ExclamationSound()
        {
            SystemSounds.Exclamation.Play();
        }

        public void QuestionSound()
        {
            SystemSounds.Question.Play();
        }

        public void ClickSound()
        {
            player.Play();
        }
    }
}

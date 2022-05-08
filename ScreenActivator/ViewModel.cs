using Prism.Commands;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Threading;

namespace ScreenActivator
{
    public class ActionCommand : ICommand
    {
        private readonly Action _action;

        public ActionCommand(Action action)
        {
            _action = action;
        }

        public void Execute(object parameter)
        {
            _action();
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;
    }

    public class ViewModel
    {
        DispatcherTimer ProcessHandle = new DispatcherTimer();
        private bool Started = true;
        private MainWindow win;
        public ViewModel(MainWindow wins)
        {
            win = wins;
            SpecialKeyTyped = new DelegateCommand(SpecialFunction);
            ProcessHandle.Interval = new TimeSpan(0, 0, 15);
            ProcessHandle.Tick += KeyPressTimer_Tick;
        }
        public ICommand SpecialKeyTyped
        {
            get; set;
        }

        private async void SpecialFunction()
        {
            if (Started)
            { ProcessHandle.Start(); Started = false; win.ChangeBackgroundOfSpecialCase(true); await win.ChangetheSizeofUIandVisibility(true); }
            else
            { ProcessHandle.Stop(); Started = true; win.ChangeBackgroundOfSpecialCase(false); await win.ChangetheSizeofUIandVisibility(false); }
        }

        private void KeyPressTimer_Tick(object sender, EventArgs e)
        {
            Process pro = new Process();
            pro.StartInfo.FileName = @"C:\Program Files\Internet Explorer\iexplore.exe";
            pro.Start();
            new Program().Main();
            Thread.Sleep(3000);
            pro.Kill();
        }

        public void ProcessHandleStop(bool start)
        {
            if (start)
                ProcessHandle.Start();
            else
            { ProcessHandle.Stop(); Started = true; }
        }

        public class Program

        {

            [DllImport("USER32.DLL")]

            public static extern bool SetForegroundWindow(IntPtr hWnd);

            static IntPtr notepadHandle;

            public void Main()

            {
                //PREPARING...
                Process notepad = new Process();
                notepad.StartInfo.FileName = "notepad.exe";
                notepad.Start();
                notepad.WaitForInputIdle();
                notepadHandle = notepad.MainWindowHandle;
                //YOUR CODE GOES HERE
                //Example:
                WriteLineToNotePad("Hello! I'm trapped in your computer!!!!");
                SendKeyStroke(Environment.NewLine);
                WriteLineToNotePad("Scaaaary isn't it?");
                SendKeyStroke(Environment.NewLine);
                WriteLineToNotePad("No Worries");
                SendKeyStroke(Environment.NewLine);
                WriteLineToNotePad("Just a Part of Automation.");
                SendKeyStroke(Environment.NewLine);
                WriteLineToNotePad("Internet Explorer is Also Opened as Part of it.");
                SendKeyStroke(Environment.NewLine);
                WriteLineToNotePad("----- THank You -----");
                //AND THEN...
                waitABitLonger();
                notepad.Kill();
            }



            void waitABit()

            {
                Random r = new Random();
                Thread.Sleep(r.Next(50, 200)); //Lower numbers is faster typing
            }

            void waitABitLonger()

            {
                Random r = new Random();

                Thread.Sleep(r.Next(2000, 5000)); //Higher numbers is longer waiting

            }

            void SendKeyStroke(string KeyStroke)

            {

                SetForegroundWindow(notepadHandle); //Make sure Notepad is the top window

                SendKeys.SendWait(KeyStroke); //And send a keystroke

            }

            void WriteLineToNotePad(string line)

            {

                for (int i = 0; i < line.Length; i++)

                {//for every letter

                    waitABit(); //wait a bit

                    SendKeyStroke(line[i].ToString()); //then send the keystroke

                }

            }

        }
    }
}

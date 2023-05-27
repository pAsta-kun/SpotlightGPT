using System;
using System.Diagnostics;
using System.Media;
using AudioSwitcher.AudioApi.CoreAudio;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net.Http;

namespace SpotlightGPT
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public class AIResponse
    {
        public string bot { get; set; }
    }

    public partial class MainWindow : Window
    {
        private readonly HttpClient client;
        private MediaPlayer mediaPlayer;
        private bool isDragging;
        private Point startPoint;
        Process process = new Process();
        public MainWindow()
        {
            InitializeComponent();
            client = new HttpClient();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            InitializeComponent();
            //InitializeMediaPlayer();
        }

        private void AIFunctions(String toBeDone)
        {
            //MessageBox.Show(toBeDone.Substring(5));
            //Gets the id from AI response
            int index = toBeDone.IndexOf("id:");
            if (index >= 0)
            {
                toBeDone = toBeDone.Substring(index);
            }

            int id = int.Parse(toBeDone.Substring(3, 1));

            //If ID == 0 then do stuff related to volume
            if(id == 0)
            {

                int newVolume = int.Parse(toBeDone.Substring(5)); // Volume changes
                CoreAudioDevice defaultPlaybackDevice = new CoreAudioController().DefaultPlaybackDevice;

                //Checks to see if the user wants to add to current vol or not
                if (toBeDone.Substring(5, 1) == "+" || toBeDone.Substring(5, 1) == "-")
                {
                    defaultPlaybackDevice.Volume = newVolume + defaultPlaybackDevice.Volume;


                }
                else 
                    defaultPlaybackDevice.Volume = newVolume;
                
            }
            //If Id == 1 then user wants to play/pause
            if(id == 1) 
                PlayPause();

            if(id == 2)
            {
                string command = toBeDone.Substring(5);
                //Sets up command line
                cmdLineSetUp();
                // Send the command to the command line
                process.StandardInput.WriteLine(command);

                // Close the command line
                process.StandardInput.WriteLine("exit");
                process.WaitForExit();
            }

            //Sets up command line
            cmdLineSetUp();
            // Send the command to the command line
            process.StandardInput.WriteLine("mkdir test");

            // Close the command line
            process.StandardInput.WriteLine("exit");
            process.WaitForExit();

        }

        private void cmdLineSetUp()
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = "cmd.exe";
            startInfo.RedirectStandardInput = true;
            startInfo.UseShellExecute = false;
            startInfo.WorkingDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            // Start the process
            process.StartInfo = startInfo;
            process.Start();
        }

        // Virtual key codes for media control
        private const int VK_MEDIA_PLAY_PAUSE = 0xB3;

        // Key event constants
        private const int KEYEVENTF_EXTENDEDKEY = 0x0001;
        private const int KEYEVENTF_KEYUP = 0x0002;

        // Import the user32.dll library
        [DllImport("user32.dll")]
        private static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);

        public static void PlayPause()
        {
            keybd_event(VK_MEDIA_PLAY_PAUSE, 0, KEYEVENTF_EXTENDEDKEY, 0);
            keybd_event(VK_MEDIA_PLAY_PAUSE, 0, KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP, 0);
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (e.Key == Key.Enter && !string.IsNullOrWhiteSpace(textBox.Text))
            {
                //MessageBox.Show(textBox.Text);
                GetAIResponse(textBox.Text);
                textBox.Text = string.Empty;
            }
        }

        //Mouse click on window methods
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            isDragging = true;
            startPoint = e.GetPosition(this);
            this.CaptureMouse();
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                Point currentPosition = e.GetPosition(this);
                double deltaX = currentPosition.X - startPoint.X;
                double deltaY = currentPosition.Y - startPoint.Y;
                this.Left += deltaX;
                this.Top += deltaY;
                startPoint = currentPosition;
            }
        }

        private void Window_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            isDragging = false;
            this.ReleaseMouseCapture();
        }



        //When button click do stuff
        private void ButtonClick(object sender, RoutedEventArgs e)
        {
            GetAIResponse("hey");
        }


        //Gets stuff from AI Method
        private async Task GetAIResponse(string prompt)
        {
            try
            {
                
                var json = "{\"message\":\"" + prompt + "\"}";
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                //MessageBox.Show("working");

                HttpResponseMessage response = await client.PostAsync("http://localhost:5000/", content);
                response.EnsureSuccessStatusCode();

                //MessageBox.Show("working");

                string responseContent = await response.Content.ReadAsStringAsync();
                AIResponse aiResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<AIResponse>(responseContent);

                string botResponse = aiResponse.bot.Trim();
                MessageBox.Show(botResponse);
                AIFunctions(botResponse);
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message);
                Console.WriteLine(error);
            }
        }


        //Removes the initial text when clicked on
        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (textBox.Text == "What would you like?")
            {
                textBox.Text = string.Empty;
            }
        }

        //Need to fix later
        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (string.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.Text = "What would you like?";
            }
        }

    }
}

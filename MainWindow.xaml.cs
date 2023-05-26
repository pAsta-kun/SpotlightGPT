using System;
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
        private bool isDragging;
        private Point startPoint;
        public MainWindow()
        {
            InitializeComponent();
            client = new HttpClient();

            WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (e.Key == Key.Enter && !string.IsNullOrWhiteSpace(textBox.Text))
            {

                MessageBox.Show(textBox.Text);
                GetAIResponse(textBox.Text);
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

                MessageBox.Show("working");

                HttpResponseMessage response = await client.PostAsync("https://chatap-3obp.onrender.com/", content);
                response.EnsureSuccessStatusCode();

                MessageBox.Show("working");

                string responseContent = await response.Content.ReadAsStringAsync();
                AIResponse aiResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<AIResponse>(responseContent);

                string botResponse = aiResponse.bot.Trim();
                MessageBox.Show(botResponse);
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

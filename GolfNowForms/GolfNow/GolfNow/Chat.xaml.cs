using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using GolfNow;
using Amazon.Lex;
using Amazon.Lex.Model;

using Xamarin.Forms;

namespace GolfNow
{
    public partial class Chat : ContentPage
    {
        public static String accessKey = "ASIAR6K6Z5XVFUUJSEEO";
        public static String accessKeySecret = "PDxskeih//5G4hrIwx4YydHAEfV7/nq/Upb1MHgh";
        public static String accessSessionToken = "FQoGZXIvYXdzECAaDEUyysO4q+4DqhIqkiL4AjSekJ4d5kn+8TuXDNrt9UtixcAUq8kwGW+s8+fXhQ2y2dg1N7DJVfZGbcYeM8BG/EAUF/ANOiJ6i+NQDACelC8BBei6FJtIaRZrmM0284M9+QxQp3ZNY5PlI6Fq36ka7iQ+ApfSdtLSM1bYfFPKLEvX47xOv15oKRyc2tktOgzBZJVz89/DbJCt/jxRC8ej/q5Z0OoVyVRvkVab27kbeHM/ZQdIul4Tgy6Q+MG4355rbtiNM68lSv66lGckilTGu6PO1B75Wh/rlHmUjQ8Rzw68EA01C/d5rGmbNx3anNot3wBTe9OJNb99ob1Ujy3hrDaIlNplsaohRDlYdtpGSF+D6e5ieRmimDzsYeqWXg55v19P47MMIkXC1s9hlxoFN+B4UZPb9ivzqDyPKiJs70TJ8JYFgRJ6YynEMnH3AisFHmpNSGm20IKeaOwJ6g4IR8ZPSFIlZ1cd84MnXXnZTqv2IyHtifmUuF5EZAJ+/E+X8mZ9g6hv36QogfCU5QU=";
        public static AmazonLexClient lexClient = new AmazonLexClient(accessKey, accessKeySecret, accessSessionToken, Amazon.RegionEndpoint.USEast1);
        //public ObservableCollection<Message> Messages { get; set; } = new ObservableCollection<Message>();
        public List<String> Messages { get; set; } = new List<String>();

        public Chat()
        {
            InitializeComponent();

            //Messages.Add(new Message() { Text = "Hi" });
            //Messages.Add(new Message() { Text = "How are you?" });

            //this.BindingContext = new[] { "a", "b", "c" };

            listView.ItemsSource = Messages;

        }

        public void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null) return; // has been set to null, do not 'process' tapped event
            DisplayAlert("Tapped", e.SelectedItem + " row was tapped", "OK");
            ((ListView)sender).SelectedItem = null; // de-select the row
        }

        async void OnCellClicked(object sender, EventArgs e)
        {
            var b = (Xamarin.Forms.Button)sender;
            var t = b.CommandParameter;

            await ((ContentPage)((ListView)((ViewCell)((StackLayout)b.Parent).Parent).Parent).Parent).DisplayAlert("Clicked", t + " button was clicked", "OK");
            //Debug.WriteLine("clicked" + t);
        }

        async void SendMessage(object sender, EventArgs e)
        {
            listView.BeginRefresh();

            PostTextRequest request = new PostTextRequest();
            request.BotName = "GolfNowReservation";
            request.BotAlias = "GolfNowTest";
            request.UserId = "250";
            request.InputText = messageText.Text;

            var task = lexClient.PostTextAsync(request);
            Messages.Add(messageText.Text);
            Messages.Add("Bot " + task.Result.Message);

            listView.ItemsSource = Messages.ToArray();
            listView.EndRefresh();

            messageText.Text = "";
        }
    }
}

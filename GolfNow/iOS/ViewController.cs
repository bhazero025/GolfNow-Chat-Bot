using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Foundation;
using UIKit;
using Amazon.Lex;
using Amazon.Lex.Model;

namespace GolfNow.iOS
{
    public partial class ViewController : UIViewController
    {
        public static List<string> messages = new List<string>();
        public static String accessKey = "ASIAR6K6Z5XVGXQC32IC";
        public static String accessKeySecret = "fTv+9XUx+8lBQkYIZmbN7I8kLXH0kTIL+1k1C1RY";
        public static String accessSessionToken = "FQoGZXIvYXdzELH//////////wEaDER3Jfq3SP+Mm9r5wyL4AqZ8nW57ZUavVM4rmbZxWZTpcbEX3yOq+Z+27NFny7JsE7L7Axfh3C5Bh3Dzp/EDgR3fbEIJChSu/+FYPWxnaly96cIUOgQQM0fyk9QwerPR8fiGPa3tGPBhYjFBcO44RNeDVHAKugR2Z1C+/pELT6530/QXfm7nFZ/J72opfv8UMfnQhOrznbiUTn51BV6yn+nL75cZj0XSUYd75aRLJQ8XSjQnE6FpNf4cluJ2WW9YDy9H2bCaRh960P/1+3f227F2TS0bFm5r9AhPOdzri+HvEtWQn9/vvDDUvB3nYKFGR95+w74B4SeBbVqS4YyuvJ0I/D/D5lxfi23ug2mxeiTDmPH/b9Z7lYVk/rs00AilRy4Hm0ltmJ5mUBU6ZWY7RCIQbZcGs9fkhP9+HVsiMFmMj33SkOsVAge2sNBeWV8R3cQUb1J+4rCiYMGSO9YbHvqPJYJwmHtEGst1ZT7p6OwYZi/eR5HvrfzkbEgqpAybwyA55wSkw48o5KDE5AU=";

        public ViewController(IntPtr handle) : base(handle)
        {

        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            tableView.SeparatorStyle = UITableViewCellSeparatorStyle.None;
            tableView.DataSource = new TableViewData();

            // Perform any additional setup after loading the view, typically from a nib.
            //Button.AccessibilityIdentifier = "myButton";
            //Button.TouchUpInside += delegate
            //{
            //    var title = string.Format("{0} clicks!", count++);
            //    Button.SetTitle(title, UIControlState.Normal);
            //};
        }


        partial void sendMessage(UIButton sender)
        {
            Console.WriteLine("Message is " + messageText);
            sendMessageApi(messageText.Text);
            messages.Add("You: " + messageText.Text);
            tableView.ReloadData();

            AmazonLexClient lexClient = new AmazonLexClient(accessKey, accessKeySecret, accessSessionToken, Amazon.RegionEndpoint.USEast1);

            PostTextRequest request = new PostTextRequest();
            request.BotName = "GolfNowReservation";
            request.BotAlias = "GolfNowTest";
            request.UserId = "250";
            request.InputText = messageText.Text;

            var task = lexClient.PostTextAsync(request);

            messages.Add("Bot " + task.Result.Message);

            messageText.Text = "";
            tableView.ReloadData();
            //throw new NotImplementedException();
        }

        private async void sendMessageApi(String message)
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri("http://webhook.site/");

            string jsonData = "{\"message\" : "+message+"}";

            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync("/92279537-4092-4935-a752-5547965db9a7", content);
            Console.WriteLine("Got here! " + message);

            // this result string should be something like: "{"token":"rgh2ghgdsfds"}"
            var result = await response.Content.ReadAsStringAsync();
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.		
        }
    }

    class TableViewData : UITableViewDataSource
    {
        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            UITableViewCell cell = tableView.DequeueReusableCell("cell");
            if (cell == null)
            {
                cell = new UITableViewCell(UITableViewCellStyle.Default, "cell");
            }
            cell.TextLabel.Text = ViewController.messages[indexPath.Row];
            return cell;
        }

        public override nint RowsInSection(UITableView tableView, nint section)
        {
            return ViewController.messages.Count;
        }
    }

}

using System;
using System.Collections.Generic;
using Foundation;
using UIKit;

namespace GolfNow.iOS
{
    public partial class ViewController : UIViewController
    {
        int count = 1;

        public static List<string> messages = new List<string>();

        public ViewController(IntPtr handle) : base(handle)
        {

        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
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
            //Console.WriteLine("Message is " + messageText);
            messages.Add(messageText.Text);
            messageText.Text = "";
            tableView.ReloadData();
            //throw new NotImplementedException();
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

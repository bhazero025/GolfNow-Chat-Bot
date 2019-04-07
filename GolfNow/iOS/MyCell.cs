using Foundation;
using System;
using UIKit;

namespace GolfNow.iOS
{
    public partial class MyCell : UITableViewCell
    {
        public MyCell (IntPtr handle) : base (handle)
        {
        }

        public void update(String message)
        {
            myMessage.Text = message;
        }
    }
}
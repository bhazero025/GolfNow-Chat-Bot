// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace GolfNow.iOS
{
    [Register ("MyCell")]
    partial class MyCell
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel myMessage { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (myMessage != null) {
                myMessage.Dispose ();
                myMessage = null;
            }
        }
    }
}
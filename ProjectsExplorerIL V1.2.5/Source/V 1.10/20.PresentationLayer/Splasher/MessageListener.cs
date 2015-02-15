using System.Linq;
using System.Xml.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Text;

//Message listener, singleton pattern. Inherit from DependencyObject to implement DataBinding.
//Used for displaying the Splash screen.

namespace ATSUI
{
    public class MessageListener : DependencyObject
    {

        private static MessageListener mInstance;

        private MessageListener()
        {
        }

        public static MessageListener Instance
        {
            get
            {
                if (mInstance == null)
                {
                    mInstance = new MessageListener();
                }
                return mInstance;
            }
        }

        public void ReceiveMessage(string Msg)
        {
            Message = Msg;
            DispatcherHelper.DoEvents();
        }

        public string Message
        {
            get
            {
                return (string)GetValue(MessageProperty);
            }
            set
            {
                SetValue(MessageProperty, value);
            }
        }

        public static readonly DependencyProperty MessageProperty = DependencyProperty.Register("Message", typeof(string), typeof(MessageListener), new UIPropertyMetadata(null));

    }
}

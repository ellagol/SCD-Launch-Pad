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
using System.Security.Permissions;
using System.Windows.Threading;

namespace ATSUI
{
    public sealed class DispatcherHelper
    {

        private DispatcherHelper()
        {
        }

        //Simulate Application.DoEvents function.
        [SecurityPermissionAttribute(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        public static void DoEvents()
        {
            DispatcherFrame Frame = new DispatcherFrame();
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, new DispatcherOperationCallback(ExitFrames), Frame);
            try
            {
                Dispatcher.PushFrame(Frame);
            }
            catch (InvalidOperationException generatedExceptionName)
            {
                System.Diagnostics.Debug.WriteLine(generatedExceptionName.Message);
            }
        }

        private static object ExitFrames(object Frame)
        {
            ((DispatcherFrame)Frame).Continue = false;
            return null;
        }

    }
}

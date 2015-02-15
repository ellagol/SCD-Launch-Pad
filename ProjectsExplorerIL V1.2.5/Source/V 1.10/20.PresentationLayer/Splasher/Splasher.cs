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

namespace ATSUI
{
    public sealed class Splasher
    {

        private Splasher()
        {
        }

        private static SplashWindowView mSplash;

        public static SplashWindowView Splash
        {
            get
            {
                return mSplash;
            }
            set
            {
                mSplash = value;
            }
        }

        public static void ShowSplash()
        {
            if (mSplash != null)
            {
                mSplash.Show();
            }
        }

        public static void CloseSplash()
		{
			if (mSplash != null)
			{
				mSplash.Close();
				if (mSplash is IDisposable)
				{
                    ((IDisposable)mSplash).Dispose();
				}
			}
		}

    }
}

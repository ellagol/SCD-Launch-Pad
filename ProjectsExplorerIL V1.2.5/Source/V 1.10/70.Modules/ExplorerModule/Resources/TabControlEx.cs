//<summary>
//The standard WPF TabControl is quite bad in the fact that it only even contains the current TabItem in the VisualTree, so if you
//have complex views it takes a while to re-create the view each tab selection change.Which makes the standard TabControl very sticky to
//work with. This class along with its associated ControlTemplate allow all TabItems to remain in the VisualTree without it being Sticky.
//It does this by keeping all TabItem content in the VisualTree but hides all inactive TabItem content, and only keeps the active TabItem
//content shown.
//Origin: Sacha Barber's Cinch

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
using System.Collections.Specialized;
using System.Windows.Controls.Primitives;
using Infra.MVVM;

namespace ATSUI.CustomControls
{
    [TemplatePart(Name = "PART_SelectedContentHost", Type = typeof(Panel))]
    public class TabControlEx : TabControl
    {

        #region  Data

        private Panel ItemsHolder = null;
        private WorkspaceViewModelBase LastVM = null;

        #endregion

        #region  Ctor

        public TabControlEx()
            : base()
        {
            // this is necessary so that we get the initial databound selected item
            this.ItemContainerGenerator.StatusChanged += ItemContainerGenerator_StatusChanged;
            this.Loaded += TabControlEx_Loaded;

        }

        #endregion

        #region  Public/Protected Methods

        /// <summary>
        /// get the ItemsHolder and generate any children
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            ItemsHolder = GetTemplateChild("PART_SelectedContentHost") as Panel;
            UpdateSelectedItem();
        }

        /// <summary>
        /// when the items change we remove any generated panel children and add any new ones as necessary
        /// </summary>
        /// <param name="e"></param>
        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);
            if (ItemsHolder == null)
            {
                return;
            }
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Reset:
                    ItemsHolder.Children.Clear();
                    break;
                case NotifyCollectionChangedAction.Add:
                case NotifyCollectionChangedAction.Remove:
                    if (e.OldItems != null)
                    {
                        foreach (object item in e.OldItems)
                        {
                            ContentPresenter cp = FindChildContentPresenter(item);
                            if (cp != null)
                            {
                                ItemsHolder.Children.Remove(cp);
                            }
                        }
                    }

                    // don't do anything with new items because we don't want to create visuals that aren't being shown

                    UpdateSelectedItem();
                    break;

                case NotifyCollectionChangedAction.Replace:
                    throw new NotImplementedException("Replace not implemented yet");
            }
        }

        /// <summary>
        /// update the visible child in the ItemsHolder
        /// </summary>
        /// <param name="e"></param>
        //private bool flg = false;
        //private int count;

        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            base.OnSelectionChanged(e);
            UpdateSelectedItem();
        }

        /// <summary>
        /// copied from TabControl; wish it were protected in that class instead of private
        /// </summary>
        /// <returns></returns>
        protected TabItem GetSelectedTabItem()
        {
            object selectedItem = base.SelectedItem;
            if (selectedItem == null)
            {
                return null;
            }
            TabItem item = selectedItem as TabItem;
            if (item == null)
            {
                item = base.ItemContainerGenerator.ContainerFromIndex(base.SelectedIndex) as TabItem;
            }
            return item;
        }
        #endregion

        #region  Private Methods

        /// <summary>
        /// in some scenarios we need to update when loaded in case the 
        /// ApplyTemplate happens before the databind.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TabControlEx_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateSelectedItem();
        }

        /// <summary>
        /// if containers are done, generate the selected item
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ItemContainerGenerator_StatusChanged(object sender, EventArgs e)
        {
            if (this.ItemContainerGenerator.Status == GeneratorStatus.ContainersGenerated)
            {
                this.ItemContainerGenerator.StatusChanged -= ItemContainerGenerator_StatusChanged;
                UpdateSelectedItem();
            }
        }

        /// <summary>
        /// generate a ContentPresenter for the selected item
        /// </summary>
        private void UpdateSelectedItem()
        {
            if (ItemsHolder == null)
            {
                return;
            }

            // generate a ContentPresenter if necessary
            TabItem item = GetSelectedTabItem();
            if (item != null)
            {
                try
                {
                    if (LastVM != null)
                    {
                        bool Capture = SaveImageOfControl(this, LastVM.WSId.ToString() + ".WSID.png");
                    }
                    LastVM = (WorkspaceViewModelBase)item.Content;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex);
                }
                //
                CreateChildContentPresenter(item);
                item.Focus(); //Eli
            }

            // show the right child
            foreach (ContentPresenter child in ItemsHolder.Children)
            {
                if (((TabItem)child.Tag).IsSelected)
                {
                    child.Visibility = System.Windows.Visibility.Visible;
                }
                else
                {
                    child.Visibility = System.Windows.Visibility.Collapsed;
                }
            }

        }

        /// <summary>
        /// create the child ContentPresenter for the given item (could be data or a TabItem)
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private ContentPresenter CreateChildContentPresenter(object item)
        {
            if (item == null)
            {
                return null;
            }

            ContentPresenter cp = FindChildContentPresenter(item);

            if (cp != null)
            {
                return cp;
            }

            // the actual child to be added.  cp.Tag is a reference to the TabItem
            cp = new ContentPresenter();
            cp.Content = (item is TabItem) ? ((TabItem)item).Content : item;
            cp.ContentTemplate = this.SelectedContentTemplate;
            cp.ContentTemplateSelector = this.SelectedContentTemplateSelector;
            cp.ContentStringFormat = this.SelectedContentStringFormat;
            cp.Visibility = System.Windows.Visibility.Collapsed;
            cp.Tag = (item is TabItem) ? item : (this.ItemContainerGenerator.ContainerFromItem(item));
            ItemsHolder.Children.Add(cp);
            return cp;
        }

        /// <summary>
        /// Find the CP for the given object.  data could be a TabItem or a piece of data
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private ContentPresenter FindChildContentPresenter(object data)
        {
            if (data is TabItem)
            {
                data = ((TabItem)data).Content;
            }

            if (data == null)
            {
                return null;
            }

            if (ItemsHolder == null)
            {
                return null;
            }

            foreach (ContentPresenter cp in ItemsHolder.Children)
            {
                if (cp.Content == data)
                {
                    return cp;
                }
            }

            return null;
        }

        #endregion

        #region  Screenshot

        private static PngBitmapEncoder getImageFromControl(Control controlToConvert)
        {
            // save current canvas transform
            Transform transform = controlToConvert.LayoutTransform;

            // get size of control
            Size sizeOfControl = new Size(controlToConvert.ActualWidth, controlToConvert.ActualHeight);
            // measure and arrange the control
            controlToConvert.Measure(sizeOfControl);
            // arrange the surface
            controlToConvert.Arrange(new Rect(sizeOfControl));

            // craete and render surface and push bitmap to it
            RenderTargetBitmap renderBitmap = new RenderTargetBitmap(Convert.ToInt32(sizeOfControl.Width), Convert.ToInt32(sizeOfControl.Height), 96.0D, 96.0D, PixelFormats.Pbgra32);
            // now render surface to bitmap
            renderBitmap.Render(controlToConvert);

            // encode png data
            PngBitmapEncoder pngEncoder = new PngBitmapEncoder();
            // puch rendered bitmap into it
            pngEncoder.Frames.Add(BitmapFrame.Create(renderBitmap));

            // return encoder
            return pngEncoder;
        }

        /// &lt;summary&gt;
        /// Get an ImageSource of a control
        /// &lt;/summary&gt;
        /// &lt;param name="controlToConvert"&gt;The control to convert to an ImageSource&lt;/param&gt;
        /// &lt;returns&gt;The returned ImageSource of the controlToConvert&lt;/returns&gt;
        public static ImageSource GetImageOfControl(Control controlToConvert)
        {
            // return first frame of image 
            return getImageFromControl(controlToConvert).Frames[0];
        }

        /// &lt;summary&gt;
        /// Save an image of a control
        /// &lt;/summary&gt;
        /// &lt;param name="controlToConvert"&gt;The control to convert to an ImageSource&lt;/param&gt;
        ///  /// &lt;param name="fileName"&gt;The location to save the image to&lt;/param&gt;
        /// &lt;returns&gt;The returned ImageSource of the controlToConvert&lt;/returns&gt;
        public static bool SaveImageOfControl(Control controlToConvert, string fileName)
        {
            try
            {
                // create a file stream for saving image
                using (System.IO.FileStream outStream = new System.IO.FileStream(fileName, System.IO.FileMode.Create))
                {
                    // save encoded data to stream
                    getImageFromControl(controlToConvert).Save(outStream);
                }
            }
            catch (Exception e)
            {
                // display for debugging
                Console.WriteLine("Exception caught saving stream: {0}", e.Message);
                // return fail
                return false;
            }

            // return that passed
            return true;
        }

        #endregion

    }

}

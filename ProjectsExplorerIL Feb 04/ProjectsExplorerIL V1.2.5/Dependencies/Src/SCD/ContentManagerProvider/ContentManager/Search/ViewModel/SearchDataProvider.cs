using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using ContentManager.ContentManagerMainWindow.ViewModel;
using ContentManager.General;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace ContentManager.Search.ViewModel
{
    public class SearchDataProvider : ViewModelBase
    {
        IEnumerator<ItemNode> _matchingItemNodeEnumerator;

        public SearchDataProvider()
        {
            IsErrorVisible = false;
            _matchingItemNodeEnumerator = null;
            FindExecute = new RelayCommand(FindExecuteFn);
        }

        private String _textForSearch;
        public String TextForSearch
        {
            get { return _textForSearch; }
            set
            {
                IsErrorVisible = false;
                _matchingItemNodeEnumerator = null;
                Set(() => TextForSearch, ref _textForSearch, value);
            }
        }

        private bool _isErrorVisible;
        public bool IsErrorVisible
        {
            get { return _isErrorVisible; }
            set
            {
                Set(() => IsErrorVisible, ref _isErrorVisible, value);
            }
        }

        public RelayCommand FindExecute { get; set; }

        private void FindExecuteFn()
        {
            if (_matchingItemNodeEnumerator == null || !_matchingItemNodeEnumerator.MoveNext())
                this.VerifyMatchingItemNodeEnumerator();

            ItemNode item;
            item = _matchingItemNodeEnumerator == null ? null : _matchingItemNodeEnumerator.Current;

            if (item == null)
                return;

            item.IsSelected = true;

            // Ensure that this person is in view.
            while (item.Parent != null)
            {
                item.Parent.IsExpanded = true;
                item = item.Parent;
            }
        }

        void VerifyMatchingItemNodeEnumerator()
        {
            if (String.IsNullOrEmpty(_textForSearch))
                return;

            var matches = this.FindMatches(_textForSearch, Locator.ContentManagerDataProvider.SubItemNode);
            _matchingItemNodeEnumerator = matches.GetEnumerator();

            if (!_matchingItemNodeEnumerator.MoveNext())
                IsErrorVisible = true;
        }

        private IEnumerable<ItemNode> FindMatches(string searchText, IEnumerable<ItemNode> itemNodes)
        {
             foreach (ItemNode child in itemNodes)
                foreach (ItemNode match in this.FindMatches(searchText, child))
                    yield return match;
        }

        IEnumerable<ItemNode> FindMatches(string searchText, ItemNode itemNode)
        {
            if (itemNode.Contains(searchText))
                yield return itemNode;

            foreach (ItemNode child in itemNode.SubItemNode)
                foreach (ItemNode match in this.FindMatches(searchText, child))
                    yield return match;
        }
    }
}



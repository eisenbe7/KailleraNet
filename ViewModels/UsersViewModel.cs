using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows.Data;
using System.Collections.ObjectModel;
using KailleraNET.Windows;
using KailleraNET.Hotkeys;
using System.Windows.Input;

namespace KailleraNET.ViewModels
{
    class UsersViewModel
    {
        ObservableCollection<User> usersList;

        public UsersWindow wind = new UsersWindow();

        public ObservableCollection<User> UsersList
        {
            get
            {
                return usersList;
            }
        }

        public delegate void AddBuddyList(User user);
        public AddBuddyList addBuddyList;

        ICollectionView usersView;

        public UsersViewModel()
        {
            usersList = new ObservableCollection<User>();
            usersView = CollectionViewSource.GetDefaultView(UsersList);
            usersView.GroupDescriptions.Add(new PropertyGroupDescription("Category"));
            usersView.SortDescriptions.Add(new SortDescription("sortOrder",ListSortDirection.Descending));
            usersView.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
            wind.usersList.ItemsSource = UsersList;

            wind.usersList.MouseDoubleClick += wind.sendPMs;

            KailleraTrayManager.Instance.addActiveWindow(wind);

            wind.addBuddyList += addBuddy;
            wind.Show();
        }



        /// <summary>
        /// Updates the observable collection with the user list
        /// </summary>
        /// <param name="gb"></param>
        public void updateUsers(List<User> users)
        {              
            UsersList.Clear();
            foreach (var user in users)
            {
                UsersList.Add(user);
            }
        }

        /// <summary>
        /// Closes the users window
        /// </summary>
        public void close()
        {
            wind.Close();
        }

        /// <summary>
        /// Refreshes the list
        /// </summary>
        public void refreshList()
        {
            var tempList = new List<User>(UsersList);
            updateUsers(tempList);
        }

        public void addBuddy(User user)
        {
            if (addBuddyList != null)
            {
                addBuddyList(user);
            }
        }

    }
}

using DineTab_v1.Models;
using DineTab_v1.Services;
using DineTab_v1.Views.Admin;
using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DineTab_v1.ViewModels.Admin
{
    public class StaffManagementViewModel : BaseViewModel
    {
        private readonly DatabaseService _dbService;

        private ObservableCollection<User> _allStaff = new(); // Keep all staff for filtering
        public ObservableCollection<User> StaffList { get; set; } = new();

        private string _selectedRole = "All Account"; // default filter
        public string SelectedRole
        {
            get => _selectedRole;
            set
            {
                if (_selectedRole == value) return;
                _selectedRole = value;
                OnPropertyChanged();
                ApplyFilters();
            }
        }

        private string _searchText;
        public string SearchText
        {
            get => _searchText;
            set
            {
                if (_searchText == value) return;
                _searchText = value;
                OnPropertyChanged();
                ApplyFilters(); // apply search + role filter
            }
        }

        public ICommand AddAccountCommand { get; }
        public ICommand GoToRemoveStaffCommand { get; }
        public ICommand GoToModifyStaffCommand { get; }
        public ICommand SelectRoleCommand { get; }

        public StaffManagementViewModel()
        {
            _dbService = new DatabaseService();

            // Subscribe to staff updates
            MessagingCenter.Subscribe<AddStaffViewModel>(this, "StaffUpdated", (sender) => LoadUser());
            MessagingCenter.Subscribe<ModifyStaffViewModel>(this, "StaffUpdated", (sender) => LoadUser());
            MessagingCenter.Subscribe<RemoveStaffViewModel>(this, "StaffUpdated", (sender) => LoadUser());

            // Commands
            AddAccountCommand = new Command(OnAddAccount);
            GoToRemoveStaffCommand = new Command<User>(async (user) => await GoToRemoveStaff(user));
            GoToModifyStaffCommand = new Command<User>(async (user) => await GoToModifyStaff(user));
            SelectRoleCommand = new Command<string>((role) => SelectedRole = role);

            // Load staff from DB
            LoadUser();
        }

        private async void OnAddAccount()
        {
            await Application.Current.MainPage.Navigation.PushModalAsync(new AddStaffPage());
        }

        private async Task GoToRemoveStaff(User user)
        {
            await Application.Current.MainPage.Navigation.PushModalAsync(new RemoveStaffPage(user));
        }

        private async Task GoToModifyStaff(User user)
        {
            await Application.Current.MainPage.Navigation.PushModalAsync(new ModifyStaffPage(user));
        }

        private async void LoadUser()
        {
            try
            {
                var staffFromDb = await _dbService.GetAllStaffAsync();
                _allStaff.Clear();
                StaffList.Clear();

                foreach (var u in staffFromDb)
                {
                    _allStaff.Add(u);
                    StaffList.Add(u);
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"Failed to load staff: {ex.Message}", "OK");
            }
        }

        /// <summary>
        /// Applies both role filter and search filter
        /// </summary>
        private void ApplyFilters()
        {
            IEnumerable<User> filtered = _allStaff;

            // Role filter
            if (!string.IsNullOrEmpty(SelectedRole) && SelectedRole != "All Account")
            {
                filtered = filtered.Where(s => s.Role.Equals(SelectedRole, StringComparison.OrdinalIgnoreCase));
            }

            // Search filter
            if (!string.IsNullOrEmpty(SearchText))
            {
                string search = SearchText.ToLower();
                filtered = filtered.Where(s =>
                    (!string.IsNullOrEmpty(s.FirstName) && s.FirstName.ToLower().Contains(search)) ||
                    (!string.IsNullOrEmpty(s.LastName) && s.LastName.ToLower().Contains(search)) ||
                    (!string.IsNullOrEmpty(s.Email) && s.Email.ToLower().Contains(search)) ||
                    (!string.IsNullOrEmpty(s.Role) && s.Role.ToLower().Contains(search)));
            }

            // Update collection
            StaffList.Clear();
            foreach (var staff in filtered)
            {
                StaffList.Add(staff);
            }
        }
    }

    // ✅ Converter to highlight only the selected role
    public class RoleToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string selectedRole = value as string;
            string thisRole = parameter as string;

            if (selectedRole == thisRole)
                return Colors.Orange; // Highlight selected

            return Colors.Transparent; // Default background
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

using Frosty.Controls;
using System.Windows.Media.Imaging;
using System.IO;
using Frosty.Core;
using Frosty.Core.Mod;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System;
using System.Linq;
using System.Runtime.CompilerServices;

namespace FrostyEditor.Windows
{
    /// <summary>
    /// Interaction logic for ModSettingsWindow.xaml
    /// </summary>
    public partial class ModSettingsWindow : FrostyDockableWindow, INotifyPropertyChanged, INotifyDataErrorInfo
    {
        private static readonly string[] ApprovedDomains = { "nexusmods.com", "moddb.com" };
        private static readonly string[] FieldOrder =
        {
            nameof(ModTitle),
            nameof(ModAuthor),
            nameof(ModVersion),
            nameof(ModCategory),
            nameof(ModLink),
            nameof(ModDescription)
        };

        private ModSettings ModSettings => project.GetModSettings();
        private FrostyProject project;
        private bool hasValidated;
        private readonly Dictionary<string, string> errors = new Dictionary<string, string>();

        private readonly List<string> categories = new List<string>()
        {
            "Custom",
            "Audio",
            "Cosmetic",
            "Gameplay",
            "Graphic",
            "Map",
            "User Interface"
        };

        private string modTitle = string.Empty;
        private string modAuthor = string.Empty;
        private string modVersion = string.Empty;
        private string modCategory = string.Empty;
        private int selectedCategoryIndex;
        private string modLink = string.Empty;
        private string modDescription = string.Empty;

        public IReadOnlyList<string> Categories => categories;

        public string ModTitle
        {
            get => modTitle;
            set
            {
                if (SetField(ref modTitle, value ?? string.Empty))
                    ValidateTextField(nameof(ModTitle), modTitle, "Title", isMandatory: true);
            }
        }

        public string ModAuthor
        {
            get => modAuthor;
            set
            {
                if (SetField(ref modAuthor, value ?? string.Empty))
                    ValidateTextField(nameof(ModAuthor), modAuthor, "Author", isMandatory: true);
            }
        }

        public string ModVersion
        {
            get => modVersion;
            set
            {
                if (SetField(ref modVersion, value ?? string.Empty))
                    ValidateTextField(nameof(ModVersion), modVersion, "Version", isMandatory: true);
            }
        }

        public string ModCategory
        {
            get => modCategory;
            set
            {
                if (SetField(ref modCategory, value ?? string.Empty))
                    ValidateTextField(nameof(ModCategory), modCategory, "Category", isMandatory: true);
            }
        }

        public int SelectedCategoryIndex
        {
            get => selectedCategoryIndex;
            set
            {
                if (value < 0 || value >= categories.Count)
                    return;

                if (SetField(ref selectedCategoryIndex, value))
                {
                    OnPropertyChanged(nameof(IsCustomCategory));
                    ModCategory = IsCustomCategory ? string.Empty : categories[selectedCategoryIndex];
                }
            }
        }

        public bool IsCustomCategory => selectedCategoryIndex == 0;

        public string ModLink
        {
            get => modLink;
            set
            {
                if (SetField(ref modLink, value ?? string.Empty))
                    ValidateLinkField(force: false);
            }
        }

        public string ModDescription
        {
            get => modDescription;
            set
            {
                if (SetField(ref modDescription, value ?? string.Empty))
                    ValidateTextField(nameof(ModDescription), modDescription, "Description", isMandatory: false);
            }
        }

        public bool HasErrors => errors.Count > 0;

        public string PrimaryError
        {
            get
            {
                List<string> orderedErrors = GetOrderedErrors();
                if (orderedErrors.Count == 0)
                    return string.Empty;

                return orderedErrors.Count == 1
                    ? orderedErrors[0]
                    : $"{orderedErrors[0]} (+{orderedErrors.Count - 1} more)";
            }
        }

        public string ErrorSummary => string.Join(Environment.NewLine, GetOrderedErrors());

        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public ModSettingsWindow(FrostyProject inProject = null)
        {
            DataContext = this;
            InitializeComponent();

            project = inProject;
            Loaded += ModSettingsWindow_Loaded;
        }

        public IEnumerable GetErrors(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
                return GetOrderedErrors();

            return errors.TryGetValue(propertyName, out string error)
                ? new[] { error }
                : null;
        }

        private void ModSettingsWindow_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            modTitle = ModSettings.Title ?? string.Empty;
            modAuthor = ModSettings.Author ?? string.Empty;
            selectedCategoryIndex =
                ModSettings.SelectedCategory >= 0
                && ModSettings.SelectedCategory < categories.Count
                ? ModSettings.SelectedCategory
                : 0;
            modCategory = IsCustomCategory ? (ModSettings.Category ?? string.Empty) : categories[selectedCategoryIndex];
            modVersion = ModSettings.Version ?? string.Empty;
            modDescription = ModSettings.Description ?? string.Empty;
            modLink = ModSettings.Link ?? string.Empty;

            OnPropertyChanged(string.Empty);

            iconImageButton.SetImage(ModSettings.Icon);
            ssImageButton1.SetImage(ModSettings.GetScreenshot(0));
            ssImageButton2.SetImage(ModSettings.GetScreenshot(1));
            ssImageButton3.SetImage(ModSettings.GetScreenshot(2));
            ssImageButton4.SetImage(ModSettings.GetScreenshot(3));
        }

        private void cancelButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void saveButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            hasValidated = true;
            ValidateAll();

            if (HasErrors)
                return;

            ValidateLink(modLink, out string normalizedLink);
            ModLink = normalizedLink;

            ModSettings.Title = ModTitle;
            ModSettings.Author = ModAuthor;
            ModSettings.Category = ModCategory;
            ModSettings.SelectedCategory = SelectedCategoryIndex;
            ModSettings.Version = ModVersion;
            ModSettings.Description = ModDescription;
            ModSettings.Link = ModLink;
            ModSettings.Icon = iconImageButton.GetImage();
            ModSettings.SetScreenshot(0, ssImageButton1.GetImage());
            ModSettings.SetScreenshot(1, ssImageButton2.GetImage());
            ModSettings.SetScreenshot(2, ssImageButton3.GetImage());
            ModSettings.SetScreenshot(3, ssImageButton4.GetImage());

            DialogResult = true;
            Close();
        }

        private bool FrostyImageButton_OnValidate(object sender, FileInfo fi, BitmapImage bimage)
        {
            FrostyImageButton btn = sender as FrostyImageButton;
            if (btn == iconImageButton)
            {
                if (bimage.PixelWidth > 128 || bimage.PixelHeight > 128)
                {
                    FrostyMessageBox.Show("Icon cannot be larger than 128x128");
                    return false;
                }
            }
            else
            {
                if (fi.Length > (5 * 1024 * 1024))
                {
                    FrostyMessageBox.Show("Screenshots cannot be larger than 5mb each");
                    return false;
                }
            }

            return true;
        }

        private void ValidateAll()
        {
            ValidateTextField(nameof(ModTitle), modTitle, "Title", isMandatory: true);
            ValidateTextField(nameof(ModAuthor), modAuthor, "Author", isMandatory: true);
            ValidateTextField(nameof(ModVersion), modVersion, "Version", isMandatory: true);
            ValidateTextField(nameof(ModCategory), modCategory, "Category", isMandatory: true);
            ValidateLinkField(force: true);
            ValidateTextField(nameof(ModDescription), modDescription, "Description", isMandatory: false);
        }

        private void ValidateTextField(string propertyName, string value, string displayName, bool isMandatory)
        {
            if (isMandatory && hasValidated && string.IsNullOrWhiteSpace(value))
                SetError(propertyName, $"{displayName} is a mandatory field");
            else if (ContainsInvalidAscii(value))
                SetError(propertyName, $"{displayName} can only contain ASCII characters");
            else
                SetError(propertyName, null);
        }

        private void ValidateLinkField(bool force)
        {
            if (!force && !hasValidated && !errors.ContainsKey(nameof(ModLink)))
                return;

            string error = ValidateLink(modLink, out _);
            SetError(nameof(ModLink), error);
        }

        private static string ValidateLink(string link, out string normalizedLink)
        {
            normalizedLink = string.Empty;

            if (string.IsNullOrWhiteSpace(link))
                return null;

            string trimmed = link.Trim();
            if (!Uri.TryCreate(trimmed, UriKind.Absolute, out Uri uriResult) ||
                (uriResult.Scheme != Uri.UriSchemeHttp && uriResult.Scheme != Uri.UriSchemeHttps))
            {
                return "Link needs to be a valid HTTP/HTTPS URL";
            }

            bool isApprovedDomain = ApprovedDomains.Any(domain =>
                uriResult.Host.Equals(domain, StringComparison.OrdinalIgnoreCase) ||
                uriResult.Host.EndsWith("." + domain, StringComparison.OrdinalIgnoreCase));

            if (!isApprovedDomain)
                return "Link needs to be nexusmods.com or moddb.com";

            if (ContainsInvalidAscii(uriResult.AbsoluteUri))
                return "Link can only contain ASCII characters";

            normalizedLink = uriResult.AbsoluteUri;
            return null;
        }

        private static bool ContainsInvalidAscii(string value)
        {
            return !string.IsNullOrEmpty(value) && value.Any(c => c == '\0' || c > 0x7F);
        }

        private List<string> GetOrderedErrors()
        {
            return FieldOrder.Where(errors.ContainsKey).Select(key => errors[key]).ToList();
        }

        private void SetError(string propertyName, string error)
        {
            bool changed;
            if (string.IsNullOrEmpty(error))
            {
                changed = errors.Remove(propertyName);
            }
            else if (errors.TryGetValue(propertyName, out string existing) && existing == error)
            {
                changed = false;
            }
            else
            {
                errors[propertyName] = error;
                changed = true;
            }

            if (changed)
            {
                ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
                OnPropertyChanged(nameof(HasErrors));
                OnPropertyChanged(nameof(PrimaryError));
                OnPropertyChanged(nameof(ErrorSummary));
            }
        }

        private bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
                return false;

            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

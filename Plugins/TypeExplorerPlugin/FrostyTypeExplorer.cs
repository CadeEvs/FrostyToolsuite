using FrostySdk.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using Frosty.Core.Controls;
using System.Windows.Media;
using FrostySdk;
using FrostySdk.Attributes;

namespace TypeExplorerPlugin
{
    [TemplatePart(Name = PART_TypesListBox, Type = typeof(ListBox))]
    [TemplatePart(Name = PART_TypeFilter, Type = typeof(TextBox))]
    [TemplatePart(Name = PART_TypeFieldsTextBox, Type = typeof(RichTextBox))]
    [TemplatePart(Name = PART_HideEmptyCheckBox, Type = typeof(CheckBox))]
    public class FrostyTypeExplorer : FrostyBaseEditor
    {
        public override ImageSource Icon => TypeExplorerMenuExtension.imageSource;

        public List<TypeItem> TypeItems { get; private set; }

        private const string PART_TypesListBox = "PART_TypesListBox";
        private const string PART_TypeFilter = "PART_TypeFilter";
        private const string PART_TypeFieldsTextBox = "PART_TypeFieldsTextBox";
        private const string PART_HideEmptyCheckBox = "PART_HideEmptyCheckBox";

        private ListBox typesListBox;
        private TextBox typeFilterTextBox;
        private RichTextBox typeFieldsTextBox;
        private CheckBox hideEmptyCheckBox;

        private static readonly SolidColorBrush TextColor = new SolidColorBrush(Color.FromRgb(0xDC, 0xDC, 0xDC));
        private static readonly SolidColorBrush EnumColor = new SolidColorBrush(Color.FromRgb(0xB8, 0xD7, 0xA3));
        private static readonly SolidColorBrush ClassColor = new SolidColorBrush(Color.FromRgb(0x4E, 0xC9, 0xB0));
        private static readonly SolidColorBrush StructColor = new SolidColorBrush(Color.FromRgb(0x86, 0xC6, 0x91));
        private static readonly SolidColorBrush LiteralColor = new SolidColorBrush(Color.FromRgb(0xB5, 0xCE, 0xA8));

        private ILogger logger;

        public class TypeItem
        {
            public Type Type { get; }
            public string Name { get; }
            public SolidColorBrush Brush { get; }

            public TypeItem(Type type)
            {
                Type = type;
                Name = type.Name;
                Brush = GetTypeColor(type);
            }
        }

        static FrostyTypeExplorer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FrostyTypeExplorer), new FrameworkPropertyMetadata(typeof(FrostyTypeExplorer)));
        }

        public FrostyTypeExplorer(ILogger inLogger = null)
        {
            logger = inLogger;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            typesListBox = GetTemplateChild(PART_TypesListBox) as ListBox;
            typeFilterTextBox = GetTemplateChild(PART_TypeFilter) as TextBox;
            typeFieldsTextBox = GetTemplateChild(PART_TypeFieldsTextBox) as RichTextBox;
            hideEmptyCheckBox = GetTemplateChild(PART_HideEmptyCheckBox) as CheckBox;

            Loaded += FrostyTypeExplorer_Loaded;
            typesListBox.SelectionChanged += TypesListBox_SelectionChanged;
            typeFilterTextBox.LostFocus += TypeFilterTextBox_LostFocus;
            typeFilterTextBox.KeyUp += TypeFilterTextBox_KeyUp;
            hideEmptyCheckBox.Checked += HideEmptyCheckBox_Checked;
            hideEmptyCheckBox.Unchecked += HideEmptyCheckBox_Unchecked;
        }

        private void TypeFilterTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                TypeFilterTextBox_LostFocus(this, new RoutedEventArgs());
        }

        private void TypeFilterTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(typeFilterTextBox.Text))
            {
                if (hideEmptyCheckBox.IsChecked == true)
                    typesListBox.Items.Filter = a => FilterItem(((TypeItem)a).Type);
                else
                    typesListBox.Items.Filter = a => ((TypeItem)a).Type.Name.ToLower().Contains(typeFilterTextBox.Text.ToLower()) || TypeLibrary.IsSubClassOf(((TypeItem)a).Type, typeFilterTextBox.Text);
            }
            else if (hideEmptyCheckBox.IsChecked == true)
                typesListBox.Items.Filter = a => GetPropertyCount(((TypeItem)a).Type) > 0;
            else
                typesListBox.Items.Filter = null;
        }

        private void TypesListBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            if (typesListBox.SelectedItem is TypeItem typeItem)
            {
                Type type = typeItem.Type;

                FlowDocument document = new FlowDocument();
                Paragraph paragraph = new Paragraph();

                paragraph.Inlines.Add(new Run(type.Name) { Foreground = GetTypeColor(type) });

                // enums
                if (type.IsEnum)
                {
                    paragraph.Inlines.Add(new Run("\n{\n") { Foreground = TextColor });

                    var underlyingType = Enum.GetUnderlyingType(type);
                    var values = type.GetEnumValues();

                    // enum members
                    for (int i = 0; i < values.Length; i++)
                    {
                        paragraph.Inlines.Add(new Run($"    {values.GetValue(i)} = ") { Foreground = TextColor });
                        paragraph.Inlines.Add(new Run(Convert.ChangeType(values.GetValue(i), underlyingType).ToString()) { Foreground = LiteralColor });
                        paragraph.Inlines.Add(new Run(i != values.Length - 1 ? ",\n" : "\n") { Foreground = TextColor });
                    }

                    paragraph.Inlines.Add(new Run("}") { Foreground = TextColor });
                    document.Blocks.Add(paragraph);

                    typeFieldsTextBox.Document = document;
                    return;
                }

                // base types
                if (type.BaseType != null && type.BaseType.Name != "Object")
                {
                    paragraph.Inlines.Add(new Run(" : ") { Foreground = TextColor });
                    paragraph.Inlines.Add(new Run(type.BaseType.Name) { Foreground = ClassColor });
                }

                paragraph.Inlines.Add(new Run("\n{\n") { Foreground = TextColor });

                var props = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);

                // class properties
                foreach (var pi in type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly))
                {
                    if (pi.Name.StartsWith("_"))
                        continue;
                    
                    string propName = pi.PropertyType.Name;
                    paragraph.Inlines.Add(new Run("    "));

                    // use ebx field type names
                    if (propName == "Single")
                        propName = "Float32";
                    else if (propName == "Double")
                        propName = "Float64";

                    // get the generic/ref type
                    if (propName == "List`1" || propName == "PointerRef")
                    {
                        paragraph.Inlines.Add(new Run(propName == "List`1" ? "List" : "PointerRef") { Foreground = ClassColor });
                        paragraph.Inlines.Add(new Run("<") { Foreground = TextColor });

                        if (propName == "List`1")
                        {
                            Type listType = pi.PropertyType.GetGenericArguments()[0];

                            // list of pointerrefs
                            if (listType.Name == "PointerRef")
                            {
                                paragraph.Inlines.Add(new Run("PointerRef") { Foreground = ClassColor });
                                paragraph.Inlines.Add(new Run("<") { Foreground = TextColor });
                                paragraph.Inlines.Add(new Run(pi.GetCustomAttribute<EbxFieldMetaAttribute>().BaseType?.Name) { Foreground = ClassColor });
                                paragraph.Inlines.Add(new Run(">") { Foreground = TextColor });
                            }
                            else
                                paragraph.Inlines.Add(new Run(listType.Name) {Foreground = GetTypeColor(listType) });
                        }
                        else
                        {
                            paragraph.Inlines.Add(new Run(pi.GetCustomAttribute<EbxFieldMetaAttribute>().BaseType?.Name) {Foreground = ClassColor});
                        }

                        paragraph.Inlines.Add(new Run(">") { Foreground = TextColor });
                    }
                    else
                    {
                        paragraph.Inlines.Add(new Run(propName) { Foreground = GetTypeColor(pi.PropertyType) });
                    }

                    paragraph.Inlines.Add(new Run($" {pi.Name};\n") { Foreground = TextColor });
                }

                paragraph.Inlines.Add(new Run("}") { Foreground = TextColor });
                document.Blocks.Add(paragraph);
                typeFieldsTextBox.Document = document;
            }
            else
            {
                typeFieldsTextBox.Document = new FlowDocument();
            }
        }

        private static SolidColorBrush GetTypeColor(Type type)
        {
            if (type.IsEnum)
                return EnumColor;
            
            if (type.IsValueType)
                return StructColor;

            return ClassColor;
        }

        private void HideEmptyCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            typesListBox.Items.Filter = a => FilterItem(((TypeItem)a).Type);
        }

        private bool FilterItem(Type type)
        {
            if (type.IsEnum && type.Name.ToLower().Contains(typeFilterTextBox.Text.ToLower()))
                return true;

            if (type.Name.ToLower().Contains(typeFilterTextBox.Text.ToLower()) || TypeLibrary.IsSubClassOf(type, typeFilterTextBox.Text))
                return GetPropertyCount(type) > 0;

            return false;
        }

        private int GetPropertyCount(Type type)
        {
            List<PropertyInfo> props = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly).ToList();

            for (int i = 0; i < props.Count; i++)
                if (props[i].Name.StartsWith("_"))
                    props.Remove(props[i]);

            return props.Count;
        }

        private void HideEmptyCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(typeFilterTextBox.Text))
                typesListBox.Items.Filter = a => ((TypeItem)a).Type.Name.ToLower().Contains(typeFilterTextBox.Text.ToLower()) || TypeLibrary.IsSubClassOf(((TypeItem)a).Type, typeFilterTextBox.Text);
            else
                typesListBox.Items.Filter = null;
        }

        private void FrostyTypeExplorer_Loaded(object sender, RoutedEventArgs e)
        {
            TypeItems = new List<TypeItem>();

            foreach (Type type in TypeLibrary.GetConcreteTypes())
            {
                TypeItems.Add(new TypeItem(type));
            }

            typesListBox.ItemsSource = TypeItems;
            typesListBox.Items.SortDescriptions.Add(new System.ComponentModel.SortDescription("Name", System.ComponentModel.ListSortDirection.Ascending));
            typesListBox.Items.Refresh();
        }
    }
}

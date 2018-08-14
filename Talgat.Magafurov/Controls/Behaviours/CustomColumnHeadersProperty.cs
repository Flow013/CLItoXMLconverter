using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Talgat.Magafurov.Controls.Behaviours
{
    /// <summary>
    /// The behavior for DateGrid that allows you to automatically generate column headers based on the attribute [Display(Name = "Name")]
    /// </summary>
    public static class CustomColumnHeadersProperty
    {
        public static DependencyProperty ItemTypeProperty = DependencyProperty.RegisterAttached(
            "ItemType",
            typeof(Type),
            typeof(CustomColumnHeadersProperty),
            new PropertyMetadata(OnItemTypeChanged));

        public static void SetItemType(DependencyObject obj, Type value)
        {
            obj.SetValue(ItemTypeProperty, value);
        }

        public static Type GetItemType(DependencyObject obj)
        {
            return (Type)obj.GetValue(ItemTypeProperty);
        }

        private static void OnItemTypeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var dataGrid = sender as DataGrid;

            if (args.NewValue != null)
                dataGrid.AutoGeneratingColumn += dataGrid_AutoGeneratingColumn;
            else
                dataGrid.AutoGeneratingColumn -= dataGrid_AutoGeneratingColumn;
        }

        private static void dataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            var type = GetItemType(sender as DataGrid);

            var displayAttribute = type.GetProperty(e.PropertyName).GetCustomAttributes(typeof(DisplayAttribute), false).FirstOrDefault() as DisplayAttribute;
            if (displayAttribute != null)
                e.Column.Header = displayAttribute.Name;
            if (!displayAttribute.GetAutoGenerateField().GetValueOrDefault(true))
                e.Column.Visibility = Visibility.Collapsed;
        }
    }
}
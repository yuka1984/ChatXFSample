using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace App1
{
    public class AppearingAttachProperty
    {
        public static readonly BindableProperty AppearingCommandProperty =
            BindableProperty.CreateAttached(
                "AppearingCommand",                     // プロパティ名
                typeof(ICommand),                       // プロパティの型
                typeof(Page),                           // 添付対象の型
                null,
                BindingMode.OneWay,                     // デフォルト BindingMode
                null,
                OnAppearingCommandPropertyChanged,      // プロパティが変更された時に呼び出されるデリゲート
                null,
                null);

        public static readonly BindableProperty DisAppearingCommandProperty =
            BindableProperty.CreateAttached(
                "DisAppearingCommand",                     // プロパティ名
                typeof(ICommand),                       // プロパティの型
                typeof(Page),                           // 添付対象の型
                null,
                BindingMode.OneWay,                     // デフォルト BindingMode
                null,
                OnAppearingCommandPropertyChanged,      // プロパティが変更された時に呼び出されるデリゲート
                null,
                null);

        public static ICommand GetAppearingCommand(BindableObject bindable)
        {
            return (ICommand)bindable.GetValue(AppearingAttachProperty.AppearingCommandProperty);
        }
        public static void SetAppearingCommand(BindableObject bindable, Command value)
        {
            bindable.SetValue(AppearingAttachProperty.AppearingCommandProperty, value);
        }

        public static void SetDisAppearingCommand(BindableObject bindable, Command value)
        {
            bindable.SetValue(AppearingAttachProperty.DisAppearingCommandProperty, value);
        }
        public static ICommand GetDisAppearingCommand(BindableObject bindable)
        {
            return (ICommand)bindable.GetValue(AppearingAttachProperty.DisAppearingCommandProperty);
        }

        public static void OnAppearingCommandPropertyChanged(BindableObject bindable, Object oldValue, Object newValue)
        {
            var page = bindable as Page;
            if (page == null)
            {
                return;
            }
            if (newValue != null)
            {
                page.Appearing += Page_Appearing;
            }
            else
            {
                page.Appearing -= Page_Appearing;
            }
        }

        private static void Page_Appearing(object sender, EventArgs e)
        {
            var command = GetAppearingCommand(sender as BindableObject);
            if(command?.CanExecute(e) == true)
            {
                command.Execute(e);
            }
        }

        public static void OnDisAppearingCommandPropertyChanged(BindableObject bindable, Object oldValue, Object newValue)
        {
            var page = bindable as Page;
            if (page == null)
            {
                return;
            }
            if (newValue != null)
            {
                page.Disappearing += Page_DisAppearing;
            }
            else
            {
                page.Disappearing -= Page_DisAppearing;
            }
        }

        private static void Page_DisAppearing(object sender, EventArgs e)
        {
            var command = GetAppearingCommand(sender as BindableObject);
            if (command?.CanExecute(e) == true)
            {
                command.Execute(e);
            }
        }
    }
}

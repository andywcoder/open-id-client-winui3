using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using System;

namespace Santolibre.OpenIdClient
{
    public sealed partial class MainPage : Page
    {
        public MainPageViewModel TypedDataContext { get; }

        public MainPage()
        {
            InitializeComponent();

            var viewModel = App.Current.Services.GetService<MainPageViewModel>();
            if (viewModel == null) { throw new NullReferenceException("MainPageViewModel is null"); }
            DataContext = viewModel;
            TypedDataContext = viewModel;
        }
    }
}

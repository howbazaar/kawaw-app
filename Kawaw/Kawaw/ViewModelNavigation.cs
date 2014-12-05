using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Kawaw
{
    class ViewModelNavigation
    {
        private readonly INavigation _navigation;
        private static Dictionary<Type, Type> Registered = new Dictionary<Type, Type>();

        public static void Register<TViewModel, TView>()
            where TViewModel : BaseViewModel
            where TView : Page
        {
            Registered[typeof (TViewModel)] = typeof (TView);
        }
        public static Page GetPageForViewModel(BaseViewModel viewModel)
        {
            var viewType = Registered[viewModel.GetType()];
            var instance = (BaseView)Activator.CreateInstance(viewType);
            instance.BindingContext = viewModel;
            return instance;
        }

        public ViewModelNavigation(INavigation navigation)
        {
            _navigation = navigation;
        }

        public Task PopAsync()
        {
            return _navigation.PopAsync();
        }
        public Task PopModalAsync()
        {
            return _navigation.PopModalAsync();
        }

        public Task PushAsync(BaseViewModel viewModel)
        {
            return _navigation.PushAsync(GetPageForViewModel(viewModel));
        }

        public Task PushModalAsync(BaseViewModel viewModel)
        {
            // maybe later take a param for the "wrap in nav view"
            return _navigation.PushModalAsync(new NavigationPage(GetPageForViewModel(viewModel)));
        }
    }
}